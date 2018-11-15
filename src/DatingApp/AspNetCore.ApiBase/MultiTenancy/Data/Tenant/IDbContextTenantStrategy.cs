using Microsoft.EntityFrameworkCore;

namespace AspNetCore.ApiBase.MultiTenancy.Data.Tenant
{
    public interface IDbContextTenantStrategy
    {
        void OnConfiguring(DbContextOptionsBuilder optionsBuilder, AppTenant appTenant, string tenantPropertyName);
        void OnModelCreating(ModelBuilder modelBuilder, DbContext context, AppTenant appTenant, string tenantPropertyName);
        void OnSaveChanges(DbContext tenantDbContext, AppTenant appTenant, string tenantPropertyName);
    }
}
