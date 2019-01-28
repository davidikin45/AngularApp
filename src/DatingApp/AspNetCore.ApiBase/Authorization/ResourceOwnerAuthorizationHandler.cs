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
            if (context.User.Claims.Where(c => c.Type == JwtClaimTypes.Scope && c.Value == ResourceCollectionsCore.Admin.Scopes.Full).Count() > 0)
            {
                context.Succeed(requirement);
            }
            else if (context.User.Claims.Where(c => c.Type == JwtClaimTypes.Scope && requirement.Name.Contains(ResourceCollectionsCore.Admin.Scopes.Create) && c.Value == ResourceCollectionsCore.Admin.Scopes.Create).Count() > 0)
            {
                context.Succeed(requirement);
            }
            else if (context.User.Claims.Where(c => c.Type == JwtClaimTypes.Scope && requirement.Name.Contains(ResourceCollectionsCore.Admin.Scopes.Read) && c.Value == ResourceCollectionsCore.Admin.Scopes.Read).Count() > 0)
            {
                context.Succeed(requirement);
            }
            else if (context.User.Claims.Where(c => c.Type == JwtClaimTypes.Scope && requirement.Name.Contains(ResourceCollectionsCore.Admin.Scopes.Update) && c.Value == ResourceCollectionsCore.Admin.Scopes.Update).Count() > 0)
            {
                context.Succeed(requirement);
            }
            else if (context.User.Claims.Where(c => c.Type == JwtClaimTypes.Scope && requirement.Name.Contains(ResourceCollectionsCore.Admin.Scopes.Delete) && c.Value == ResourceCollectionsCore.Admin.Scopes.Delete).Count() > 0)
            {
                context.Succeed(requirement);
            }
            else if (!requirement.Name.Contains("-if-owner") && context.User.Claims.Where(c => c.Type == JwtClaimTypes.Scope && c.Value == requirement.Name).Count() > 0)
            {
                context.Succeed(requirement);
            }
            else if (requirement.Name.Contains("-if-owner") && context.User.Claims.Where(c => c.Type == JwtClaimTypes.Scope && c.Value == requirement.Name).Count() > 0)
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (entity.OwnedBy == userId)
                {
                    context.Succeed(requirement);
                }else if(entity.OwnedBy == null && requirement.Name.Contains("read"))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
