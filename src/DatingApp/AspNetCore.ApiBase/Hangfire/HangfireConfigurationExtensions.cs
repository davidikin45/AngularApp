using AspNetCore.ApiBase.Helpers;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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
            if (ConnectionStringHelper.IsSQLite(connectionString))
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

        public static IApplicationBuilder UseHangfire(this IApplicationBuilder builder, string route = "/admin/hangfire")
        {

            builder.UseHangfireDashboard(route, new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationfilter() },
                AppPath = route.Replace("/hangfire", "")
            });
            builder.UseHangfireServer();
            return builder;
        }

        public static IServiceCollection AddHangfireJob<HangfireJob>(this IServiceCollection services)
            where HangfireJob : class
        {
            return services.AddTransient<HangfireJob>();
        }
    }
}
