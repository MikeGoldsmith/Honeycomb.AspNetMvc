using Honeycomb.AspNetMvc.Extensions;
using Honeycomb.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Timers;
using System.Web;

namespace Honeycomb.AspNetMvc
{
    public class HoneycombModule : IHttpModule
    {
        private static bool initialised;
        private static object lockObject = new object();
        private static IServiceProvider _serviceProvider;
        private static Timer _timer;

        private void InitOnce()
        {
            _serviceProvider = SetupServiceProvider();

            var settings = _serviceProvider.GetRequiredService<IOptions<HoneycombApiSettings>>();

            _timer = new Timer(settings.Value.SendFrequency);
            _timer.Elapsed += FlushEvents;
            _timer.Start();
        }

        public void Init(HttpApplication context)
        {
            lock (lockObject)
            {
                if (!initialised)
                {
                    InitOnce();
                    initialised = true;
                }
            }

            context.BeginRequest += BeginRequest;
            context.EndRequest += FinishRequest;
        }

        private void BeginRequest(object sender, EventArgs e)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var context = ((HttpApplication)sender).Context;
            context.Items.Add("honeycomb_stopwatch", stopwatch);

            var settings = _serviceProvider.GetRequiredService<IOptions<HoneycombApiSettings>>();
            var ev = new HoneycombEvent
            {
                DataSetName = settings.Value.DefaultDataSet
            };
            context.Items.Add(Constants.ContextItemName, ev);

            ev.Data.Add("trace.trace_id", Guid.NewGuid().ToString().Replace("-", ""));
            ev.Data.Add("request.path", context.Request.Path);
            ev.Data.Add("request.method", context.Request.HttpMethod);
            ev.Data.Add("request.http_version", context.Request.Url.Scheme);
            ev.Data.Add("request.content_length", context.Request.ContentLength);
            ev.Data.Add("meta.local_hostname", Environment.MachineName);
        }

        private void FinishRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;
            var stopwatch = context.Items["honeycomb_stopwatch"] as Stopwatch;
            stopwatch.Stop();

            var ev = context.Items[Constants.ContextItemName] as HoneycombEvent;
            ev.Data.TryAdd("response.status_code", context.Response.StatusCode);
            ev.Data.TryAdd("duration_ms", stopwatch.ElapsedMilliseconds);

            _serviceProvider.GetRequiredService<IHoneycombService>().QueueEvent(ev);
        }

        private async void FlushEvents(object sender, ElapsedEventArgs e)
        {
            var service = _serviceProvider.GetRequiredService<IHoneycombService>();
            await service.Flush();
        }

        private IServiceProvider SetupServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            services.AddSingleton<IHoneycombService, HoneycombService>();
            services.AddLogging();
            services.Configure<HoneycombApiSettings>(o => {
                o.TeamId = ConfigurationManager.AppSettings.Get("Honeycomb.TeamId");
                o.DefaultDataSet = ConfigurationManager.AppSettings.Get("Honeycomb.DefaultDataSet");
                if (int.TryParse(ConfigurationManager.AppSettings.Get("Honeycomb.SendFrequency"), out var sendFrequency))
                    o.SendFrequency = sendFrequency;
                if (int.TryParse(ConfigurationManager.AppSettings.Get("Honeycomb.BatchSize"), out var batchSize))
                    o.BatchSize = batchSize;
            });
            return services.BuildServiceProvider();
        }

        public void Dispose()
        {
        }

    }
}