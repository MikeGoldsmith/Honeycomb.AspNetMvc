using Honeycomb.Models;
using System.Web;

namespace Honeycomb.AspNetMvc.Extensions
{
    internal static class HttpContextExtensions
    {
        public static void AddHoneycombData(this HttpContextBase context, string key, object data)
        {
            if (!context.Items.Contains(Constants.ContextItemName))
                return;

            var ev = context.Items[Constants.ContextItemName] as HoneycombEvent;
            ev.Data.TryAdd(key, data);
        }
    }
}
