using AspNetCore.ApiBase.Hangfire;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Hangfire;
using AspNetCore.ApiBase.Tasks;
using Autofac.Multitenant;
using DatingApp.Data.Tenants;
using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DatingApp.Tenant.Api
{
    public class AppTenantsHangireInitializer : IAsyncInitializer
    {
        private readonly AppTenantsContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;

        private readonly IApplicationLifetime _applicationLifetime;
        private readonly IJobFilterProvider _jobFilters;
        private readonly MultitenantContainer _multiTenantContainer;
        private readonly IBackgroundJobFactory _backgroundJobFactory;
        private readonly IBackgroundJobPerformer _backgroundJobPerformer;
        private readonly IBackgroundJobStateChanger _backgroundJobStateChanger;
        private readonly IBackgroundProcess[] _additionalProcesses;

        public AppTenantsHangireInitializer(
            AppTenantsContext context,
            IConfiguration configuration,
            IHostingEnvironment environment,
            IApplicationLifetime applicationLifetime,
            IJobFilterProvider jobFilters,
            MultitenantContainer multiTenantContainer,
            IBackgroundJobFactory backgroundJobFactory,
            IBackgroundJobPerformer backgroundJobPerformer,
            IBackgroundJobStateChanger backgroundJobStateChanger,
            IBackgroundProcess[] additionalProcesses)
        {
            _context = context;
            _configuration = configuration;
            _environment = environment;
            _applicationLifetime = applicationLifetime;
            _jobFilters = jobFilters;
            _multiTenantContainer = multiTenantContainer;
            _backgroundJobFactory = backgroundJobFactory;
            _backgroundJobPerformer = backgroundJobPerformer;
            _backgroundJobStateChanger = backgroundJobStateChanger;
            _additionalProcesses = additionalProcesses;
        }

        public async Task ExecuteAsync()
        {
            var types = Assembly.GetEntryAssembly()
               .GetExportedTypes()
               .Where(type => typeof(ITenantConfiguration).IsAssignableFrom(type))
               .Where(type => (type.IsAbstract == false) && (type.IsInterface == false));

            foreach (var tenant in await _context.Tenants.ToListAsync())
            {
                var connectionString = tenant.GetConnectionString("HangfireConnection") ?? (_configuration.GetSection("ConnectionStrings").GetChildren().Any(x => x.Key == "HangfireConnection") ? _configuration.GetConnectionString("HangfireConnection") : null);
                if(connectionString != null)
                {
                    var recurringJobManager = HangfireMultiTenantHelper.StartHangfireServer(connectionString, tenant.Id, _applicationLifetime, _jobFilters, _multiTenantContainer, _backgroundJobFactory, _backgroundJobPerformer, _backgroundJobStateChanger, _additionalProcesses);

                    var instance = types
                     .Select(type => Activator.CreateInstance(type))
                     .OfType<ITenantConfiguration>()
                     .SingleOrDefault(x => x.TenantId == tenant.Id);

                    if (instance != null)
                    {
                        instance.ConfigureHangfireJobs(recurringJobManager, _configuration, _environment);
                    }
                }
            }
        }
    }
}
