using AspNetCore.ApiBase.Domain;
using AspNetCore.ApiBase.DomainCommands;
using AutoMapper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.ApiBase.DomainEvents
{
    public class DomainCommandsService: IDomainCommandsService
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainCommandsService(IServiceProvider serviceProvider)
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

            var mapper = (IMapper)_serviceProvider.GetService(typeof(IMapper));
            var mapping = mapper.ConfigurationProvider.GetAllTypeMaps().Where(m => m.SourceType == dtoType && typeof(IEntity).IsAssignableFrom(m.DestinationType)).FirstOrDefault();
            if (mapping != null)
            {
                var entityType = mapping.DestinationType;
                actions = GetActions(entityType);
            }

            return actions;
        }

        public Dictionary<string, List<string>> GetActions(Type entityType)
        {
            Dictionary<string, List<string>> actions = new Dictionary<string, List<string>>();

            IDomainCommand actionEvent = null;

            if (typeof(IEntity).IsAssignableFrom(entityType))
            {
                Type genericType = typeof(EntityCommand<>);
                Type[] typeArgs = { entityType };
                Type constructed = genericType.MakeGenericType(typeArgs);
                actionEvent = (IDomainCommand)Activator.CreateInstance(constructed, null, null, null, null);
            }

            if (actionEvent != null)
            {
                var eventHandlerInterfaceType = typeof(IDomainEventHandler<>).MakeGenericType(actionEvent.GetType());
                var types = typeof(IEnumerable<>).MakeGenericType(eventHandlerInterfaceType);

                dynamic handlers = _serviceProvider.GetService(types);

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

            return actions;
        }

        public IDomainCommand CreateEntityActionEvent(string action, JObject payload, object entity, string triggeredBy)
        {
            IDomainCommand actionEvent = null;

            if (entity is IEntity)
            {
                Type genericType = typeof(EntityCommand<>);
                Type[] typeArgs = { entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                actionEvent = (IDomainCommand)Activator.CreateInstance(constructed, action, payload, entity, triggeredBy);
            }

            return actionEvent;
        }
    }
}
