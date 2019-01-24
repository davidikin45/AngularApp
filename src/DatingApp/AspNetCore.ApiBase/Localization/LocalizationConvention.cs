using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;

namespace AspNetCore.ApiBase.Localization
{
    //https://andrewlock.net/applying-the-routedatarequest-cultureprovider-globally-with-middleware-as-filters/
    public class LocalizationConvention : IApplicationModelConvention
    {
        private readonly string _defaultCulture;
        public LocalizationConvention(string defaultCulture)
        {
            _defaultCulture = defaultCulture;
        }

        public void Apply(ApplicationModel application)
        {
            var culturePrefix = new AttributeRouteModel(new RouteAttribute("{culture:cultureCheck" + (!string.IsNullOrEmpty(_defaultCulture) ? $"={_defaultCulture}" : "") + "}"));

            foreach (var controller in application.Controllers)
            {
                var matchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
                if (matchedSelectors.Any())
                {
                    foreach (var selectorModel in matchedSelectors)
                    {
                        selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(culturePrefix,
                            selectorModel.AttributeRouteModel);
                    }
                }

                var unmatchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel == null).ToList();
                if (unmatchedSelectors.Any())
                {
                    foreach (var selectorModel in unmatchedSelectors)
                    {
                        selectorModel.AttributeRouteModel = culturePrefix;
                    }
                }
            }
        }
    }
}
