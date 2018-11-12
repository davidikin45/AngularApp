using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.ApiBase.HostedServices
{
    public static class HostedServiceExtensionMethods
    {
        public static IServiceCollection AddHostedServiceCronJob<TCronJob>(this IServiceCollection services)
            where TCronJob : class, IHostedServiceCronJob
        {
            services.AddScoped<TCronJob>();
            return services.AddHostedService<HostedServiceCron<TCronJob>>();
        }
    }
}
