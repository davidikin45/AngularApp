using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using AspNetCore.ApiBase.MultiTenancy.Middleware;
using Microsoft.AspNetCore.Builder;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public static class MultiTenancyApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTenants<TContext, TTenant>(this IApplicationBuilder builder)
        where TContext : DbContextTenantsBase<TTenant>
        where TTenant : AppTenant
        {
            return builder.UseMiddleware<TenantMiddleware<TContext, TTenant>>();
        }
    }
}
