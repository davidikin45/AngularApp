using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.ApiBase.Controllers.Angular
{
    public abstract class MvcControllerAngular6ServerTemplateBase : Controller
    {
        public PartialViewResult Render(string feature, string name)
        {
            return PartialView(string.Format("~/ClientApp/src/app/{0}/server-templates/{1}", feature, name));
        }
    }
}
