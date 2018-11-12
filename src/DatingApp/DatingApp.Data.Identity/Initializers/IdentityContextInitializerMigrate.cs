using AspnetCore.ApiBase.Data.Initializers;

namespace DatingApp.Data.Identity.Initializers
{
    public class IdentityContextInitializerMigrate : ContextInitializerMigrate<IdentityContext>
    {
        public IdentityContextInitializerMigrate(IdentityContext context)
            :base(context)
        {

        }

        public override void Seed(IdentityContext context)
        {
            context.Seed();
        }
    }
}
