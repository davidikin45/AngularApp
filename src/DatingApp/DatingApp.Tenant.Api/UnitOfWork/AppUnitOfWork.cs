
using AspNetCore.ApiBase.Data.UnitOfWork;
using AspNetCore.ApiBase.DomainEvents;
using AspNetCore.ApiBase.Validation;
using DatingApp.Data.Tenant.Identity;
using DatingApp.Tenant.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DatingApp.Api.UnitOfWork
{
    public class AppUnitOfWork : UnitOfWorkWithEventsBase, IAppUnitOfWork
    {
        public AppUnitOfWork(
            Tenant.Data.AppContext appContext,
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
