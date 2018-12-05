using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using AspNetCore.ApiBase.MultiTenancy.Middleware;
using Microsoft.AspNetCore.Builder;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public static class MultiTenancyApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTenants<TTenant>(this IApplicationBuilder builder)
        where TTenant : AppTenant
        {
            return builder.UseMiddleware<TenantMiddleware<TTenant>>();
        }
    }
}
