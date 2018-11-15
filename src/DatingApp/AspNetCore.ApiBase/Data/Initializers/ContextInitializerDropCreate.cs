using AspNetCore.ApiBase.Data.Helpers;
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
            //Delete database relating to this context only
            context.EnsureDeleted();

            //Recreate databases with the current data model. This is useful for development as no migrations are applied.
            context.EnsureCreated();

            Seed(context);

            await context.SaveChangesAsync();

            await OnSeedCompleteAsync(context);
        }

        public abstract void Seed(TDbContext context);
        public virtual Task OnSeedCompleteAsync(TDbContext context)
        {
            return Task.CompletedTask;
        }
    }
}
