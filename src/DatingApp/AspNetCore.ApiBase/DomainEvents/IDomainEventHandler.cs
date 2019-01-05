using AspNetCore.ApiBase.Validation;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.DomainEvents
{
    public interface IDomainEventHandler<T>
        where T : IDomainEvent
    {
        Task<Result> HandlePreCommitAsync(T domainEvent);
        Task<Result> HandlePostCommitAsync(T domainEvent);
    }

    public interface IDynamicDomainEventHandler
    {
        Task<Result> HandlePreCommitAsync(dynamic domainEvent);
        Task<Result> HandlePostCommitAsync(dynamic domainEvent);
    }
}