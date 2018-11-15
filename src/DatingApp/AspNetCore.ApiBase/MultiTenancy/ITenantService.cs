using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public interface ITenantService<TTentant> : ITenantService
        where TTentant : AppTenant
    {
        new TTentant GetTenant();
    }

    public interface ITenantService
    {
        IDbContextTenantStrategy TenantStrategy { get; }
        AppTenant GetTenant();
        string GetTenantId();
        string TenantPropertyName { get; }
    }
}
