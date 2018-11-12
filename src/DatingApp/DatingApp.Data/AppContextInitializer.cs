using AspNetCore.ApiBase.Tasks;
using DatingApp.Data.Initializers;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace DatingApp.Data
{
    public class ApplicationContextInitializer : IAsyncInitializer
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AppContext _context;

        public ApplicationContextInitializer(AppContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task ExecuteAsync()
        {
            if (_hostingEnvironment.IsDevelopment())
            {
                var migrationInitializer = new AppContextInitializerDropCreate(_context);
                await migrationInitializer.InitializeAsync();
            }
            else
            {
                var migrationInitializer = new AppContextInitializerMigrate(_context);
                await migrationInitializer.InitializeAsync();
            }
        }
    }
}
