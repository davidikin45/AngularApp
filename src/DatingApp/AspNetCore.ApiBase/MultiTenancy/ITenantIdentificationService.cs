using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public interface ITenantIdentificationService<TTenant> 
        where TTenant : AppTenant

    {
        TTenant GetTenant(HttpContext httpContext, DbContextTenantsBase<TTenant> context);
    }
}
