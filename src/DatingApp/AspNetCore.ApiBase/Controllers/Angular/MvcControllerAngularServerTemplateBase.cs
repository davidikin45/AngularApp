using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.ApiBase.Controllers.Angular
{
    public abstract class MvcControllerAngularServerTemplateBase : Controller
    {
        public PartialViewResult Render(string feature, string name)
        {
            return PartialView(string.Format("~/wwwroot/js/app/{0}/templates/{1}", feature, name));
        }
    }
}
