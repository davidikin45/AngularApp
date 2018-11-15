using AspNetCore.ApiBase.MultiTenancy;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.ApiBase.Extensions
{
    public static class ServiceCollectionMvcExtensions
    {
        public static IServiceCollection AddViewLocationExpander(this IServiceCollection services, string mvcImplementationFolder = "Mvc/")
        {
            return services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Insert(0, new ViewLocationExpander(mvcImplementationFolder));
            });
        }
    }
}
