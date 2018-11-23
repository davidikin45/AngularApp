using AspNetCore.ApiBase.Hangfire;
using AspNetCore.ApiBase.Helpers;
using AspNetCore.ApiBase.MultiTenancy.Hangfire;
using Hangfire;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.SQLite;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace AspNetCore.ApiBase.MultiTenancy
{
    public static class HangfireMultiTenantExtensions
    {
        public static IApplicationBuilder UseHangfireDashboardMultiTenant(
            [NotNull] this IApplicationBuilder app,
            [NotNull] string route = "/admin/hangfire")
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (route == null) throw new ArgumentNullException(nameof(route));

            app.Map(new PathString(route), x => x.UseMiddleware<AspNetCoreMultiTenantDashboardMiddleware>(route));

            return app;
        }
    }
}
