using AspNetCore.ApiBase.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AspnetCore.ApiBase.Data.Initializers
{
    public abstract class ContextInitializerMigrate<TDbContext>
        where TDbContext : DbContext
    {
        private readonly TDbContext _context;

        public ContextInitializerMigrate(
            TDbContext context)
        {
            _context = context;
        }

        public async Task InitializeAsync()
        {
            var script = _context.GenerateMigrationScript();
            _context.Database.Migrate();

            Seed(_context);

           await _context.SaveChangesAsync();
        }

        public abstract void Seed(TDbContext context);
    }
}
