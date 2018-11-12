using AspNetCore.ApiBase.Extensions;
using AspNetCore.ApiBase.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using System;

namespace AspNetCore.ApiBase.Data
{
    public abstract class DbContextMultiTenancyBase<TTentant> : DbContext
        where TTentant : AppTenant
    {
        private readonly ITenantProvider<TTentant> _tenantProvider;

        public DbContextMultiTenancyBase(DbContextOptions options, ITenantProvider<TTentant> tenantProvider)
            :base(options)
        {
            _tenantProvider = tenantProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var tenant = _tenantProvider.GetTenant();
            if(tenant == null)
            {
                throw new Exception("Invalid Tenant");
            }

            var tenantConnectionString = tenant.ConnectionString;
            optionsBuilder.SetConnectionString(tenantConnectionString);
        }
    }
}
