using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data
{
    public class AppContext : DbContextTenantBase
    {
        public DbSet<Value> Values { get; set; }

        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {

        }

        public AppContext(ITenantService tenantService) : base(tenantService)
        {

        }

        public override void BuildQueries(ModelBuilder builder)
        {

        }

        public override void Seed()
        {
            DbSeed.Seed(this);
        }
    }
}
