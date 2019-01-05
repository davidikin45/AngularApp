using AspNetCore.ApiBase.DomainEvents;
using System;
using System.Collections.Generic;

namespace AspNetCore.ApiBase.Domain
{
    public abstract class EntityAggregateRootBase<T> : EntityBase<T>, IEntityAggregateRoot, IEntityConcurrencyAware where T : IEquatable<T>
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        public virtual IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

        //Deferred domain events
        //https://blogs.msdn.microsoft.com/cesardelatorre/2017/03/23/using-domain-events-within-a-net-core-microservice/
        protected virtual void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        protected virtual void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public virtual void ClearEvents()
        {
            _domainEvents.Clear();
        }

        //Optimistic Concurrency. Potentially ETags serve the same purpose
        public byte[] RowVersion { get; set; }
    }
}
