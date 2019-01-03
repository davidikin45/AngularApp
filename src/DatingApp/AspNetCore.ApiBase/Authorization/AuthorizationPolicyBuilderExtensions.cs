using AspNetCore.ApiBase.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCore.ApiBase.Authorization
{
    public static class AuthorizationPolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequireScope(this AuthorizationPolicyBuilder builder, params string[] scope)
        {
            return builder.RequireClaim("scope", scope);
        }

        public static AuthorizationPolicyBuilder RequireScopeRequirement(this AuthorizationPolicyBuilder builder, params string[] scope)
        {
            builder.Requirements.Add(new ScopeAuthorizationRequirement(scope));
            return builder;
        }
    }
}
