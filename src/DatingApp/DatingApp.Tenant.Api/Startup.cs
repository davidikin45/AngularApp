﻿using AspNetCore.ApiBase;
using AspNetCore.ApiBase.ApiClient;
using AspNetCore.ApiBase.Extensions;
using AspNetCore.ApiBase.Hangfire;
using AspNetCore.ApiBase.HostedServices;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.MultiTenancy.Data.Tenant;
using AspNetCore.ApiBase.Tasks;
using DatingApp.Api.Jobs;
using DatingApp.Api.UnitOfWork;
using DatingApp.Data.Tenant.Identity;
using DatingApp.Tenant.Api.ApiClient;
using DatingApp.Tenant.Core;
using DatingApp.Tenant.Data;
using DatingApp.Tenant.Domain;
using Hangfire;
using Hangfire.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
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
            //services.AddMultiTenancyDbContextStore<AppTenantsContext, AppTenant>(tenantsConnectionString);
            services.AddMultiTenancyStore<TenantsApiClient, AppTenant>();

            services.AddMultiTenancy<AppTenant>(Configuration);

            services.AddDbContextTenant<IdentityContext>(sharedIdentityConnectionString).AllowDifferentConnectionFilterByTenantAndDifferentSchemaForTenant("IdentityConnection");
            services.AddDbContextTenant<AppContext>(sharedDefaultConnectionString).AllowDifferentConnectionFilterByTenantAndDifferentSchemaForTenant("DefaultConnection");
        }

        public override void AddUnitOfWorks(IServiceCollection services)
        {
            services.AddUnitOfWork<IAppUnitOfWork, AppUnitOfWork>();
        }

        public override void AddHostedServices(IServiceCollection services)
        {
            services.AddHostedServiceCronJob<Job2>("* * * * *");
        }

        public override void AddHangfireJobServices(IServiceCollection services)
        {
            services.AddHangfireJob<Job1>();
        }

        public override void AddHttpClients(IServiceCollection services)
        {
            var apiClientSettings = GetSettings<ApiClientSettings>("ApiClientSettings");

            //Todo add polly cache policy
            services.AddHttpClient<TenantsApiClient>(client =>
            {
                client.Timeout = System.TimeSpan.FromSeconds(apiClientSettings.MaxTimeoutSeconds);
                client.BaseAddress = new System.Uri(apiClientSettings.BaseUrl);
            })
           .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
           {
               AllowAutoRedirect = true,
               AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
           })
           .AddPolicyHandler((sp, req) => {
               var cache = sp.GetService<IMemoryCache>();
               return PolicyHolder.GetRequestPolicy(cache, 1800);
           });

            services.AddHttpClient<AppApiClient>(client =>
            {
                client.Timeout = System.TimeSpan.FromSeconds(apiClientSettings.MaxTimeoutSeconds);
                client.BaseAddress = new System.Uri(apiClientSettings.BaseUrl);
            })
            .AddHttpMessageHandler<AuthorizationJwtProxyHttpHandler>()//2
            .AddHttpMessageHandler<AuthorizationBearerProxyHttpHandler>()//1
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            });
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
