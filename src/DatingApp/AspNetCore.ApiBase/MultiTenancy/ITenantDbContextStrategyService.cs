using Microsoft.EntityFrameworkCore;
using System;

namespace AspNetCore.ApiBase.MultiTenancy.Data.Tenant
{
    public interface ITenantDbContextStrategyService
    {
        IDbContextTenantStrategy GetStrategy(DbContext context);
    }
}
