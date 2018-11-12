using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.ApiBase
{
    public class ViewExpander : IViewLocationExpander
    {
        private readonly string MvcImplementationFolder;

        public ViewExpander(string MvcImplementationFolder = "Mvc")
        {
            this.MvcImplementationFolder = MvcImplementationFolder;
        }

        private string[] Locations()
        {
            string[] locations = {
                "~/" + MvcImplementationFolder + "/{1}/Views/{0}.cshtml",
                "~/" + MvcImplementationFolder + "/Shared/Views/{0}.cshtml"
            };

            return locations;
        }

        public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            return viewLocations.Union(Locations());
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {

        }
    }
}
