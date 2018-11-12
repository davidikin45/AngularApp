using AspNetCore.ApiBase.Domain;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.ApiBase.DomainEvents
{
    public class ActionEventsService: IActionEventsService
    {
        private readonly IServiceProvider _serviceProvider;

        public ActionEventsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool IsValidAction<T>(string action)
        {
            return GetActionsForDto(typeof(T)).ContainsKey(action);
        }

        public Dictionary<string, List<string>> GetActionsForDto(Type dtoType)
        {
            Dictionary<string, List<string>> actions = new Dictionary<string, List<string>>();

            using (var scope = _serviceProvider.CreateScope())
            {
                var mapper = (IMapper)scope.ServiceProvider.GetService(typeof(IMapper));
                var mapping = mapper.ConfigurationProvider.GetAllTypeMaps().Where(m => m.SourceType == dtoType && typeof(IEntity).IsAssignableFrom(m.DestinationType)).FirstOrDefault();
                if (mapping != null)
                {
                    var entityType = mapping.DestinationType;
                    actions = GetActions(entityType);
                }
            }

            return actions;
        }

        public Dictionary<string, List<string>> GetActions(Type entityType)
        {
            Dictionary<string, List<string>> actions = new Dictionary<string, List<string>>();

            IDomainActionEvent actionEvent = null;

            if (typeof(IEntity).IsAssignableFrom(entityType))
            {
                Type genericType = typeof(EntityActionEvent<>);
                Type[] typeArgs = { entityType };
                Type constructed = genericType.MakeGenericType(typeArgs);
                actionEvent = (IDomainActionEvent)Activator.CreateInstance(constructed, null, null, null, null);
            }

            if (actionEvent != null)
            {
                var eventHandlerInterfaceType = typeof(IDomainEventHandler<>).MakeGenericType(actionEvent.GetType());
                var types = typeof(IEnumerable<>).MakeGenericType(eventHandlerInterfaceType);

                using (var scope = _serviceProvider.CreateScope())
                {
                    dynamic handlers = scope.ServiceProvider.GetService(types);

                    foreach (var handler in handlers)
                    {
                        IDictionary<string, string> handleActions = (IDictionary<string, string>)handler.HandleActions;
                        foreach (var handleAction in handleActions)
                        {
                            if (!actions.ContainsKey(handleAction.Key))
                            {
                                actions.Add(handleAction.Key, new List<string>());
                            }

                            if (!actions[handleAction.Key].Contains(handleAction.Value))
                            {
                                actions[handleAction.Key].Add(handleAction.Value);
                            }
                        }
                    }

                }                 
            }

            return actions;
        }

        public IDomainActionEvent CreateEntityActionEvent(string action, dynamic args, object entity, string triggeredBy)
        {
            IDomainActionEvent actionEvent = null;

            if (entity is IEntity)
            {
                Type genericType = typeof(EntityActionEvent<>);
                Type[] typeArgs = { entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                actionEvent = (IDomainActionEvent)Activator.CreateInstance(constructed, action, args, entity, triggeredBy);
            }

            return actionEvent;
        }
    }
}
