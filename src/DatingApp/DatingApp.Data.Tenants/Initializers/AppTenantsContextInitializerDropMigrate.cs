using AspnetCore.ApiBase.Data.Initializers;
using AspnetCore.ApiBase.Data.Tenants.Initializers;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using AspNetCore.ApiBase.Settings;
using DatingApp.Data.Initializers;

namespace DatingApp.Data.Tenants.Initializers
{
    public class AppTenantsContextInitializerDropMigrate : TenantsContextInitializerDropMigrate<AppTenantsContext, AppTenant>
    {
        public AppTenantsContextInitializerDropMigrate(IDbContextTenantStrategy strategy, TenantSettings<AppTenant> settings)
            : base(strategy, settings)
        {
            AddContextInitializer<AppContext, AppContextInitializerDropMigrate>();
        }

        public override void Seed(AppTenantsContext context)
        {
            var dbSeeder = new DbSeed();
            dbSeeder.Seed(context, settings.SeedTenants);
        }
    }
}
