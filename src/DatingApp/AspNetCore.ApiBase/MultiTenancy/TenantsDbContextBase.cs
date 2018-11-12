using AspNetCore.ApiBase.Data;
using AspNetCore.ApiBase.Data.Converters;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public abstract class TenantsDbContextBase<TTenant> : DbContextBase
        where TTenant : AppTenant
    {
        public DbSet<TTenant> Tenants { get; set; }

        public TenantsDbContextBase(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TTenant>().Property(e => e.HostNames).HasConversion(new ArrayToCsvValueConverter());
            builder.Entity<TTenant>().HasData(
               GetTenants()
            );
        }

        public abstract IEnumerable<TTenant> GetTenants();

        public override void BuildQueries(ModelBuilder builder)
        {
            
        }

        public override void Seed()
        {
           
        }
    }
}
