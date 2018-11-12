using AspNetCore.ApiBase.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.ApiBase.Filters
{
    //Add this to controller to enable and disable features
    //[TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "Loan" })]
    public class FeatureAuthFilter : IAuthorizationFilter
    {
        private IFeatureService featureService;
        private string featureName;

        public FeatureAuthFilter(IFeatureService featureService, string featureName)
        {
            this.featureService = featureService;
            this.featureName = featureName;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!featureService.IsFeatureActive(featureName))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
