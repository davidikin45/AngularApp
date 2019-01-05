using System.Threading.Tasks;

namespace AspNetCore.ApiBase.IntegrationEvents
{
    public interface IIntegrationEventHandler<in TIntegrationEvent>
       where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }
}
