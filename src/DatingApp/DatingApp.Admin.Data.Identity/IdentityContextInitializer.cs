using AspNetCore.ApiBase.Extensions;
using AspNetCore.ApiBase.Tasks;
using DatingApp.Admin.Data.Identity.Initializers;
using DatingApp.Admin.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DatingApp.Admin.Data.Identity
{
    public class IdentityContextInitializer : IAsyncInitializer
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IdentityContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public IdentityContextInitializer(IdentityContext context, IPasswordHasher<User> passwordHasher, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task ExecuteAsync()
        {
            if (_hostingEnvironment.IsStaging() || _hostingEnvironment.IsProduction())
            {
                var migrationInitializer = new IdentityContextInitializerMigrate(_passwordHasher);
                await migrationInitializer.InitializeAsync(_context);
            }
            else if (_hostingEnvironment.IsIntegration())
            {
                var migrationInitializer = new IdentityContextInitializerDropMigrate(_passwordHasher);
                await migrationInitializer.InitializeAsync(_context);
            }
            else
            {
                var migrationInitializer = new IdentityContextInitializerDropCreate(_passwordHasher);
                await migrationInitializer.InitializeAsync(_context);
            }
        }
    }
}
