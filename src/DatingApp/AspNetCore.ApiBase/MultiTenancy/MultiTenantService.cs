using AspNetCore.ApiBase.Domain;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public class MultiTenantService<TContext, TTenant> : ITenantService<TTenant> 
        where TContext : DbContextTenantsBase<TTenant>
        where TTenant : AppTenant
    {
        private TTenant _tenant;

        public IDbContextTenantStrategy TenantStrategy { get; }
        public MultiTenantService(IHttpContextAccessor accessor, TContext context, ITenantIdentificationService<TTenant> service, IDbContextTenantStrategy strategy)
            :this(accessor, context, service)
        {
            TenantStrategy = strategy;
        }

        public MultiTenantService(IHttpContextAccessor accessor, TContext context, ITenantIdentificationService<TTenant> service)
        {
            _tenant = service.GetTenant(accessor.HttpContext, context);
        }

        private MultiTenantService(TTenant tenant, IDbContextTenantStrategy strategy)
        {
            _tenant = tenant;
            TenantStrategy = strategy;
        }
        
        public static MultiTenantService<TContext, TTenant> Create(TTenant tenant, IDbContextTenantStrategy strategy)
        {
            return new MultiTenantService<TContext, TTenant>(tenant, strategy);
        }

        public string TenantPropertyName => nameof(IEntityTenant.TenantId);

        public TTenant GetTenant()
        {
            return _tenant;
        }

        public string GetTenantId()
        {
            return _tenant?.Id;
        }

        AppTenant ITenantService.GetTenant()
        {
            return _tenant;
        }
    }
}
