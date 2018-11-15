using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.ApiBase.MultiTenancy.Request.IdentificationStrategies
{
    public class TenantHostQueryStringRequestIpIdentificationService<TTenant> : ITenantIdentificationService<TTenant>
        where TTenant : AppTenant
    {
        public TTenant GetTenant(HttpContext httpContext, DbContextTenantsBase<TTenant> context)
        {
            var hostIdentificationService = new HostIdentificationService<TTenant>();
            var queryStringIdentificationService = new QueryStringIdentificationService<TTenant>();
            var requestIpIdentificationService = new SourceIPIdentificationService<TTenant>();

            //destination
            var tenant = hostIdentificationService.GetTenant(httpContext, context);
            if (tenant != null)
            {
                return tenant;
            }

            tenant = queryStringIdentificationService.GetTenant(httpContext, context);
            if (tenant != null)
            {
                return tenant;
            }

            //origin
            tenant = requestIpIdentificationService.GetTenant(httpContext, context);

            return tenant;
        }
    }
}
