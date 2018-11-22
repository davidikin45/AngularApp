using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public interface ITenantIdentificationService<TContext, TTenant> : ITenantIdentificationStrategy
        where TContext : DbContextTenantsBase<TTenant>
        where TTenant : AppTenant

    {
        TTenant GetTenant(HttpContext httpContext);
    }
}
