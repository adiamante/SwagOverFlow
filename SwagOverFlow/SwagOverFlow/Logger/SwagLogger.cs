using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace SwagOverFlow.Logger
{
    public static class SwagLogger
    {
        private static ConcurrentDictionary<object, Stopwatch> _dictStopWatches = new ConcurrentDictionary<object, Stopwatch>();
        
        public static SwagSinkHandler SwagSinkEvent;
        static SwagLogger()
        {
            //https://improveandrepeat.com/2014/08/structured-logging-with-serilog/
            //https://github.com/serilog/serilog/wiki/Formatting-Output
            //https://github.com/serilog/serilog-formatting-compact
            Serilog.Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                //.WriteTo.Debug(outputTemplate: "[{Message:lj}]{NewLine}{Properties:j}{NewLine}")
                //.WriteTo.Debug(formatter : new RenderedCompactJsonFormatter())
                .WriteTo.Debug(formatter: new SwagSimpleCompactFormatter())
                .WriteTo.File(path: "swaglog.txt", formatter: new JsonFormatter())
                .WriteTo.SwagSink()
                .CreateLogger();
        }

        public static void Log(String messageTemplate, params object [] propertyValues)
        {
            Serilog.Log.Logger.Information(messageTemplate, propertyValues);
        }

        public static void Log(Exception ex, String messageTemplate, params object[] propertyValues)
        {
            Serilog.Log.Logger.Information(ex, messageTemplate, propertyValues);
        }

        public static void LogStart(Object key, String messageTemplate, params object [] propertyValues)
        {
            if (!_dictStopWatches.ContainsKey(key))
            {
                _dictStopWatches.TryAdd(key, new Stopwatch());
            }

            _dictStopWatches[key].Start();
            Log("[Start - 00:00:00.0000000] " + messageTemplate, propertyValues);
        }

        public static void LogEnd(Object key, String messageTemplate, params object[] propertyValues)
        {
            if (!_dictStopWatches.ContainsKey(key))
            {
                throw new KeyNotFoundException("Log key not found.");
            }

            _dictStopWatches[key].Stop();
            object[] newProps = new object[propertyValues.Length + 1];
            propertyValues.CopyTo(newProps, 1);
            newProps[0] = _dictStopWatches[key].Elapsed;

            Log("[End   - {Elapsed}] " + messageTemplate, newProps);
        }
    }
}
