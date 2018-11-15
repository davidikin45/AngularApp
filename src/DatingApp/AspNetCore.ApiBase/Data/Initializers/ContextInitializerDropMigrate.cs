using AspNetCore.ApiBase.Data.Helpers;
using AspNetCore.ApiBase.Data.Initializers;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AspnetCore.ApiBase.Data.Initializers
{
    public abstract class ContextInitializerDropMigrate<TDbContext> : IDbContextInitializer<TDbContext>
         where TDbContext : DbContext
    {
        public async Task InitializeAsync(TDbContext context)
        {
            context.EnsureDeleted();

            var script = context.GenerateMigrationScript();
            context.Database.Migrate();

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
