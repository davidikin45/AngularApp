using AspNetCore.ApiBase.Alerts;
using AspNetCore.ApiBase.Validation;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.ApiBase.Controllers.Api
{
    //using CSharpFunctionalExtensions;
    public abstract class ApiControllerCQRSBase : Controller
    {
        protected new IActionResult Ok()
        {
            return base.Ok(Envelope.Ok());
        }

        protected IActionResult Ok<T>(T result)
        {
            return base.Ok(Envelope.Ok(result));
        }

        protected IActionResult Error(string errorMessage)
        {
            return BadRequest(Envelope.Error(errorMessage));
        }

        protected IActionResult FromResult(Result result)
        {
            return result.IsSuccess ? Ok() : Error("");
        }
    }
}
