using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using AspNetCore.ApiBase.Settings;
using AspNetCore.ApiBase.Tasks;
using DatingApp.Data.Tenants.Initializers;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace DatingApp.Data.Tenants
{
    public class AppTenantsContextInitializer : IAsyncInitializer
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AppTenantsContext _context;
        private readonly IDbContextTenantStrategy _strategy;
        private readonly TenantSettings<AppTenant> _settings;

        public AppTenantsContextInitializer(AppTenantsContext context, IHostingEnvironment hostingEnvironment, IDbContextTenantStrategy strategy, TenantSettings<AppTenant> settings)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _strategy = strategy;
            _settings = settings;
        }

        public async Task ExecuteAsync()
        {
            if (_hostingEnvironment.IsDevelopment())
            {
                var migrationInitializer = new AppTenantsContextInitializerDropCreate(_strategy, _settings);
                await migrationInitializer.InitializeAsync(_context);
            }
            else
            {
                var migrationInitializer = new AppTenantsContextInitializerMigrate(_strategy, _settings);
                await migrationInitializer.InitializeAsync(_context);
            }
        }
    }
}
