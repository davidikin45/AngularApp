using AspnetCore.ApiBase.Data.Initializers;
using System.Threading.Tasks;

namespace DatingApp.Data.Initializers
{
    public class AppContextInitializerDropMigrate : ContextInitializerDropMigrate<AppContext>
    {
        public override void Seed(AppContext context)
        {
            context.Seed();
        }
    }
}
