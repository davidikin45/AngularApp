using AspNetCore.ApiBase.Controllers.Api.Authentication;
using AspNetCore.ApiBase.Email;
using AspNetCore.ApiBase.Settings;
using AutoMapper;
using DatingApp.Admin.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Admin.Api.Controllers
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/auth")]
    public class AuthController : ApiControllerAuthenticationBase<User>
    {
        public AuthController(
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TokenSettings tokenSettings,
            IUrlHelper urlHelper,
            IEmailService emailSender,
            IMapper mapper,
            PasswordSettings passwordSettings,
            EmailTemplates emailTemplates,
            AppSettings appSettings)
            : base(roleManager, userManager, signInManager, tokenSettings, urlHelper, emailSender, mapper, passwordSettings, emailTemplates, appSettings)
        {

        }
    }
}