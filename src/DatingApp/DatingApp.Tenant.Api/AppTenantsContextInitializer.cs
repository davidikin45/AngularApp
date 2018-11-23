using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.Settings;
using AspNetCore.ApiBase.Tasks;
using AutoMapper.Configuration;
using DatingApp.Data.Tenants;
using DatingApp.Data.Tenants.Initializers;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;

namespace DatingApp.Api
{
    public class AppTenantsContextInitializer : IAsyncInitializer
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AppTenantsContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly TenantSettings<AppTenant> _settings;

        public AppTenantsContextInitializer(AppTenantsContext context, IHostingEnvironment hostingEnvironment, IServiceProvider serviceProvider, TenantSettings<AppTenant> settings)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _serviceProvider = serviceProvider;
            _settings = settings;
        }

        public async Task ExecuteAsync()
        {
            if (_hostingEnvironment.IsDevelopment())
            {
                var migrationInitializer = new AppTenantsContextInitializerDropCreate(_serviceProvider, _settings);
                await migrationInitializer.InitializeAsync(_context);
            }
            else
            {
                var migrationInitializer = new AppTenantsContextInitializerMigrate(_serviceProvider, _settings);
                await migrationInitializer.InitializeAsync(_context);
            }
        }
    }
}