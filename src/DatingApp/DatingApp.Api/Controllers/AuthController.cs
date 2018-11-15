using AspNetCore.ApiBase;
using AspNetCore.ApiBase.Authorization;
using AspNetCore.ApiBase.Controllers.Api.Authentication;
using AspNetCore.ApiBase.Email;
using AspNetCore.ApiBase.Settings;
using AspNetCore.ApiBase.Users;
using AutoMapper;
using DatingApp.Core.Dtos;
using DatingApp.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
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