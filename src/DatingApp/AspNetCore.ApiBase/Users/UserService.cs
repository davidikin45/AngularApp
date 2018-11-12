using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace AspNetCore.ApiBase.Users
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimsPrincipal User { get; }
        public string UserId { get; } = null;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            // service is scoped, created once for each request => we only need
            // to fetch the info in the constructor
            _httpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            var currentContext = _httpContextAccessor.HttpContext;
            if (currentContext == null)
            {
                return;
            }

            User = currentContext.User;

            if (!currentContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            var claim = currentContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return;
            }

            UserId = claim.Value;
        }
    }
}
