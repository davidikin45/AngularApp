using AspNetCore.ApiBase.Extensions;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using AspNetCore.ApiBase.MultiTenancy.Middleware;
using AspNetCore.ApiBase.MultiTenancy.Mvc;
using AspNetCore.ApiBase.MultiTenancy.Request;
using AspNetCore.ApiBase.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
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
            services.AddDbContext<TContext>(connectionString)
                    .AddTenantService<TContext, TTentant>()
                    .AddTenantMiddleware<TContext, TTentant>()
                    .AddTenantLocations<TTentant>()
                    .AddTenantRequestIdentification().TenantForHostQueryStringSourceIP<TTentant>()
                    .AddTenantDbContextIdentitication().DifferentConnectionForTenant<TTentant>()
                    .AddTenantConfiguration<TTentant>()
                    .AddTenantSettings<TTentant>(configuration);

            return services;
        }

        public static IServiceCollection AddTenantSettings<TTenant>(this IServiceCollection services, IConfiguration configuration)
            where TTenant : AppTenant
        {
            services.Configure<TenantSettings<TTenant>>(configuration.GetSection("TenantSettings"));
            return services.AddTransient(sp => sp.GetService<IOptions<TenantSettings<TTenant>>>().Value);
        }

        public static IServiceCollection AddTenantService<TContext, TTentant>(this IServiceCollection services)
            where TContext : DbContextTenantsBase<TTentant>
            where TTentant : AppTenant
        {
            return services
                .AddHttpContextAccessor()
                .AddScoped<MultiTenantService<TContext, TTentant>>()
                .AddScoped<ITenantService>(x => x.GetRequiredService<MultiTenantService<TContext, TTentant>>())
                .AddScoped<ITenantService<TTentant>>(x => x.GetRequiredService<MultiTenantService<TContext, TTentant>>())
                .AddScoped(sp => sp.GetService<ITenantService<TTentant>>().GetTenant());
        }

        public static TenantRequestIdentification AddTenantRequestIdentification(this IServiceCollection services)
        {
            return new TenantRequestIdentification(services);
        }

        public static TenantDbContextIdentification AddTenantDbContextIdentitication(this IServiceCollection services)
        {
            return new TenantDbContextIdentification(services);
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

            services.AddScoped(typeof(ITenantConfiguration), sp =>
            {
                var svc = sp.GetRequiredService<ITenantService<TTenant>>();
                var configuration = sp.GetRequiredService<IConfiguration>();

                var tenantId = svc.GetTenant().Id;
                var instance = types
                    .Select(type => ActivatorUtilities.CreateInstance(sp, type))
                    .OfType<ITenantConfiguration>()
                    .SingleOrDefault(x => x.TenantId == tenantId);


                if (instance != null)
                {
                    instance.Configure(configuration);
                    instance.ConfigureServices(services);

                    sp.GetRequiredService<IHttpContextAccessor>().HttpContext.RequestServices = services.BuildServiceProvider();

                    return instance;
                }
                else
                {
                    return DummyTenantServiceProviderConfiguration.Instance;
                }
            });

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
