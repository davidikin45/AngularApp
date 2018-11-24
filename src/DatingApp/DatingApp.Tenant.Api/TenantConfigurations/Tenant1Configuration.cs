using AspNetCore.ApiBase.MultiTenancy;
using Autofac.Multitenant;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DatingApp.Api.TenantConfigurations
{
    public class Tenant1Configuration : ITenantConfiguration
    {
        public string TenantId => "t1";

        public void ConfigureServices(ConfigurationActionBuilder services, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {

        }

        public void ConfigureHangfireJobs(IRecurringJobManager recurringJobManager, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {

        }
    }
}
