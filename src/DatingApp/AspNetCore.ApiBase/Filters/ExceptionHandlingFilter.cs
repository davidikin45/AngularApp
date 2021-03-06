﻿using AspnetCore.ApiBase.Validation.Errors;
using AspNetCore.ApiBase.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace AspNetCore.ApiBase.Filters
{
    //This will only handle MVC errors
    //https://stackoverflow.com/questions/42582758/asp-net-core-middleware-vs-filters
    public class ExceptionHandlingFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public ExceptionHandlingFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExceptionHandlingFilter>();
        }
        public override void OnException(ExceptionContext context)
        {
            LogException(context);
            if (context.HttpContext.Request.Path.ToString().StartsWith("/api"))
            {
                var result = ApiErrorHandler.HandleApiException(context.HttpContext.User, context.Exception);
                if (result.exceptionHandled)
                {
                    context.ExceptionHandled = true;

                    var objectResult = new ObjectResult(result.message);
                    objectResult.StatusCode = result.statusCode;

                    context.Result = objectResult;
                }
            }
            else
            {
                var result = MvcErrorHandler.HandleException(context.HttpContext.User, context.Exception);
                if (result.exceptionHandled)
                {
                    context.ExceptionHandled = true;
                    context.Result = result.result;
                }
            }
        }

        private void LogException(ExceptionContext context)
        {
            if (context.Exception is UnauthorizedErrors)
            {
                _logger.LogInformation(Messages.Unauthorised);
            }
            else if (context.Exception is TimeoutException)
            {
                _logger.LogInformation(Messages.RequestTimedOut);
            }
            else if (context.Exception is OperationCanceledException)
            {
                _logger.LogInformation(Messages.RequestCancelled);
            }
            else
            {
                _logger.LogInformation(Messages.UnknownError);
            }
        }
    }
}
