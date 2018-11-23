using AspnetCore.ApiBase.Data.Tenants.Initializers;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.Settings;
using DatingApp.Data.Tenant.Identity;
using DatingApp.Data.Tenant.Identity.Initializers;
using DatingApp.Tenant.Data;
using DatingApp.Tenant.Data.Initializers;

namespace DatingApp.Data.Tenants.Initializers
{
    public class AppTenantsContextInitializerDropMigrate : TenantsContextInitializerDropMigrate<AppTenantsContext, AppTenant>
    {
        public AppTenantsContextInitializerDropMigrate(System.IServiceProvider serviceProvider, TenantSettings<AppTenant> settings)
            : base(serviceProvider, settings)
        {
            AddContextInitializer<AppContext, AppContextInitializerDropMigrate>();
            AddContextInitializer<IdentityContext, IdentityContextInitializerDropMigrate>();
        }

        public override void Seed(AppTenantsContext context, string tenantId)
        {
            var dbSeeder = new DbSeed();
            dbSeeder.Seed(context, settings.SeedTenants);
        }
    }
}
