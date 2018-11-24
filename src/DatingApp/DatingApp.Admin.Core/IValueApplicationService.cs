using AspNetCore.ApiBase.ApplicationServices;
using AspNetCore.ApiBase.MultiTenancy;

namespace DatingApp.Admin.Core
{
    public interface ITenantApplicationService : IApplicationServiceEntity<AppTenant, AppTenant, AppTenant, AppTenant>
    {

    }
}
