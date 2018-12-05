using AspNetCore.ApiBase.DomainCommands;
using AspNetCore.ApiBase.DomainEvents;
using System.Collections.Generic;

namespace AspNetCore.ApiBase.Domain
{
    public interface IEntityAggregateRoot
    {
        void AddDomainCommand(IDomainCommand actionEvent);
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        void ClearEvents();
    }
}
