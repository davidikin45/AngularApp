using AspNetCore.ApiBase.Settings;
using AspNetCore.ApiBase.Validation;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.DomainEvents
{
    public class DomainEventsMediator : IDomainEventsMediator
    {
        public static bool HandlePostCommitEventsInProcess = false;
        public static bool DispatchPostCommitEventsInParellel = true;

        private readonly IServiceProvider _serviceProvider;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ServerSettings _serverSettings;

        public DomainEventsMediator(IServiceProvider serviceProvider, IBackgroundJobClient backgroundJobClient, ServerSettings serverSettings)
        {
            _serviceProvider = serviceProvider;
            _backgroundJobClient = backgroundJobClient;
            _serverSettings = serverSettings;
        }

        #region Get Event Handlers
        private (List<dynamic> eventHandlers, List<dynamic> dynamicEventHandlers) GetEventHandlerInstancesForPreCommit(IServiceProvider services, DomainEventMessage domainEventMessage)
        {
            List<dynamic> instances = new List<dynamic>();

            if (domainEventMessage.DomainEventTypeExists)
            {
                var domainEvent = domainEventMessage.DomainEvent;

                var eventHandlerInterfaceType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                var types = typeof(IEnumerable<>).MakeGenericType(eventHandlerInterfaceType);
                dynamic handlers = services.GetService(types);

                foreach (var handler in handlers)
                {
                    if (handler.HandlePreCommitCondition((dynamic)domainEvent))
                    {
                        instances.Add(handler);
                    }
                }
            }

            List<dynamic> dynamicInstances = new List<dynamic>();

            var dynamicDomainEvent = domainEventMessage.DomainEventAsDynamic();

            dynamic dynamicHandlers = services.GetService(typeof(IDynamicDomainEventHandler));

            foreach (var handler in dynamicHandlers)
            {
                if (handler.EventNames.Contains(domainEventMessage.EventName) && handler.HandlePreCommitCondition((dynamic)dynamicDomainEvent))
                {
                    dynamicInstances.Add(handler);
                }
            }

            return (instances, dynamicInstances);
        }

        private (List<dynamic> eventHandlers, List<dynamic> dynamicEventHandlers) GetEventHandlerInstancesForPostCommit(IServiceProvider services, DomainEventMessage domainEventMessage)
        {
            List<dynamic> instances = new List<dynamic>();

            if (domainEventMessage.DomainEventTypeExists)
            {
                var domainEvent = domainEventMessage.DomainEvent;

                var eventHandlerInterfaceType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                var types = typeof(IEnumerable<>).MakeGenericType(eventHandlerInterfaceType);
                dynamic handlers = services.GetService(types);

                foreach (var handler in handlers)
                {
                    if (handler.HandlePostCommitCondition((dynamic)domainEvent))
                    {
                        instances.Add(handler);
                    }
                }
            }

            List<dynamic> dynamicInstances = new List<dynamic>();

            var dynamicDomainEvent = domainEventMessage.DomainEventAsDynamic();

            dynamic dynamicHandlers = services.GetService(typeof(IDynamicDomainEventHandler));

            foreach (var handler in dynamicHandlers)
            {
                if (handler.EventNames.Contains(domainEventMessage.EventName) && handler.HandlePostCommitCondition((dynamic)dynamicDomainEvent))
                {
                    dynamicInstances.Add(handler);
                }
            }

            return (instances, dynamicInstances);
        }

        private List<Type> GetEventHandlerTypesForPostCommit(DomainEventMessage domainEventMessage)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handlers = GetEventHandlerInstancesForPostCommit(scope.ServiceProvider, domainEventMessage);
                var types = handlers.eventHandlers.Select(x => (Type)x.GetType()).Concat(handlers.dynamicEventHandlers.Select(x => (Type)x.GetType())).ToList();
                return types;
            }
        }
        #endregion

        #region Dispatch Pre Commit InProcess Domain Events
        public async Task DispatchPreCommitAsync(IDomainEvent domainEvent)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var domainEventMessage = new DomainEventMessage(_serverSettings.ServerName, domainEvent);

                var handlers = GetEventHandlerInstancesForPreCommit(scope.ServiceProvider, domainEventMessage);

                //pre commit events are atomic
                foreach (var handler in handlers.eventHandlers)
                {
                    Result result = await handler.HandlePreCommitAsync((dynamic)domainEvent);
                    if (result.IsFailure)
                    {
                        throw new Exception("Pre Commit Event Failed");
                    }
                }

                foreach (var handler in handlers.dynamicEventHandlers)
                {
                    Result result = await handler.HandlePreCommitAsync((dynamic)domainEvent);
                    if (result.IsFailure)
                    {
                        throw new Exception("Pre Commit Event Failed");
                    }
                }

            }
        }
        #endregion

        #region Dispatch Post Commit Integration Events
        public async Task DispatchPostCommitBatchAsync(IEnumerable<IDomainEvent> domainEvents)
        {
            if (DispatchPostCommitEventsInParellel)
            {
                await Task.Run(() => Parallel.ForEach(domainEvents, async domainEvent =>
                {
                    await DispatchPostCommitAsync(domainEvent);
                }));
            }
            else
            {
                foreach (var domainEvent in domainEvents)
                {
                    await DispatchPostCommitAsync(domainEvent);
                }
            }
        }

        public async Task DispatchPostCommitAsync(IDomainEvent domainEvent)
        {
            var domainEventMessage = new DomainEventMessage(_serverSettings.ServerName, domainEvent);

            if (HandlePostCommitEventsInProcess)
            {
                try
                {
                    await HandlePostCommitDispatchAsync(domainEventMessage).ConfigureAwait(false);
                }
                catch
                {
                    //Log InProcess Post commit event failed
                }
            }
            else
            {
                var job = Job.FromExpression<IDomainEventsMediator>(m => m.HandlePostCommitDispatchAsync(domainEventMessage));

                foreach (var queueName in _serverSettings.ServerNames)
                {
                    try
                    {
                        var queue = new EnqueuedState(queueName);
                        _backgroundJobClient.Create(job, queue);
                    }
                    catch
                    {
                        //Log Hangfire Post commit event Background enqueue failed
                    }
                }
            }
        }
        #endregion

        #region Handle Post Commit Integration Events - Generally handled out of process in HangFire
        //Event Dispatcher
        public async Task HandlePostCommitDispatchAsync(DomainEventMessage domainEventMessage)
        {
            var eventHandlerTypes = GetEventHandlerTypesForPostCommit(domainEventMessage);


            if (DispatchPostCommitEventsInParellel)
            {
                await Task.Run(() => Parallel.ForEach(eventHandlerTypes, async handlerType =>
                {
                    var domainEventHandlerMessage = new DomainEventHandlerMessage(handlerType.FullName, typeof(IDynamicDomainEventHandler).IsAssignableFrom(handlerType) ? true : false, domainEventMessage);
                    await DispatchPostCommitAsync(domainEventHandlerMessage).ConfigureAwait(false);
                }));
            }
            else
            {
                foreach (Type handlerType in eventHandlerTypes)
                {
                    var domainEventHandlerMessage = new DomainEventHandlerMessage(handlerType.FullName, typeof(IDynamicDomainEventHandler).IsAssignableFrom(handlerType) ? true : false, domainEventMessage);
                    await DispatchPostCommitAsync(domainEventHandlerMessage).ConfigureAwait(false);
                }
            }

        }

        private async Task DispatchPostCommitAsync(DomainEventHandlerMessage domainEventHandlerMessage)
        {
            if (HandlePostCommitEventsInProcess)
            {
                try
                {
                    await HandlePostCommitAsync(domainEventHandlerMessage).ConfigureAwait(false);
                }
                catch
                {
                    //Log InProcess Post commit event failed
                }
            }
            else
            {
                try
                {
                    //Each Post Commit Domain Event Handling is completely independent. By registering the event AND handler (rather than just the event) in hangfire we get the granularity of retrying at a event/handler level.
                    //Hangfire unfortunately uses System.Type.GetType to get job type. This only looks at the referenced assemblies of the web project and not the dynamic loaded plugins so need to
                    //proxy back through this common assembly.

                    var job = Job.FromExpression<IDomainEventsMediator>(m => m.HandlePostCommitAsync(domainEventHandlerMessage));

                    var queue = new EnqueuedState(_serverSettings.ServerName);
                    _backgroundJobClient.Create(job, queue);

                }
                catch
                {
                    //Log Hangfire Post commit event Background enqueue failed
                }
            }
        }

        //Event Handler
        public async Task HandlePostCommitAsync(DomainEventHandlerMessage domainEventHandlerMessage)
        {
            Type handlerType = System.Type.GetType(domainEventHandlerMessage.HandlerType);
            if (handlerType == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    handlerType = assembly.GetType(domainEventHandlerMessage.HandlerType);
                    if (handlerType != null)
                    {
                        break;
                    }
                }
            }

            if (handlerType == null)
            {
                throw new Exception("Invalid handler type");
            }

            if (domainEventHandlerMessage.IsDynamic)
            {
                await HandlePostCommitAsync(handlerType, domainEventHandlerMessage.DomainEventMessage.DomainEventAsDynamic()).ConfigureAwait(false);
            }
            else
            {
                await HandlePostCommitAsync(handlerType, domainEventHandlerMessage.DomainEventMessage.DomainEvent).ConfigureAwait(false);
            }
        }

        private async Task HandlePostCommitAsync(Type handlerType, dynamic domainEvent)
        {
            dynamic handler = _serviceProvider.GetService(handlerType);
            Result result = await handler.HandlePostCommitAsync(domainEvent);
            if (result.IsFailure)
            {
                throw new Exception("Post Commit Event Failed");
            }
        }
        #endregion
    }
}
