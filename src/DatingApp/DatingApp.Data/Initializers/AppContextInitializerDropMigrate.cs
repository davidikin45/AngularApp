using AspnetCore.ApiBase.Data.Initializers;

namespace DatingApp.Data.Initializers
{
    public class AppContextInitializerDropMigrate : ContextInitializerDropMigrate<AppContext>
    {
        public AppContextInitializerDropMigrate(AppContext context)
            :base(context)
        {

        }

        public override void Seed(AppContext context)
        {
            context.Seed();
        }
    }
}
