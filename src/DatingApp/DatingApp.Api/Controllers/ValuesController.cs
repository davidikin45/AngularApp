using AspNetCore.ApiBase.Controllers.Api;
using AspNetCore.ApiBase.DomainEvents;
using AspNetCore.ApiBase.Email;
using AspNetCore.ApiBase.Reflection;
using AspNetCore.ApiBase.Settings;
using AutoMapper;
using DatingApp.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
    [Route("api/values")]
    [ApiController]
    public class ValuesController : ApiControllerEntityBase<Value, Value, Value, Value, IValueApplicationService>
    {
        public ValuesController(IValueApplicationService applicationService, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService,
            AppSettings appSettings, IAuthorizationService authorizationService, IActionEventsService actionEventsService )
            :base("values", applicationService, mapper, emailService, urlHelper, typeHelperService, appSettings, authorizationService, actionEventsService)
        {

        }
    }
}
