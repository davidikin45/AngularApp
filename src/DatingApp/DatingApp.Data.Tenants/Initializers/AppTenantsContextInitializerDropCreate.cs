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
    public class AppTenantsContextInitializerDropCreate: TenantsContextInitializerDropCreate<AppTenantsContext, AppTenant>
    {
        public AppTenantsContextInitializerDropCreate(IServiceProvider serviceProvider, TenantSettings<AppTenant> settings)
            : base(serviceProvider, settings)
        {
            AddContextInitializer<AppContext, AppContextInitializerDropCreate>();
            AddContextInitializer<IdentityContext, IdentityContextInitializerDropCreate>();
        }

        public override void Seed(AppTenantsContext context, string tenantId)
        {
            var dbSeeder = new DbSeed();
            dbSeeder.Seed(context, settings.SeedTenants);
        }
    }
}
