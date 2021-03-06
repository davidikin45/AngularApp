﻿using AspnetCore.ApiBase.Data.Initializers;
using AspNetCore.ApiBase.Data.Initializers;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenants.Initializers;
using AspNetCore.ApiBase.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetCore.ApiBase.Data.Tenants.Initializers
{
    public abstract class TenantsContextInitializerDropMigrate<TDbContextTenants, TTenant> : ContextInitializerDropMigrate<TDbContextTenants>
        where TDbContextTenants : DbContextTenantsBase<TTenant>
        where TTenant : AppTenant
    {
        private Dictionary<Type, Type> _contextInitializers = new Dictionary<Type, Type>();

        private readonly IServiceProvider _serviceProvider;
        protected readonly TenantSettings<TTenant> settings;
        public TenantsContextInitializerDropMigrate(IServiceProvider serviceProvider, TenantSettings<TTenant> settings)
        {
            _serviceProvider = serviceProvider;
            this.settings = settings;
        }

        public void AddContextInitializer<TDbContext, TInitializer>()
             where TDbContext : DbContext
             where TInitializer : IDbContextInitializer<TDbContext>
        {
            _contextInitializers.Add(typeof(TDbContext), typeof(TDbContext));
        }

        public async override Task OnSeedCompleteAsync(TDbContextTenants context)
        {
            await TenantContenInitializerHelper.InitializeContextsAsync<TDbContextTenants, TTenant>(context, _contextInitializers, _serviceProvider);
        }
    }
}
