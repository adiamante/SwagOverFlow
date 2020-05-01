using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SwagOverFlow.Logger
{
    public delegate void SwagSinkHandler(SwagSinkEvent sse);

    public class SwagSinkEvent
    {
        JsonValueFormatter _valueFormatter = new JsonValueFormatter(typeTagName: "$type");
        public DateTimeOffset Timestamp { get; }
        public String LogEventLevel { get; }
        public String MessageTemplate { get; }
        public String Message { get; }
        public String Properties { get; }
        public Exception Exception { get; }
        public JObject JProperties { get; }

        public SwagSinkEvent(LogEvent logEvent)
        {
            Timestamp = logEvent.Timestamp;
            LogEventLevel = logEvent.ToString();
            MessageTemplate = logEvent.MessageTemplate.ToString();
            Exception = logEvent.Exception;
            JProperties = JObject.FromObject(logEvent.Properties);

            String message = logEvent.MessageTemplate.Render(logEvent.Properties);
            String values = "|";
            foreach (KeyValuePair<String, LogEventPropertyValue> kvp in logEvent.Properties)
            {
                String name = kvp.Key;
                if (name.Length > 0 && name[0] == '@')
                {
                    // Escape first '@' by doubling
                    name = '@' + name;
                }
                values += $"{name}=";

                StringWriter swValue = new StringWriter();
                _valueFormatter.Format(kvp.Value, swValue);
                values += swValue.ToString();
                values += "|";

                if (kvp.Value is ScalarValue && ((ScalarValue)kvp.Value).Value is String)
                {
                    message = message.Replace(kvp.Value.ToString(), kvp.Value.ToString().TrimStart('\"').TrimEnd('\"'));
                    values = values.Replace(kvp.Value.ToString(), kvp.Value.ToString().TrimStart('\"').TrimEnd('\"'));
                }
            }

            Message = message;
            Properties = values;
        }
    }

    //https://github.com/serilog/serilog/wiki/Developing-a-sink
    public class SwagSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        public SwagSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            if (SwagLogger.SwagSinkEvent != null)
            {
                SwagLogger.SwagSinkEvent.Invoke(new SwagSinkEvent(logEvent));
            }
        }
    }

    public static class SwagSinkExtensions
    {
        public static LoggerConfiguration SwagSink(
                  this LoggerSinkConfiguration loggerConfiguration,
                  IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new SwagSink(formatProvider));
        }
    }
}
