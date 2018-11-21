using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace AspNetCore.ApiBase.MultiTenancy.Request.IdentificationStrategies
{
    public class HostIdentificationService<TTenant> : ITenantIdentificationService<TTenant>
   where TTenant : AppTenant
    {
        public TTenant GetTenant(HttpContext httpContext, DbContextTenantsBase<TTenant> context)
        {
            if (httpContext == null)
            {
                return null;
            }

            //destination
            var host = httpContext.Request.Host.Value.Replace("www.","");
            var hostWithoutPort = host.Split(":")[0];

            var tenants = context.Tenants.Where(t => t.HostNames.Contains(host) || t.HostNames.Contains(hostWithoutPort)).ToList();
            if(tenants.Count == 1)
            {
                return tenants.First();
            }

            return null;
        }
    }
}
