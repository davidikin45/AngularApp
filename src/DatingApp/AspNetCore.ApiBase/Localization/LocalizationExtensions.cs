using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AspNetCore.ApiBase.Localization
{
    public static class LocalizationExtensions
    {
        public static IApplicationBuilder UseRequestLocalizationCustom(this IApplicationBuilder app, string defaultCulture, params string[] supportedUICultures)
        {
            //https://andrewlock.net/adding-localisation-to-an-asp-net-core-application/
            //Default culture should be set to where the majority of traffic comes from.
            //If the client sends through "en" and the default culture is "en-AU". Instead of falling back to "en" it will fall back to "en-AU".
            var defaultLanguage = defaultCulture.Split('-')[0];

            //Support all formats for numbers, dates, etc.
            var formatCulturesList = new List<string>();

            //Countries
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                if (!formatCulturesList.Contains(ci.Name))
                {
                    formatCulturesList.Add(ci.Name);
                }
            }

            //Languages
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
            {
                if (!formatCulturesList.Contains(ci.TwoLetterISOLanguageName) && ci.TwoLetterISOLanguageName != defaultLanguage)
                {
                    formatCulturesList.Add(ci.TwoLetterISOLanguageName);
                }
            }

            var supportedFormatCultures = formatCulturesList.Select(x => new CultureInfo(x)).ToArray();

            var supportedUICultureInfoList = new List<CultureInfo>() {
                new CultureInfo(defaultCulture)
            };

            foreach (var supportedUICulture in supportedUICultures)
            {
                if(supportedUICulture != defaultCulture)
                {
                    supportedUICultureInfoList.Add(new CultureInfo(supportedUICulture));
                }
            }

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(culture: defaultCulture, uiCulture: defaultCulture),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedFormatCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedUICultureInfoList
            });

            return app;
        }
    }
}
