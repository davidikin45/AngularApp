using AspNetCore.ApiBase.DomainEvents;
using System.Collections.Generic;

namespace AspNetCore.ApiBase.Domain
{
    public interface IEntityAggregateRoot
    {
        void AddActionEvent(IDomainActionEvent actionEvent);
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        void ClearEvents();
    }
}
