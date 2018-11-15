using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace AspNetCore.ApiBase.MultiTenancy.Request.IdentificationStrategies
{
    public class SourceIPIdentificationService<TTenant> : ITenantIdentificationService<TTenant>
   where TTenant : AppTenant
    {
        public TTenant GetTenant(HttpContext httpContext, DbContextTenantsBase<TTenant> context)
        {
            //origin
            var ip = httpContext.Connection.RemoteIpAddress.ToString();
            return context.Tenants.FirstOrDefault(t => t.RequestIpAddresses != null && t.RequestIpAddresses.Any(i => ip.StartsWith(i)));
        }
    }
}
