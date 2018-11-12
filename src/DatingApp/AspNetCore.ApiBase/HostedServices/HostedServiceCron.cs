using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.HostedServices
{
    public class HostedServiceCron<TService> : IHostedService
      where TService : IHostedServiceCronJob
    {
        // Example untested base class code kindly provided by David Fowler: https://gist.github.com/davidfowl/a7dd5064d9dcf35b6eae1a7953d615e3

        private Task _executingTask;
        private CancellationTokenSource _cts;

        private readonly ILogger _logger;

        public HostedServiceCron(IServiceProvider services, ILogger<HostedServiceCron<TService>> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        private async Task ScheduledTask(CancellationToken ct)
        {
            var cronSchedule = (CronJobAttribute)typeof(TService).GetCustomAttributes(typeof(CronJobAttribute), true).FirstOrDefault();
            if(cronSchedule == null)
            {
                throw new Exception("Job must have a CronJobAttribute");
            }

            var schedules = cronSchedule.Schedules.Select(schedule => CrontabSchedule.Parse(schedule));

            do
            {
                var currentTime = DateTime.UtcNow;
               var nextOccurence = schedules.Select(schedule => schedule.GetNextOccurrence(currentTime)).Min();

                var delay = nextOccurence - currentTime;
                if(delay.Seconds > 0)
                {
                    await Task.Delay(delay);
                }

                using (var scope = Services.CreateScope())
                {
                    var scopedProcessingService =
                        scope.ServiceProvider
                            .GetRequiredService<TService>();

                    _logger.LogInformation("Executing CronJob {CronJob}", typeof(TService));
                    try
                    {
                        await scopedProcessingService.ExecuteAsync(ct);
                        _logger.LogInformation("CronJob {CronJob} completed successfully", typeof(TService));
                    }
                    catch
                    {
                        _logger.LogInformation("CronJob {CronJob} failed", typeof(TService));
                    }
                }

            }
            while (!ct.IsCancellationRequested);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Store the task we're executing
            _executingTask = ScheduledTask(_cts.Token);

            // If the task is completed then return it, otherwise it's running
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            // Signal cancellation to the executing method
            _cts.Cancel();

            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

            // Throw if cancellation triggered
            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
