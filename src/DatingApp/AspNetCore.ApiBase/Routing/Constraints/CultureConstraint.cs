using Microsoft.AspNetCore.Routing.Constraints;

namespace AspNetCore.ApiBase.Routing.Constraints
{
    public class CultureConstraint : RegexRouteConstraint
    {
        public CultureConstraint()
            : base(@"^[a-zA-Z]{2}(\-[a-zA-Z]{2})?$") { }
    }
}
