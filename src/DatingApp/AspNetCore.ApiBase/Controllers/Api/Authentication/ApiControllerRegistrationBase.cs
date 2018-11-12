using AspNetCore.ApiBase.Email;
using AspNetCore.ApiBase.Settings;
using AspNetCore.ApiBase.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Controllers.Api.Authentication
{
    public abstract class ApiControllerRegistrationBase<TUser, TRegistrationDto> : ApiControllerAuthenticationBase<TUser>
        where TUser : IdentityUser
        where TRegistrationDto : RegisterDto
    {
        private readonly UserManager<TUser> _userManager;

        private readonly string _welcomeEmailTemplate;

        public ApiControllerRegistrationBase(
            string resource,
            RoleManager<IdentityRole> roleManager,
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            TokenSettings tokenSettings,
            IUrlHelper urlHelper,
            IEmailService emailSender,
            IMapper mapper,
            PasswordSettings passwordSettings,
            EmailTemplates emailTemplates,
            AppSettings appSettings,
            IAuthorizationService authorizationService)
            :base(resource, roleManager, userManager, signInManager, tokenSettings, urlHelper, emailSender, mapper, passwordSettings, emailTemplates, appSettings, authorizationService)
        {
            _welcomeEmailTemplate = emailTemplates.Welcome;
        }

        #region Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] TRegistrationDto registerDto)
        {
            var user = Mapper.Map<TUser>(registerDto);
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(_welcomeEmailTemplate))
                {
                    await EmailService.SendWelcomeEmailAsync(_welcomeEmailTemplate, user.Email);
                }
                return await GenerateJWTToken(user);
            }
            AddErrors(result);
            return ValidationErrors();
        }
        #endregion
    }
}
