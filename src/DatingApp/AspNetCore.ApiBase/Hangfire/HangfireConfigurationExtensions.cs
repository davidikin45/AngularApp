using AspNetCore.ApiBase.Helpers;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCore.ApiBase.Hangfire
{
    public static class HangfireConfigurationExtensions
    {
        public static IServiceCollection AddHangfire(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return services.AddHangfireInMemory();
            }
            else if (ConnectionStringHelper.IsSQLite(connectionString))
            {
                return services.AddHangfireSqlLite(connectionString);
            }
            else
            {
                return services.AddHangfireSqlServer(connectionString);
            }
        }

        public static IServiceCollection AddHangfireInMemory(this IServiceCollection services)
        {
            return services.AddHangfire(config =>
            {
                config.UseFilter(new HangfireLoggerAttribute());
                config.UseMemoryStorage();
            });
        }

        public static IServiceCollection AddHangfireSqlServer(this IServiceCollection services, string connectionString)
        {
            return services.AddHangfire(config =>
            {
                config.UseFilter(new HangfireLoggerAttribute());
                config.UseSqlServerStorage(connectionString);
            });
        }

        public static IServiceCollection AddHangfireSqlLite(this IServiceCollection services, string connectionString)
        {
            return services.AddHangfire(config =>
            {
                config.UseFilter(new HangfireLoggerAttribute());
                config.UseSQLiteStorage(connectionString);
            });
        }

        public static IApplicationBuilder UseHangfire(this IApplicationBuilder builder, string serverName, string route = "/admin/hangfire")
        {

            builder.UseHangfireDashboard(route, new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationfilter() },
                AppPath = route.Replace("/hangfire", "")
            });

            //each microserver has its own queue. Queue by using the Queue attribute.
            //https://discuss.hangfire.io/t/one-queue-for-the-whole-farm-and-one-queue-by-server/490
            var options = new BackgroundJobServerOptions
            {
                ServerName = serverName,
                Queues = new[] { serverName, "default" }
            };

            //https://discuss.hangfire.io/t/one-queue-for-the-whole-farm-and-one-queue-by-server/490/3

            builder.UseHangfireServer(options);
            return builder;
        }

        public static IServiceCollection AddHangfireJob<HangfireJob>(this IServiceCollection services)
            where HangfireJob : class
        {
            return services.AddTransient<HangfireJob>();
        }
    }
}
