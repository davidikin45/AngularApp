﻿using AspNetCore.ApiBase.Data.Helpers;
using AspNetCore.ApiBase.Data.Initializers;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AspnetCore.ApiBase.Data.Initializers
{
    public abstract class ContextInitializerDropCreate<TDbContext> : IDbContextInitializer<TDbContext>
        where TDbContext : DbContext
    {
        public async Task InitializeAsync(TDbContext context)
        {
            InitializeSchema(context);
            await InitializeDataAsync(context, null);
        }

        public void InitializeSchema(TDbContext context)
        {
            //Delete database relating to this context only
            context.EnsureTablesAndMigrationsDeleted();

            //Recreate databases with the current data model. This is useful for development as no migrations are applied.
            context.EnsureTablesCreated();
        }

        public async Task InitializeDataAsync(TDbContext context, string tenantId)
        {
            Seed(context, tenantId);

            await context.SaveChangesAsync();

            await OnSeedCompleteAsync(context);
        }

        public abstract void Seed(TDbContext context, string tenantId);
        public virtual Task OnSeedCompleteAsync(TDbContext context)
        {
            return Task.CompletedTask;
        }
    }
}
