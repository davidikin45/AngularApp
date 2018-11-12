using AspNetCore.ApiBase.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public static class TenantProviderExtensions
    {
        public static IServiceCollection AddMultiTenancyDbContext<TContext, TTentant>(this IServiceCollection services, string connectionString) 
            where TContext : TenantsDbContextBase<TTentant>
            where TTentant : AppTenant
        {
            services.AddDbContext<TContext>(connectionString);
            services.AddTransient<ITenantProvider<TTentant>, TenantProvider<TContext, TTentant>>();
            return services.AddTransient(sp => sp.GetService<ITenantProvider<TTentant>>().GetTenant());
        }
    }

    public class TenantProvider<TContext, TTentant> : ITenantProvider<TTentant> 
        where TContext : TenantsDbContextBase<TTentant>
        where TTentant : AppTenant
    {
        private TTentant tenant;

        public TenantProvider(TContext context, IHttpContextAccessor accessor)
        {
            var host = accessor.HttpContext.Request.Host.Value;
            tenant = context.Tenants.First(t => t.Active && t.HostNames.Contains(host));
        }

        public TTentant GetTenant()
        {
            return tenant;
        }
    }
}
