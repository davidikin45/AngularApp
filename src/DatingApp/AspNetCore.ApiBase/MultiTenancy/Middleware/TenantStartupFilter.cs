﻿using AspNetCore.ApiBase.MultiTenancy.Data.Tenants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace AspNetCore.ApiBase.MultiTenancy.Middleware
{
    public class TenantStartupFilter<TTenant> : IStartupFilter
        where TTenant: AppTenant
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder => next(builder.UseMiddleware<TenantMiddleware<TTenant>>());
        }
    }
}
