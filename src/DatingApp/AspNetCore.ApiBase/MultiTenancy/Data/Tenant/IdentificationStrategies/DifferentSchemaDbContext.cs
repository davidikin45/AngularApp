using AspNetCore.ApiBase.MultiTenancy.Data.Tenant.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.ApiBase.MultiTenancy.Data.Tenant.IdentificationStrategies
{
    public class DifferentSchemaTenantDbContext : IDbContextTenantStrategy
    {


        public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, AppTenant tenant, string tenantPropertyName)
        {
         
        }

        public void OnModelCreating(ModelBuilder modelBuilder, DbContext context, AppTenant tenant, string tenantPropertyName)
        {
            modelBuilder.AddTenantSchema(tenant.Id, tenantPropertyName);
        }

        public void OnSaveChanges(DbContext context, AppTenant tenant, string tenantPropertyName)
        {

        }
    }
}
