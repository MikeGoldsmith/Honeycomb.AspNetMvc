using Honeycomb.AspNetMvc.Filters;
using System.Web.Mvc;

namespace Honeycomb.AspNetMvc
{
    public static class HoneycombMvc
    {
        public static void Register()
        {
            GlobalFilters.Filters.Add(new HoneycombExceptionFilter());
            GlobalFilters.Filters.Add(new HoneycombActionFilter());
        }
    }
}
