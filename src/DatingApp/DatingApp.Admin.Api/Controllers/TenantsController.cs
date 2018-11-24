using AspNetCore.ApiBase.Authorization;
using AspNetCore.ApiBase.Controllers.Api;
using AspNetCore.ApiBase.DomainEvents;
using AspNetCore.ApiBase.Email;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.Reflection;
using AspNetCore.ApiBase.Settings;
using AutoMapper;
using DatingApp.Admin.Core;
using DatingApp.Admin.Domain;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Admin.Api.Controllers
{
    [Resource(ResourceOperations.Tenants.Name)]
    [ApiVersion("1.0")]
    [Route("api/tenants")]
    public class TenantsController : ApiControllerEntityAuthorizeBase<AppTenant, AppTenant, AppTenant, AppTenant, ITenantApplicationService>
    {
        public TenantsController(ITenantApplicationService applicationService, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService,
            AppSettings appSettings, IActionEventsService actionEventsService )
            :base( applicationService, mapper, emailService, urlHelper, typeHelperService, appSettings, actionEventsService)
        {

        }
    }
}
