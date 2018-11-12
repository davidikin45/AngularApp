using AspnetCore.ApiBase.Data.Initializers;

namespace DatingApp.Data.Initializers
{
    public class AppContextInitializerMigrate : ContextInitializerMigrate<AppContext>
    {
        public AppContextInitializerMigrate(AppContext context)
            :base(context)
        {

        }

        public override void Seed(AppContext context)
        {
            context.Seed();
        }
    }
}
