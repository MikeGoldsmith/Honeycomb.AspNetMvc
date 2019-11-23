using System;
using System.Web;
using Honeycomb.AspNetMvc.Extensions;
using Honeycomb.Models;

namespace Honeycomb.AspNetMvc
{
    public interface IHoneycombEventManager
    {
        /// <summary>
        /// Add eventData to the webEvent
        /// </summary>
        /// <param name="key">The Key</param>
        /// <param name="data">The data</param>
        void AddData(string key, object data);
    }

    public class HoneycombEventManager : IHoneycombEventManager
    {
        private readonly Func<HttpContext> _getContext;

        public HoneycombEventManager(Func<HttpContext> getContext)
        {
            _getContext = getContext;
        }

        public void AddData(string key, object data)
        {
            var context = _getContext();
            if (!context.Items.Contains(Constants.ContextItemName))
                return;

            var ev = context.Items[Constants.ContextItemName] as HoneycombEvent;
            ev.Data.TryAdd(key, data);
        }
    }
}
