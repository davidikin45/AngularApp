using Nest;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.DomainEvents
{
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

    public interface IDynamicDomainEventHandler
    {
        string[] EventNames { get; }

        bool HandlePreCommitCondition(dynamic domainEvent);
        Task<Result> HandlePreCommitAsync(dynamic domainEvent);

        //Integration Event
        bool HandlePostCommitCondition(dynamic domainEvent);
        Task<Result> HandlePostCommitAsync(dynamic domainEvent);
    }
}