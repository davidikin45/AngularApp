using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Data.Initializers
{
    public interface IDbContextInitializer<TDbContext>
        where TDbContext : DbContext
    {
        void Seed(TDbContext context);
        Task InitializeAsync(TDbContext context);
    }
}
