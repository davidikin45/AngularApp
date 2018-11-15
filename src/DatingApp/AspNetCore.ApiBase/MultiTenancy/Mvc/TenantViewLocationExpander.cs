using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.ApiBase.MultiTenancy.Mvc
{
    public class TenantViewLocationExpander<TTenant> : IViewLocationExpander
    where TTenant : AppTenant
    {
        private readonly string MvcImplementationFolder;
        private string _tenantId;

        public TenantViewLocationExpander(string MvcImplementationFolder = "Mvc/")
        {
            this.MvcImplementationFolder = MvcImplementationFolder;
        }

        private string[] Locations()
        {
            string[] locations = {
                "~/" + MvcImplementationFolder + "{1}/Views/{0}.cshtml",
                "~/" + MvcImplementationFolder + "Shared/Views/{0}.cshtml"
            };

            return locations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
           var tenantProvider = (ITenantService<TTenant>)context.ActionContext.HttpContext.RequestServices.GetService(typeof(ITenantService<TTenant>));
            _tenantId = tenantProvider.GetTenant().Id.ToString();
        }

        //The view locations passed to ExpandViewLocations are:
        // /Views/{1}/{0}.cshtml
        // /Shared/{0}.cshtml
        // /Pages/{0}.cshtml
        //Where {0} is the view and {1} the controller name.
        public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var locations = viewLocations.Union(Locations());

            foreach (var location in viewLocations)
            {
                yield return location.Replace("{0}", this._tenantId + "/{0}");
                yield return location;
            }
        }
    }
}
