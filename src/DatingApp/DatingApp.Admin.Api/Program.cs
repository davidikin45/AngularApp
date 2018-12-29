using AspNetCore.ApiBase;
using System.Threading.Tasks;

namespace DatingApp.Admin.Api
{
    public class Program : ProgramSingleTenantBase<Startup>
    {
        public async static Task<int> Main(string[] args)
        {
            return await RunApp(args);
        }
    }
}
