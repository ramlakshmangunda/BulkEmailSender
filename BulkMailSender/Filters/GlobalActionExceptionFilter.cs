using BulkMailSender.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Net;

namespace BulkMailSender.Filters
{
    public class GlobalActionExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            // Access Exception
            var exception = context.Exception;

            Serilog.Log.Error(exception.InnerException, "Application Error");

            var errors = new List<string>
            {
                exception.Message
            };
            var genericErrorMessage = new ServiceResponse<object>
            {
                Success = false,
                Errors = errors
            };

            var result = new ObjectResult(genericErrorMessage) { StatusCode = (int)HttpStatusCode.OK };
            context.Result = result;
        }
    }
}
