﻿using AspnetCore.ApiBase.Data.Initializers;
using System.Threading.Tasks;

namespace DatingApp.Tenant.Data.Initializers
{
    public class AppContextInitializerMigrate : ContextInitializerMigrate<AppContext>
    {
        public override void Seed(AppContext context, string tenantId)
        {
            context.Seed();
        }
    }
}
