using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Localization
{
    public static class RouteCultureExtensions
    {
        public static MvcOptions AddCultureRouteConvention(this MvcOptions options, string defaultCulture)
        {
            options.Conventions.Insert(0, new LocalizationConvention(defaultCulture));

            return options;
        }

        public static IRouteBuilder RedirectCulturelessToDefaultCulture(this IRouteBuilder routes, RequestLocalizationOptions localizationOptions)
        {
            routes.MapRoute("{culture:cultureCheck}/{*path}", ctx => {
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                return Task.CompletedTask;
            });

            routes.MapRoute("{*path}", (RequestDelegate)(ctx =>
            {
                var defaultCulture = localizationOptions.DefaultRequestCulture.Culture.Name;
                var path = ctx.GetRouteValue("path") ?? string.Empty;
                var culturedPath = $"{ctx.Request.PathBase}/{defaultCulture}/{path}";
                ctx.Response.Redirect(culturedPath);
                return Task.CompletedTask;
            }));

            return routes;
        }
    }
}
