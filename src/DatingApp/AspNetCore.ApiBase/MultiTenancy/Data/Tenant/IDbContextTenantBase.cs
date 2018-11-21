namespace AspNetCore.ApiBase.MultiTenancy.Data.Tenant
{
    interface IDbContextTenantBase
    {
         ITenantService TenantService { get; }
    }
}
