using Honeycomb.AspNetMvc.Extensions;
using System.Web.Mvc;

namespace Honeycomb.AspNetMvc.Filters
{
    internal class HoneycombActionFilter : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var context = filterContext.RequestContext;
            var httpContext = filterContext.HttpContext;
            httpContext.AddHoneycombData("name", $"{context.GetRouteValue("controller")}#{context.GetRouteValue("action")}");
            httpContext.AddHoneycombData("action", context.GetRouteValue("action"));
            httpContext.AddHoneycombData("controller", context.GetRouteValue("controller"));
            httpContext.AddHoneycombData("response.status_code", context.HttpContext.Response.StatusCode);
        }
    }
}
