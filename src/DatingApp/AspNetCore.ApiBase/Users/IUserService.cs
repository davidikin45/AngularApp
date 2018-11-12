using System.Security.Claims;

namespace AspNetCore.ApiBase.Users
{
    public interface IUserService
    {
        ClaimsPrincipal User { get; }
        string UserId { get; }
    }
}
