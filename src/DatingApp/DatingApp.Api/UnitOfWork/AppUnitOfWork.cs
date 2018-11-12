using AspNetCore.ApiBase.Data.UnitOfWork;
using AspNetCore.ApiBase.DomainEvents;
using AspNetCore.ApiBase.Validation;
using DatingApp.Core;
using DatingApp.Data;
using DatingApp.Data.Identity;

namespace DatingApp.Api.UnitOfWork
{
    public class AppUnitOfWork : UnitOfWorkWithEventsBase, IAppUnitOfWork
    {
        public AppUnitOfWork(
            AppContext appContext,
            IdentityContext identityContext,
            IValidationService validationService,
            IDomainEventsDispatcher domainEventsDispatcher) 
            : base(true, validationService, domainEventsDispatcher, appContext, identityContext)
        {

        }
    }
}
