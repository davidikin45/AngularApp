﻿using AspNetCore.ApiBase.Authorization;
using AspNetCore.ApiBase.Controllers.Api;
using AspNetCore.ApiBase.Email;
using AspNetCore.ApiBase.Reflection;
using AspNetCore.ApiBase.Settings;
using AutoMapper;
using DatingApp.Tenant.Core;
using DatingApp.Tenant.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DatingApp.Tenant.Api.Controllers
{
    [ResourceCollection(ResourceCollections.Values.CollectionId)]
    [ApiVersion("1.0")]
    [Route("api/values")]
    public class ValuesController : ApiControllerEntityAuthorizeBase<Value, Value, Value, Value, IValueApplicationService>
    {
        public ValuesController(IValueApplicationService applicationService, IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, ITypeHelperService typeHelperService,
            AppSettings appSettings)
            :base( applicationService, mapper, emailService, linkGenerator, typeHelperService, appSettings)
        {

        }
    }
}
