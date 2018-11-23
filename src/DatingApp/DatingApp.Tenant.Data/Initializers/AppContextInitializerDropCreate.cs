using System.Threading.Tasks;
using AspnetCore.ApiBase.Data.Initializers;

namespace DatingApp.Tenant.Data.Initializers
{
    public class AppContextInitializerDropCreate : ContextInitializerDropCreate<AppContext>
    {
        public override void Seed(AppContext context, string tenantId)
        {
            context.Seed();
        }

        public override Task OnSeedCompleteAsync(AppContext context)
        {
            return Task.CompletedTask; 
        }
    }
}
