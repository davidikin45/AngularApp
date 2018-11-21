﻿using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data
{
    public class AppContext : DbContextTenantBase
    {
        public DbSet<Value> Values { get; set; }

        public AppContext(DbContextOptions<AppContext> options = null, ITenantService tenantService = null) : base(options, tenantService)
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
