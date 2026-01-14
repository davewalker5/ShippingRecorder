using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HealthTracker.Mvc.Attributes
{
    public class AjaxOnlyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // See if this is an Ajax request or not
            var isAjax = context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (!isAjax)
            {
                // Not an Ajax request, so it's forbidden
                context.Result = new ForbidResult();
                return;
            }

            // Allow pipeline to continue to the action
            await next();
        }
    }
}