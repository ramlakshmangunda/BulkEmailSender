using BulkMailSender.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkMailSender.Filters
{
    public class GlobalActionResponseFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context != null)
            {
                var objectContent = context.Result as ObjectResult;
                if (objectContent != null)
                {
                    var result = new ServiceResponse<object>
                    {
                        Success = true,
                        Result = objectContent.Value
                    };

                    objectContent.Value = result;
                }
            }
        }
    }
}
