using AspNetCore.ApiBase.Authorization;
using AspNetCore.ApiBase.Controllers.Api;
using AspNetCore.ApiBase.Email;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.Reflection;
using AspNetCore.ApiBase.Settings;
using AutoMapper;
using DatingApp.Admin.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DatingApp.Admin.Api.Controllers
{
    [ResourceCollection(ResourceCollections.Tenants.Name)]
    [ApiVersion("1.0")]
    [Route("api/tenants")]
    public class TenantsController : ApiControllerEntityAuthorizeBase<AppTenant, AppTenant, AppTenant, AppTenant, ITenantApplicationService>
    {
        public TenantsController(ITenantApplicationService applicationService, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, LinkGenerator linkGenerator, ITypeHelperService typeHelperService,
            AppSettings appSettings)
            :base( applicationService, mapper, emailService, urlHelper, linkGenerator, typeHelperService, appSettings)
        {

        }
    }
}
