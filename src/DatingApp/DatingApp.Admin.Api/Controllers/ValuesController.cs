﻿using AspNetCore.ApiBase.Authorization;
using AspNetCore.ApiBase.Controllers.Api;
using AspNetCore.ApiBase.DomainEvents;
using AspNetCore.ApiBase.Email;
using AspNetCore.ApiBase.Reflection;
using AspNetCore.ApiBase.Settings;
using AutoMapper;
using DatingApp.Admin.Core;
using DatingApp.Admin.Domain;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Admin.Api.Controllers
{
    [Resource(ResourceOperations.Values.Name)]
    [ApiVersion("1.0")]
    [Route("api/values")]
    public class ValuesController : ApiControllerEntityAuthorizeBase<Value, Value, Value, Value, IValueApplicationService>
    {
        public ValuesController(IValueApplicationService applicationService, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService,
            AppSettings appSettings, IActionEventsService actionEventsService )
            :base( applicationService, mapper, emailService, urlHelper, typeHelperService, appSettings, actionEventsService)
        {

        }
    }
}