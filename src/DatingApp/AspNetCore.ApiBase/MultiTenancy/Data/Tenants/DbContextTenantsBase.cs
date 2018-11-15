using AspNetCore.ApiBase.Data;
using AspNetCore.ApiBase.Data.Converters;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.ApiBase.MultiTenancy.Data.Tenants
{
    public abstract class DbContextTenantsBase<TTenant> : DbContextBase
        where TTenant : AppTenant
    {
        public DbSet<TTenant> Tenants { get; set; }

        public DbContextTenantsBase(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<TTenant>().Property(t => t.HostNames).HasConversion(new StringArrayToCsvValueConverter());
            builder.Entity<TTenant>().Property(t => t.RequestIpAddresses).HasConversion(new StringArrayToCsvValueConverter());
        }
    }
}
