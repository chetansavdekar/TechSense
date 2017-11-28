using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechSense.Filters
{
    public class IsAuthenticatedActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            var result = context.Result as ViewResult;
            if (result != null)
            {
                result.ViewData["IsAuthenticated"] = (context.HttpContext?.User?.Identity?.IsAuthenticated ?? false).ToString().Trim().ToLower();
                result.ViewData["Username"] = context.HttpContext?.User?.Identity?.Name ?? "";
            }
        }
    }
}
