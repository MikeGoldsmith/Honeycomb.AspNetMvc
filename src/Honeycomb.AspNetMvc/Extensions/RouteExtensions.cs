using System.Web.Routing;

namespace Honeycomb.AspNetMvc.Extensions
{
    public static class RouteExtensions
    {
        public static string GetRouteValue(this RequestContext context, string value)
        {
            if (!context.RouteData.Values.ContainsKey(value.ToLower()))
                return null;

            return context.RouteData.Values[value.ToLower()].ToString();
        }
    }
}