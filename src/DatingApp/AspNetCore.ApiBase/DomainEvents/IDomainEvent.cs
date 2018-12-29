using AspNetCore.ApiBase.Validation;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.DomainEvents
{
    public interface IDomainEvent
    {
    }

    public interface IDomainEventHandler<T>
        where T : IDomainEvent
    {
        //Domain Event
        bool HandlePreCommitCondition(T domainEvent);
        Task<Result> HandlePreCommitAsync(T domainEvent);

        //Integration Event
        bool HandlePostCommitCondition(T domainEvent);
        Task<Result> HandlePostCommitAsync(T domainEvent);
    }
}
