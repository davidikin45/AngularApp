using AspNetCore.ApiBase.Hangfire;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.Tasks;
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
        private readonly JobActivator _jobActivator;
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
            JobActivator jobActivator,
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
            _jobActivator = jobActivator;
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
                var connectionString = tenant.GetConnectionString("DefaultConnection");
                var recurringJobManager = HangfireHelper.StartHangfireServer(connectionString, tenant.Id, _applicationLifetime, _jobFilters, _jobActivator, _backgroundJobFactory, _backgroundJobPerformer, _backgroundJobStateChanger, _additionalProcesses);

                var instance = types
                 .Select(type => Activator.CreateInstance(type))
                 .OfType<ITenantConfiguration>()
                 .SingleOrDefault(x => x.TenantId == tenant.Id);

                if(instance != null)
                {
                    instance.ConfigureHangfireJobs(recurringJobManager, _configuration, _environment);
                }
            }
        }
    }
}
