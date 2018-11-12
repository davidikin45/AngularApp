using AspNetCore.ApiBase.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.DomainEvents
{
    public interface IDomainEventHandler<T>
        where T : IDomainEvent
    {
        IDictionary<string, string> HandleActions { get; }

        bool HandlePreCommitCondition(T domainEvent);
        Task<Result> HandlePreCommitAsync(T domainEvent);

        bool HandlePostCommitCondition(T domainEvent);
        Task<Result> HandlePostCommitAsync(T domainEvent);
    }
}
