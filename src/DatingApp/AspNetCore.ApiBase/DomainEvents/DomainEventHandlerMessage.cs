using Newtonsoft.Json;
using System;

namespace AspNetCore.ApiBase.DomainEvents
{
    public class DomainEventHandlerMessage
    {
        public DomainEventHandlerMessage(string handlerType, bool isDynamic, DomainEventMessage domainEventMessage)
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            HandlerType = handlerType;
            IsDynamic = isDynamic;
            DomainEventMessage = domainEventMessage;
        }

        [JsonConstructor]
        public DomainEventHandlerMessage(Guid id, DateTime createDate, string handlerType, bool isDynamic, DomainEventMessage domainEventMessage)
        {
            Id = id;
            CreationDate = createDate;
            HandlerType = handlerType;
            IsDynamic = isDynamic;
            DomainEventMessage = domainEventMessage;
        }

        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public DateTime CreationDate { get; private set; }

        [JsonProperty]
        public string HandlerType { get; private set; }

        [JsonProperty]
        public bool IsDynamic { get; private set; }

        [JsonProperty]
        public DomainEventMessage DomainEventMessage { get; private set; }
    }
}
