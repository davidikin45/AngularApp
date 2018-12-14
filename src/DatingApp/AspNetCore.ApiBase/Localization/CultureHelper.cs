using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AspNetCore.ApiBase.Localization
{
    public static class CultureHelper
    {
        public static IEnumerable<SelectListItem> CultureList()
        {
           return CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(c => new SelectListItem()
            {
                Value = c.Name,
                Text = c.DisplayName,
                Selected = c.Name == CultureInfo.CurrentCulture.Name
            })
            .OrderBy(s => s.Text);
        }
    }
}
