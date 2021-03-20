using BulkMailSender.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BulkMailSender.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,Inherited = false)]
    public class GlobalActionModelValidateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var errors = actionContext.ModelState
                             .Where(a => a.Value.Errors.Count > 0)
                             .SelectMany(x => x.Value.Errors)
                             .ToList();
                var errorss = new List<string>
                {
                    errors.ToString()
                };

                var genericErrorMessage = new ServiceResponse<object>
                {
                    Success = false,
                    Errors = errorss
                };

                var result = new ObjectResult(genericErrorMessage) { StatusCode = (int)HttpStatusCode.BadRequest };
                actionContext.Result = result;
            }
        }
    }
}
