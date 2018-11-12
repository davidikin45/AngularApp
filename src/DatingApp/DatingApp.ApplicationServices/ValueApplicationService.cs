using AspNetCore.ApiBase.ApplicationServices;
using AspNetCore.ApiBase.DomainEvents;
using AspNetCore.ApiBase.Users;
using AspNetCore.ApiBase.Validation;
using AutoMapper;
using DatingApp.Core;
using Microsoft.AspNetCore.Authorization;

namespace DatingApp.ApplicationServices
{
    public class ValueApplicationService : ApplicationServiceEntityBase<Value, Value, Value, Value, Value, IAppUnitOfWork>, IValueApplicationService
    {
        public ValueApplicationService(IMapper mapper, IAppUnitOfWork unitOfWork, IAuthorizationService auth, IUserService userService, IValidationService validationService, IActionEventsService actionEventsService)
            :base("value", unitOfWork, mapper, auth, userService, validationService, actionEventsService)
        {

        }
    }
}
