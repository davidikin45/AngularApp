using AspNetCore.ApiBase.Hangfire;
using Autofac.Multitenant;
using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.AspNetCore.Hosting;

namespace AspNetCore.ApiBase.MultiTenancy.Hangfire
{
    public static class HangfireMultiTenantHelper
    {
        public static IRecurringJobManager StartHangfireServer(
            string connectionString,
            string tenantId,
            IApplicationLifetime applicationLifetime,
            IJobFilterProvider jobFilters,
            MultitenantContainer mtc,
            IBackgroundJobFactory backgroundJobFactory,
            IBackgroundJobPerformer backgroundJobPerformer,
            IBackgroundJobStateChanger backgroundJobStateChanger,
            IBackgroundProcess[] additionalProcesses
            )
        {
            var tenantJobActivator = new AspNetCoreMultiTenantJobActivator(mtc, tenantId);

            return HangfireHelper.StartHangfireServer(
                connectionString,
                tenantId,
                applicationLifetime,
                jobFilters,
                tenantJobActivator,
                backgroundJobFactory,
                backgroundJobPerformer,
                backgroundJobStateChanger,
                additionalProcesses);
        }
    }
}
