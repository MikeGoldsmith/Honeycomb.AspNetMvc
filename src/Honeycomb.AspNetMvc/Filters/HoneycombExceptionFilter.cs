using Honeycomb.AspNetMvc.Extensions;
using System.Web.Mvc;

namespace Honeycomb.AspNetMvc.Filters
{
    public class HoneycombExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var context = filterContext.HttpContext;
            var ex = filterContext.Exception;
            context.AddHoneycombData("request.error", ex.Source);            
            context.AddHoneycombData("request.error_detail", ex.Message);
        }
    }
}
