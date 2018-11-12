using AspNetCore.ApiBase.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AspnetCore.ApiBase.Data.Initializers
{
    public abstract class ContextInitializerDropMigrate<TDbContext>
         where TDbContext : DbContext
    {
        private readonly TDbContext _context;

        public ContextInitializerDropMigrate(
            TDbContext context)
        {
            _context = context;
        }

        public async Task InitializeAsync()
        {
            //Delete database relating to this context only
            _context.EnsureDeleted();

            var script = _context.GenerateMigrationScript();
            _context.Database.Migrate();

            Seed(_context);

            await _context.SaveChangesAsync();
        }

        public abstract void Seed(TDbContext context);
    }
}
