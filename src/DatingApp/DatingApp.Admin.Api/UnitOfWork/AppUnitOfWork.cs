
using AspNetCore.ApiBase.Data.UnitOfWork;
using AspNetCore.ApiBase.DomainEvents;
using AspNetCore.ApiBase.Validation;
using DatingApp.Admin.Core;
using DatingApp.Admin.Data.Identity;
using DatingApp.Data.Tenants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DatingApp.Api.UnitOfWork
{
    public class AppUnitOfWork : UnitOfWorkWithEventsBase, IAppUnitOfWork
    {
        public AppUnitOfWork(
            AppTenantsContext appContext,
            IdentityContext identityContext,
            IValidationService validationService,
            IDomainEventsDispatcher domainEventsDispatcher) 
            : base(true, validationService, domainEventsDispatcher, appContext, identityContext)
        {
 
        }

        public override void InitializeRepositories(Dictionary<Type, DbContext> contextsByEntityType)
        {
          
        }
    }
}
