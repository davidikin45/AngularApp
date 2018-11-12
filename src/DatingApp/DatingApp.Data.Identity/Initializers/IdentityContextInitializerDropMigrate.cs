using AspnetCore.ApiBase.Data.Initializers;

namespace DatingApp.Data.Identity.Initializers
{
    public class IdentityContextInitializerDropMigrate : ContextInitializerDropMigrate<IdentityContext>
    {
        public IdentityContextInitializerDropMigrate(IdentityContext context)
            :base(context)
        {

        }

        public override void Seed(IdentityContext context)
        {
            context.Seed();
        }
    }
}
