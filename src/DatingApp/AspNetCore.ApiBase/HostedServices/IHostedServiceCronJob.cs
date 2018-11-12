using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.HostedServices
{
    public interface IHostedServiceCronJob
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
