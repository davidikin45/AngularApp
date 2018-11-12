using AspNetCore.ApiBase.Domain;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Authorization
{
    public class ResourceOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, IEntityOwned>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OperationAuthorizationRequirement requirement,
                                                       IEntityOwned entity)
        {
            if (context.User.Claims.Where(c => c.Type == JwtClaimTypes.Scope && c.Value == requirement.Name).Count() > 0)
            {
                context.Succeed(requirement);
            }
            else if (context.User.Claims.Where(c => c.Type == JwtClaimTypes.Scope && c.Value == requirement.Name + "-if-owner").Count() > 0)
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (entity.OwnedBy == null || entity.OwnedBy == userId)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
