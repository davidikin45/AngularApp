using AspNetCore.ApiBase.Controllers.Api.Authentication;
using AspNetCore.ApiBase.Email;
using AspNetCore.ApiBase.Settings;
using AutoMapper;
using DatingApp.Tenant.Core.Dtos;
using DatingApp.Tenant.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DatingApp.Tenant.Api.Controllers
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/auth")]
    public class AuthController : ApiControllerRegistrationBase<User, RegisterDto>
    {
        public AuthController(
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TokenSettings tokenSettings,
            IUrlHelper urlHelper,
            LinkGenerator linkGenerator,
            IEmailService emailSender,
            IMapper mapper,
            PasswordSettings passwordSettings,
            EmailTemplates emailTemplates,
            AppSettings appSettings)
            : base(roleManager, userManager, signInManager, tokenSettings, urlHelper, linkGenerator, emailSender, mapper, passwordSettings, emailTemplates, appSettings)
        {

        }
    }
}