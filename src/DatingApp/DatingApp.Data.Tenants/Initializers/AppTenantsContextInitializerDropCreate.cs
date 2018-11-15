using AspnetCore.ApiBase.Data.Initializers;
using AspnetCore.ApiBase.Data.Tenants.Initializers;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using AspNetCore.ApiBase.Settings;
using DatingApp.Data.Initializers;

namespace DatingApp.Data.Tenants.Initializers
{
    public class AppTenantsContextInitializerDropCreate: TenantsContextInitializerDropCreate<AppTenantsContext, AppTenant>
    {
        public AppTenantsContextInitializerDropCreate(IDbContextTenantStrategy strategy, TenantSettings<AppTenant> settings)
            : base(strategy, settings)
        {
            AddContextInitializer<AppContext, AppContextInitializerDropCreate>();
        }

        public override void Seed(AppTenantsContext context)
        {
            var dbSeeder = new DbSeed();
            dbSeeder.Seed(context, settings.SeedTenants);
        }
    }
}
