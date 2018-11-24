﻿using AspNetCore.ApiBase.ApplicationServices;
using AspNetCore.ApiBase.Authorization;
using AspNetCore.ApiBase.DomainEvents;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.Users;
using AspNetCore.ApiBase.Validation;
using AutoMapper;
using DatingApp.Admin.Core;
using Microsoft.AspNetCore.Authorization;

namespace DatingApp.Admin.ApplicationServices
{
    [Resource(ResourceOperations.Tenants.Name)]
    public class ValueApplicationService : ApplicationServiceEntityBase<AppTenant, AppTenant, AppTenant, AppTenant, AppTenant, IAppUnitOfWork>, ITenantApplicationService
    {
        public ValueApplicationService(IMapper mapper, IAppUnitOfWork unitOfWork, IAuthorizationService auth, IUserService userService, IValidationService validationService, IActionEventsService actionEventsService)
            :base(unitOfWork, mapper, auth, userService, validationService, actionEventsService)
        {

        }
    }
}
