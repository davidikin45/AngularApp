using AspNetCore.ApiBase.Domain;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public class MultiTenantService<TContext, TTenant> : ITenantService<TTenant> 
        where TContext : DbContextTenantsBase<TTenant>
        where TTenant : AppTenant
    {
        private TTenant _tenant;

        private readonly ITenantDbContextStrategyService _strategyService;
        public MultiTenantService(IHttpContextAccessor accessor, TContext context, ITenantIdentificationService<TTenant> service, ITenantDbContextStrategyService strategyService)
            :this(accessor, context, service)
        {
            _strategyService = strategyService;
        }

        public MultiTenantService(IHttpContextAccessor accessor, TContext context, ITenantIdentificationService<TTenant> service)
        {
            _tenant = service.GetTenant(accessor.HttpContext, context);
        }

        public string TenantPropertyName => nameof(IEntityTenantFilter.TenantId);

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

        public void SetTenant(TTenant tenant)
        {
            _tenant = tenant;
        }

        public IDbContextTenantStrategy GetTenantStrategy(DbContext context)
        {
            return _strategyService.GetStrategy(context);
        }
    }
}
