using AspnetCore.ApiBase.Data.Initializers;
using AspNetCore.ApiBase.Data.Initializers;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
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

        private readonly IDbContextTenantStrategy _strategy;
        protected readonly TenantSettings<TTenant> settings;
        public TenantsContextInitializerDropMigrate(IDbContextTenantStrategy strategy, TenantSettings<TTenant> settings)
        {
            _strategy = strategy;
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
            foreach (var contextInitializer in _contextInitializers)
            {
                var migrator = Activator.CreateInstance(contextInitializer.Value);
                foreach (var tenant in context.Tenants)
                {
                    var multiTenantService = MultiTenantService<TDbContextTenants, TTenant>.Create(tenant, _strategy);
                    using (var dbContext = (IDisposable)Activator.CreateInstance(contextInitializer.Key, multiTenantService))
                    {
                        var genericType = typeof(IDbContextInitializer<>).MakeGenericType(contextInitializer.Key);
                        var result = (Task)genericType.GetMethod(nameof(IDbContextInitializer<DbContext>.InitializeAsync)).Invoke(migrator, new object[] { dbContext });
                        await result;
                    }
                }
            }
        }
    }
}
