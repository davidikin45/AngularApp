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
    }
}
