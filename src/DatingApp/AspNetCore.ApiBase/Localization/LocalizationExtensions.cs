using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AspNetCore.ApiBase.Localization
{
    public static class LocalizationExtensions
    {
        public static IApplicationBuilder UseRequestLocalizationCustom(this IApplicationBuilder app, string defaultCulture, bool allowCountries, bool allowLanguages, params string[] supportedUICultures)
        {
            //https://andrewlock.net/adding-localisation-to-an-asp-net-core-application/
            //Default culture should be set to where the majority of traffic comes from.
            //If the client sends through "en" and the default culture is "en-AU". Instead of falling back to "en" it will fall back to "en-AU".
            var defaultLanguage = defaultCulture.Split('-')[0];

            //Support all formats for numbers, dates, etc.
            var formatCulturesList = new List<string>();

            //Countries = en-US
            if(allowCountries)
            {
                foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                {
                    if (!formatCulturesList.Contains(ci.Name))
                    {
                        formatCulturesList.Add(ci.Name);
                    }
                }
            }

            if(allowLanguages)
            {
                //Languages = en
                foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
                {
                    if (!formatCulturesList.Contains(ci.TwoLetterISOLanguageName) && ci.TwoLetterISOLanguageName != defaultLanguage)
                    {
                        formatCulturesList.Add(ci.TwoLetterISOLanguageName);
                    }
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

           var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(culture: defaultCulture, uiCulture: defaultCulture),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedFormatCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedUICultureInfoList
            };

            //Default culture providers
            //1. Query string
            //2. Cookie
            //3. Accept-Language header

            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.2
            //https://andrewlock.net/url-culture-provider-using-middleware-as-mvc-filter-in-asp-net-core-1-1-0/
            //https://andrewlock.net/applying-the-routedatarequest-cultureprovider-globally-with-middleware-as-filters/
            //https://gunnarpeipman.com/aspnet/aspnet-core-simple-localization/
            //http://sikorsky.pro/en/blog/aspnet-core-culture-route-parameter

            //Route("{culture}/{ui-culture}/[controller]")]
            //[Route("{culture}/[controller]")]

            var routeDataRequestProvider = new RouteDataRequestCultureProvider() { Options = options, RouteDataStringKey = "culture", UIRouteDataStringKey = "ui-culture" };

            //options.RequestCultureProviders.Insert(0, routeDataRequestProvider);

            options.RequestCultureProviders = new List<IRequestCultureProvider>()
            {
                 routeDataRequestProvider,
                 new QueryStringRequestCultureProvider() { QueryStringKey = "culture", UIQueryStringKey = "ui-culture" },
                 new CookieRequestCultureProvider(),
                 new AcceptLanguageHeaderRequestCultureProvider(),
            };

            app.UseRequestLocalization(options);

            return app;
        }
    }
}
