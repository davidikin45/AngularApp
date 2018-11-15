using System.Threading.Tasks;
using AspnetCore.ApiBase.Data.Initializers;

namespace DatingApp.Data.Initializers
{
    public class AppContextInitializerDropCreate : ContextInitializerDropCreate<AppContext>
    {
        public override void Seed(AppContext context)
        {
            context.Seed();
        }

        public override Task OnSeedCompleteAsync(AppContext context)
        {
            return Task.CompletedTask; 
        }
    }
}
