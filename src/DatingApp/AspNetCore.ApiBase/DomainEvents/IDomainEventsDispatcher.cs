using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.DomainEvents
{
    public interface IDomainEventsDispatcher
    {
        Task DispatchPreCommitAsync(IDomainEvent domainEvent);
        Task DispatchPostCommitAsync(IDomainEvent domainEvent);
        Task DispatchPostCommitBatchAsync(IEnumerable<IDomainEvent> domainEvent);
        Task HandlePostCommitAsync<T>(string handlerType, T domainEvent) where T : IDomainEvent;
    }
}
