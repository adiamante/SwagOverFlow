using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Parsing;

namespace SwagOverFlow.Logger
{
    //https://github.com/serilog/serilog-formatting-compact/blob/dev/src/Serilog.Formatting.Compact/Formatting/Compact/CompactJsonFormatter.cs
    public class SwagSimpleCompactFormatter : ITextFormatter
    {
        readonly JsonValueFormatter _valueFormatter;

        public SwagSimpleCompactFormatter(JsonValueFormatter valueFormatter = null)
        {
            _valueFormatter = valueFormatter ?? new JsonValueFormatter(typeTagName: "$type");
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            FormatEvent(logEvent, output, _valueFormatter);
            output.WriteLine();
        }

        public static void FormatEvent(LogEvent logEvent, TextWriter output, JsonValueFormatter valueFormatter)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (output == null) throw new ArgumentNullException(nameof(output));

            String message = logEvent.MessageTemplate.Render(logEvent.Properties);
            String values = "";
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
                valueFormatter.Format(kvp.Value, swValue);
                values += swValue.ToString();
                values += "|";

                if (kvp.Value is ScalarValue && ((ScalarValue)kvp.Value).Value is String)
                {
                    message = message.Replace(kvp.Value.ToString(), kvp.Value.ToString().TrimStart('\"').TrimEnd('\"'));
                    values = values.Replace(kvp.Value.ToString(), kvp.Value.ToString().TrimStart('\"').TrimEnd('\"'));
                }
            }
            output.Write("[{0}] {1}", logEvent.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"), message);
            output.Write(" => |{0}", values);
        }
    }
}
