using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.MultiTenancy.Middleware
{
    public class TenantMiddleware<TContext, TTenant>
        where TContext : DbContextTenantsBase<TTenant>
        where TTenant : AppTenant
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            if (context.Items.ContainsKey("Tenant") == false)
            {
                var tenantConfiguration = context.RequestServices.GetServices<ITenantConfiguration>();
                var service = context.RequestServices.GetService<ITenantIdentificationService<TTenant>>();
                var dbContext = context.RequestServices.GetService<TContext>();
                var tenant = service.GetTenant(context, dbContext);
                var configuration = context.RequestServices.GetService<IConfiguration>();
                var environment = context.RequestServices.GetService<IHostingEnvironment>();
                var providers = ((configuration as ConfigurationRoot).Providers as List<IConfigurationProvider>);
                var provider = providers.OfType<TenantJsonConfigurationProvider>().SingleOrDefault();
                if (provider == null)
                {
                    providers.Insert(0, new TenantJsonConfigurationProvider($"appsettings.{tenant}.{environment.EnvironmentName}.json"));
                }
                else
                {
                    provider.Source.Path = $"appsettings.{tenant}.{environment.EnvironmentName}.json";
                }

                context.Items["Tenant"] = tenant;
            }

            return this._next(context);
        }
    }
}
