using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using Microsoft.EntityFrameworkCore;
using System;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public interface ITenantService<TTenant> : ITenantService
        where TTenant : AppTenant
    {
        new TTenant GetTenant();
        void SetTenant(TTenant tenant);
    }

    public interface ITenantService
    {
        IDbContextTenantStrategy GetTenantStrategy(DbContext context);
        AppTenant GetTenant();
        string GetTenantId();
        string TenantPropertyName { get; }
    }
}
