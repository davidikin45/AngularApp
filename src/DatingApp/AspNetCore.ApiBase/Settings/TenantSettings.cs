using AspNetCore.ApiBase.MultiTenancy;

namespace AspNetCore.ApiBase.Settings
{
    public class TenantSettings<TTenant>
        where TTenant: AppTenant
    {
        public TTenant[] SeedTenants { get; set; }
    }
}
