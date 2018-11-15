﻿using AspNetCore.ApiBase.MultiTenancy.Data.Tenant.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.ApiBase.MultiTenancy.Data.Tenant.IdentificationStrategies
{
    public sealed class FilterTenantDbContext : IDbContextTenantStrategy
    {

        public void OnConfiguring(DbContextOptionsBuilder optionsBuilder, AppTenant tenant, string tenantPropertyName)
        {

        }

        public void OnModelCreating(ModelBuilder modelBuilder, DbContext context, AppTenant tenant, string tenantPropertyName)
        {
            modelBuilder.AddTenantFilter(tenant.Id, tenantPropertyName);
        }

        public void OnSaveChanges(DbContext context, AppTenant tenant, string tenantPropertyName)
        {

            context.SetTenantIds(tenant.Id, tenantPropertyName);
        }
    }
}
