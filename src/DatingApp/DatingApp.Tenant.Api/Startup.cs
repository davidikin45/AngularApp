using AspNetCore.ApiBase;
using AspNetCore.ApiBase.Extensions;
using AspNetCore.ApiBase.Hangfire;
using AspNetCore.ApiBase.HostedServices;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using AspNetCore.ApiBase.Tasks;
using DatingApp.Api.Jobs;
using DatingApp.Api.UnitOfWork;
using DatingApp.Data.Tenant.Identity;
using DatingApp.Data.Tenants;
using DatingApp.Tenant.Core;
using DatingApp.Tenant.Data;
using DatingApp.Tenant.Domain;
using Hangfire;
using Hangfire.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DatingApp.Tenant.Api
{
    public class Startup : AppStartupApiIdentity<IdentityContext, User>
    {
        public Startup(ILoggerFactory loggerFactory, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
            : base(loggerFactory, configuration, hostingEnvironment)
        {

        }

        public override void AddDatabases(IServiceCollection services, string tenantsConnectionString, string sharedIdentityConnectionString, string sharedHangfireConnectionString, string sharedDefaultConnectionString)
        {
            services.AddMultiTenancyDbContextStore<AppTenantsContext, AppTenant>(tenantsConnectionString);
            services.AddMultiTenancy<AppTenant>(Configuration);

            services.AddDbContextTenant<IdentityContext>(sharedIdentityConnectionString).AllowDifferentConnectionFilterByTenantAndDifferentSchemaForTenant("IdentityConnection");
            services.AddDbContextTenant<AppContext>(sharedDefaultConnectionString).AllowDifferentConnectionFilterByTenantAndDifferentSchemaForTenant("DefaultConnection");
        }

        public override void AddUnitOfWorks(IServiceCollection services)
        {
            services.AddUnitOfWork<IAppUnitOfWork, AppUnitOfWork>();
        }

        public override void ConfigureHttpClients(IServiceCollection services)
        {

        }

        public override void AddHostedServices(IServiceCollection services)
        {
            services.AddHostedServiceCronJob<Job2>("* * * * *");
        }

        public override void AddHangfireJobServices(IServiceCollection services)
        {
            services.AddHangfireJob<Job1>();
        }
    }

    public class HangfireScheduledJobs : IAsyncInitializer
    {
        private readonly IRecurringJobManager _recurringJobManager;
        public HangfireScheduledJobs(IRecurringJobManager recurringJobManager)
        {
            _recurringJobManager = recurringJobManager;
        }

        public Task ExecuteAsync()
        {
            _recurringJobManager.AddOrUpdate("check-link", Job.FromExpression<Job1>(m => m.Execute()), Cron.Minutely(), new RecurringJobOptions());
            _recurringJobManager.Trigger("check-link");

            return Task.CompletedTask;
        }
    }
}
