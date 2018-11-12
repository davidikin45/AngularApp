using AspnetCore.ApiBase.Data.Initializers;

namespace DatingApp.Data.Initializers
{
    public class AppContextInitializerDropCreate : ContextInitializerDropCreate<AppContext>
    {
        public AppContextInitializerDropCreate(AppContext context)
            :base(context)
        {

        }

        public override void Seed(AppContext context)
        {
            context.Seed();
        }
    }
}
