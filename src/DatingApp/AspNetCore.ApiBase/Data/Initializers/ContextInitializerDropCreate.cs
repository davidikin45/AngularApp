using AspNetCore.ApiBase.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AspnetCore.ApiBase.Data.Initializers
{
    public abstract class ContextInitializerDropCreate<TDbContext>
        where TDbContext : DbContext
    {
        private readonly TDbContext _context;

        public ContextInitializerDropCreate(
            TDbContext context)
        {
            _context = context;
        }

        public async Task InitializeAsync()
        {
            //Delete database relating to this context only
            _context.EnsureDeleted();

            //Recreate databases with the current data model. This is useful for development as no migrations are applied.
            _context.EnsureCreated();

            Seed(_context);

           await _context.SaveChangesAsync();
        }

        public abstract void Seed(TDbContext context);
    }
}
