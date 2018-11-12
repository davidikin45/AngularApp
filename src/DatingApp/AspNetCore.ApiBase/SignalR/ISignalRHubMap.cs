using Microsoft.AspNetCore.SignalR;

namespace AspNetCore.ApiBase.SignalR
{
    public interface ISignalRHubMap
    {
        void MapHub(HubRouteBuilder routes, string hubPathPrefix);
    }
}
