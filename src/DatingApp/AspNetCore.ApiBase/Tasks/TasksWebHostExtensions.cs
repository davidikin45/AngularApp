using AspNetCore.ApiBase.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Hosting
{
    public static class TasksWebHostExtensions
    {
        public static async Task InitAsync(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                if (scope.ServiceProvider.GetService<TaskRunnerInitialization>() != null)
                {
                    var taskRunner = scope.ServiceProvider.GetRequiredService<TaskRunnerInitialization>();
                    await taskRunner.RunTasksAfterApplicationConfigurationAsync();
                }
            }
        }
    }
}
