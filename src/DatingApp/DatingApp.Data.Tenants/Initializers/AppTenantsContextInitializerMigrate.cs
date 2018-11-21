using AspnetCore.ApiBase.Data.Tenants.Initializers;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using AspNetCore.ApiBase.Settings;
using DatingApp.Data.Identity;
using DatingApp.Data.Identity.Initializers;
using DatingApp.Data.Initializers;
using System;

namespace DatingApp.Data.Tenants.Initializers
{
    public class AppTenantsContextInitializerMigrate : TenantsContextInitializerMigrate<AppTenantsContext, AppTenant>
    {
        public AppTenantsContextInitializerMigrate(IServiceProvider serviceProvider, TenantSettings<AppTenant> settings)
            :base(serviceProvider, settings)
        {
            AddContextInitializer<AppContext, AppContextInitializerMigrate>();
            AddContextInitializer<IdentityContext, IdentityContextInitializerMigrate>();
        }

        public override void Seed(AppTenantsContext context, string tenantId)
        {
            var dbSeeder = new DbSeed();
            dbSeeder.Seed(context, settings.SeedTenants);
        }
    }
}
