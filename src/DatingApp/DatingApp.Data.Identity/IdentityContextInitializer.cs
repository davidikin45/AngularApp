using AspNetCore.ApiBase.Tasks;
using DatingApp.Data.Identity;
using DatingApp.Data.Identity.Initializers;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace DatingApp.Data
{
    public class IdentityContextInitializer : IAsyncInitializer
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IdentityContext _context;

        public IdentityContextInitializer(IdentityContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task ExecuteAsync()
        {
            if (_hostingEnvironment.IsDevelopment())
            {
                var migrationInitializer = new IdentityContextInitializerDropCreate(_context);
                await migrationInitializer.InitializeAsync();
            }
            else
            {
                var migrationInitializer = new IdentityContextInitializerMigrate(_context);
                await migrationInitializer.InitializeAsync();
            }
        }
    }
}
