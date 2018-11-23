using System.Threading;
using System.Threading.Tasks;
using AspNetCore.ApiBase.HostedServices;
using Microsoft.Extensions.Logging;

namespace DatingApp.Api.Jobs
{
    [CronJob("1/2 * * * *", "1/3 * * * *")]
    public class Job2 : IHostedServiceCronJob
    {
        private ILogger _Logger;

        public Job2(ILogger<Job2> logger)
        {
            _Logger = logger;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
