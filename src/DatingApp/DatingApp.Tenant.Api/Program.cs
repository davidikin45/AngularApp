using AspNetCore.ApiBase;
using AspNetCore.ApiBase.MultiTenancy;
using DatingApp.Data.Tenants;
using System.Threading.Tasks;

namespace DatingApp.Tenant.Api
{
    public class Program : ProgramMultiTenantBase<Startup, AppTenantsContext, AppTenant>
    {
        public async static Task<int> Main(string[] args)
        {
            return await RunApp(args);
        }
    }
}
