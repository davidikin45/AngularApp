using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Hangfire;
using AspNetCore.ApiBase.Tasks;
using Autofac;
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
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Tenant.Api
{
    public class AppTenantsInitializer : IAsyncInitializer
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
        private readonly ITenantConfiguration[] _tenantConfigurations;

        public AppTenantsInitializer(
            AppTenantsContext context,
            IConfiguration configuration,
            IHostingEnvironment environment,
            IApplicationLifetime applicationLifetime,
            IJobFilterProvider jobFilters,
            MultitenantContainer multiTenantContainer,
            IBackgroundJobFactory backgroundJobFactory,
            IBackgroundJobPerformer backgroundJobPerformer,
            IBackgroundJobStateChanger backgroundJobStateChanger,
            IBackgroundProcess[] additionalProcesses,
            ITenantConfiguration[] tenantConfigurations)
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
            _tenantConfigurations = tenantConfigurations;
        }

        public async Task ExecuteAsync()
        {
            foreach (var tenant in await _context.Tenants.ToListAsync())
            {
                var actionBuilder = new ConfigurationActionBuilder();

                var tenantInitializer = _tenantConfigurations.FirstOrDefault(i => i.TenantId == tenant.Id);

                if(tenantInitializer != null)
                {
                    tenantInitializer.ConfigureServices(actionBuilder, _configuration, _environment);
                }

                var connectionString = tenant.GetConnectionString("HangfireConnection") ?? (_configuration.GetSection("ConnectionStrings").GetChildren().Any(x => x.Key == "HangfireConnection") ? _configuration.GetConnectionString("HangfireConnection") : null);
                if(connectionString != null)
                {
                    var serverDetails = HangfireMultiTenantHelper.StartHangfireServer(tenant.Id, connectionString, _applicationLifetime, _jobFilters, _multiTenantContainer, _backgroundJobFactory, _backgroundJobPerformer, _backgroundJobStateChanger, _additionalProcesses);

                    if (tenantInitializer != null)
                    {
                        tenantInitializer.ConfigureHangfireJobs(serverDetails.recurringJobManager, _configuration, _environment);
                    }

                    actionBuilder.Add(b => b.RegisterInstance(serverDetails.recurringJobManager).As<IRecurringJobManager>().SingleInstance());
                    actionBuilder.Add(b => b.RegisterInstance(serverDetails.backgroundJobClient).As<IBackgroundJobClient>().SingleInstance());
                }
                else
                {
                    actionBuilder.Add(b => b.RegisterInstance<IRecurringJobManager>(null).As<IRecurringJobManager>().SingleInstance());
                    actionBuilder.Add(b => b.RegisterInstance<IBackgroundJobClient>(null).As<IBackgroundJobClient>().SingleInstance());
                }

                _multiTenantContainer.ConfigureTenant(tenant.Id, actionBuilder.Build());
            }
        }
    }
}
