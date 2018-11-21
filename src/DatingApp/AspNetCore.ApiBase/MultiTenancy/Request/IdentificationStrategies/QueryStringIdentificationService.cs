using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.ApiBase.MultiTenancy.Request.IdentificationStrategies
{
    public class QueryStringIdentificationService<TTenant> : ITenantIdentificationService<TTenant>
   where TTenant : AppTenant
    {
        public TTenant GetTenant(HttpContext httpContext, DbContextTenantsBase<TTenant> context)
        {
            if (httpContext == null)
            {
                return null;
            }

            var tenantId = httpContext.Request.Query["TenantId"].ToString();
            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                return context.Tenants.Find(tenantId);
            }
            return null;
        }
    }
}
