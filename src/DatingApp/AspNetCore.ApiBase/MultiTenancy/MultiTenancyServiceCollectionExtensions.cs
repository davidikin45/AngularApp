using AspNetCore.ApiBase.Extensions;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using AspNetCore.ApiBase.MultiTenancy.Middleware;
using AspNetCore.ApiBase.MultiTenancy.Mvc;
using AspNetCore.ApiBase.MultiTenancy.Request;
using AspNetCore.ApiBase.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Reflection;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public static class MultiTenancyServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContextMultiTenancyInMemory<TContext, TTentant>(this IServiceCollection services, IConfiguration configuration, ServiceLifetime contextLifetime = ServiceLifetime.Singleton)
           where TContext : DbContextTenantsBase<TTentant>
           where TTentant : AppTenant
        {
            return services.AddDbContextMultiTenancy<TContext, TTentant>("", configuration);
        }

        public static IServiceCollection AddDbContextMultiTenancy<TContext, TTentant>(this IServiceCollection services, string connectionString, IConfiguration configuration, ServiceLifetime contextLifetime = ServiceLifetime.Singleton)
            where TContext : DbContextTenantsBase<TTentant>
            where TTentant : AppTenant
        {
            services.AddDbContext<TContext>(connectionString, ServiceLifetime.Scoped)
                    .AddTenantService<TContext, TTentant>()
                    .AddTenantStrategyService<TContext, TTentant>()
                    .AddTenantMiddleware<TContext, TTentant>()
                    .AddTenantLocations<TTentant>()
                    .AddTenantRequestIdentification<TContext, TTentant>().TenantForHostQueryStringSourceIP()
                    .AddTenantConfiguration<TTentant>(Assembly.GetCallingAssembly())
                    .AddTenantSettings<TTentant>(configuration);

            return services;
        }

        public static IServiceCollection AddTenantSettings<TTenant>(this IServiceCollection services, IConfiguration configuration)
            where TTenant : AppTenant
        {
            services.Configure<TenantSettings<TTenant>>(configuration.GetSection("TenantSettings"));
            return services.AddTransient(sp => sp.GetService<IOptions<TenantSettings<TTenant>>>().Value);
        }

        public static IServiceCollection AddTenantService<TContext, TTenant>(this IServiceCollection services)
            where TContext : DbContextTenantsBase<TTenant>
            where TTenant : AppTenant
        {
            return services
                .AddHttpContextAccessor()
                .AddScoped<MultiTenantService<TContext, TTenant>>()
                .AddScoped<ITenantService>(x => x.GetRequiredService<MultiTenantService<TContext, TTenant>>())
                .AddScoped<ITenantService<TTenant>>(x => x.GetRequiredService<MultiTenantService<TContext, TTenant>>())
                .AddScoped(sp => sp.GetService<ITenantService<TTenant>>().GetTenant());
        }

        public static IServiceCollection AddTenantStrategyService<TContext, TTenant>(this IServiceCollection services)
            where TContext : DbContextTenantsBase<TTenant>
            where TTenant : AppTenant
        {
            return services.AddSingleton<ITenantDbContextStrategyService, MultiTenantDbContextStrategyService>();
        }

        public static TenantRequestIdentification<TContext,TTenant> AddTenantRequestIdentification<TContext, TTenant>(this IServiceCollection services)
             where TContext : DbContextTenantsBase<TTenant>
            where TTenant : AppTenant
        {
            return new TenantRequestIdentification<TContext, TTenant>(services);
        }

        public static TenantDbContextIdentification<TContext> AddTenantDbContextIdentification<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            return new TenantDbContextIdentification<TContext>(services);
        }

        public static IServiceCollection AddTenantLocations<TTenant>(this IServiceCollection services, string mvcImplementationFolder = "Mvc/")
             where TTenant : AppTenant
        {
            return services.Configure<RazorViewEngineOptions>(options =>
            {
                if (!(options.ViewLocationExpanders.FirstOrDefault() is TenantViewLocationExpander<TTenant>))
                {
                    options.ViewLocationExpanders.Insert(0, new TenantViewLocationExpander<TTenant>(mvcImplementationFolder));
                }
            });
        }

        public static IServiceCollection AddTenantConfiguration<TTenant>(this IServiceCollection services, Assembly assembly)
            where TTenant : AppTenant
        {
            var types = assembly
                .GetExportedTypes()
                .Where(type => typeof(ITenantConfiguration).IsAssignableFrom(type))
                .Where(type => (type.IsAbstract == false) && (type.IsInterface == false));

            foreach (var type in types)
            {
                services.AddSingleton(typeof(ITenantConfiguration), type);
            }

            return services;
        }

        public static IServiceCollection AddTenantConfiguration<TTenant>(this IServiceCollection services)
            where TTenant : AppTenant
        {
            var target = Assembly.GetCallingAssembly();
            return services.AddTenantConfiguration<TTenant>(target);
        }

        public static IServiceCollection AddTenantMiddleware<TContext, TTentant>(this IServiceCollection services)
           where TContext : DbContextTenantsBase<TTentant>
           where TTentant : AppTenant
        {
            return services.AddSingleton<IStartupFilter, TenantStartupFilter<TContext, TTentant>>();
        }
    }
}
