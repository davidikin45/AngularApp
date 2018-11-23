using AspNetCore.ApiBase.ApplicationServices;
using AspNetCore.ApiBase.Authorization;
using AspNetCore.ApiBase.DomainEvents;
using AspNetCore.ApiBase.Users;
using AspNetCore.ApiBase.Validation;
using AutoMapper;
using DatingApp.Admin.Core;
using DatingApp.Admin.Domain;
using Microsoft.AspNetCore.Authorization;

namespace DatingApp.Admin.ApplicationServices
{
    [Resource(ResourceOperations.Values.Name)]
    public class ValueApplicationService : ApplicationServiceEntityBase<Value, Value, Value, Value, Value, IAppUnitOfWork>, IValueApplicationService
    {
        public ValueApplicationService(IMapper mapper, IAppUnitOfWork unitOfWork, IAuthorizationService auth, IUserService userService, IValidationService validationService, IActionEventsService actionEventsService)
            :base(unitOfWork, mapper, auth, userService, validationService, actionEventsService)
        {

        }
    }
}
