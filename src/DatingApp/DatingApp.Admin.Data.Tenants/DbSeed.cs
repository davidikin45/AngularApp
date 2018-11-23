using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using System.Collections.Generic;

namespace DatingApp.Data.Tenants
{
    public class DbSeed : DbSeedTenants<AppTenant>
    {
        public override IEnumerable<AppTenant> InMemorySeedTenants => new List<AppTenant>()
        {

        };
    }
}
