using System.Threading.Tasks;
using AspNetCore.ApiBase;
using AspNetCore.ApiBase.Extensions;
using AspNetCore.ApiBase.Hangfire;
using AspNetCore.ApiBase.HostedServices;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.Tasks;
using DatingApp.Api.Jobs;
using DatingApp.Api.UnitOfWork;
using DatingApp.Core;
using DatingApp.Data;
using DatingApp.Data.Identity;
using DatingApp.Data.Tenants;
using DatingApp.Domain;
using Hangfire;
using Hangfire.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DatingApp.Api
{
    public class Startup : AppStartupApiIdentity<IdentityContext, User>
    {
        public Startup(ILoggerFactory loggerFactory, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
            : base(loggerFactory, configuration, hostingEnvironment)
        {

        }

        public override void AddDatabases(IServiceCollection services, string defaultConnectionString)
        {
            //services.AddDbContext<AppContext>(defaultConnectionString);
            services.AddDbContext<IdentityContext>(defaultConnectionString);

            services.AddDbContextMultiTenancy<AppTenantsContext, AppTenant>(defaultConnectionString, Configuration);

            services.AddDbContextTenant<AppContext>();
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
