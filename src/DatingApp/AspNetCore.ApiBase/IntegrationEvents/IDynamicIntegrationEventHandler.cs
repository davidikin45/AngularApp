using System.Threading.Tasks;

namespace AspNetCore.ApiBase.IntegrationEvents
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
