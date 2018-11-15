using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data.Tenants
{
    public class AppTenantsContext : DbContextTenantsBase<AppTenant>
    {
        public AppTenantsContext(DbContextOptions<AppTenantsContext> options)
            :base(options)
        {

        }

        public override void BuildQueries(ModelBuilder builder)
        {
           
        }

        public override void Seed()
        {
            
        }
    }
}
