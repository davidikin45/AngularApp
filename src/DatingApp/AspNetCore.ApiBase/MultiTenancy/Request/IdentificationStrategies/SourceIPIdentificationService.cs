using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace AspNetCore.ApiBase.MultiTenancy.Request.IdentificationStrategies
{
    public class SourceIPIdentificationService<TContext,TTenant> : ITenantIdentificationService<TContext,TTenant>
   where TContext : DbContextTenantsBase<TTenant>
   where TTenant : AppTenant
    {
        private readonly ILogger<ITenantIdentificationService<TContext, TTenant>> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TContext _context;

        public SourceIPIdentificationService(TContext context, IHttpContextAccessor contextAccessor, ILogger<ITenantIdentificationService<TContext, TTenant>> logger)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public TTenant GetTenant(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                return null;
            }

            //origin
            var ip = httpContext.Connection.RemoteIpAddress.ToString();
            var tenant = _context.Tenants.FirstOrDefault(t => t.RequestIpAddresses != null && t.RequestIpAddresses.Any(i => ip.StartsWith(i)));
            if (tenant != null)
            {
                httpContext.Items["_tenant"] = tenant;
                httpContext.Items["_tenantId"] = tenant.Id;
                _logger.LogInformation("Identified tenant from ip address: {tenant}", tenant.Id);
                return tenant;
            }

            httpContext.Items["_tenant"] = null;
            httpContext.Items["_tenantId"] = null;
            _logger.LogWarning("Unable to identify tenant from ip address.");
            return null;
        }

        public bool TryIdentifyTenant(out object tenantId)
        {
            var httpContext = _contextAccessor.HttpContext;
            if (httpContext == null)
            {
                // No current HttpContext. This happens during app startup
                // and isn't really an error, but is something to be aware of.
                tenantId = null;
                return false;
            }

            // Caching the value both speeds up tenant identification for
            // later and ensures we only see one log message indicating
            // relative success or failure for tenant ID.
            if (httpContext.Items.TryGetValue("_tenantId", out tenantId))
            {
                // We've already identified the tenant at some point
                // so just return the cached value (even if the cached value
                // indicates we couldn't identify the tenant for this context).
                return tenantId != null;
            }

            var tenant = GetTenant(httpContext);
            if (tenant != null)
            {
                tenantId = tenant.Id;
                return true;
            }

            tenantId = null;
            return false;
        }
    }
}
