using AspNetCore.ApiBase.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.ApiBase.MultiTenancy.Data.Tenant.IdentificationStrategies
{
    public sealed class DifferentConnectionTenantDbContext<TTenant> : IDbContextTenantStrategy
        where TTenant : AppTenant
    {
        public void OnModelCreating(ModelBuilder modelBuilder, DbContext context, AppTenant tenant, string tenantPropertyName)
        {
          
        }

        public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, AppTenant tenant, string tenantPropertyName)
        {
            optionsBuilder.SetConnectionString(tenant.ConnectionString);
        }

        public void OnSaveChanges(DbContext context, AppTenant tenant, string tenantPropertyName)
        {

        }
    }
}
