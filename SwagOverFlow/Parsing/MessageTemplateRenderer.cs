using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace SwagOverFlow.Parsing
{
    //https://github.com/serilog/serilog/blob/dev/src/Serilog/Rendering/MessageTemplateRenderer.cs
    static class MessageTemplateRenderer
    {
        //static readonly JsonValueFormatter JsonValueFormatter = new JsonValueFormatter("$type");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Render(MessageTemplate messageTemplate, IReadOnlyDictionary<string, string> properties, TextWriter output, string format = null, IFormatProvider formatProvider = null)
        {
            bool isLiteral = false, isJson = false;

            if (format != null)
            {
                for (var i = 0; i < format.Length; ++i)
                {
                    if (format[i] == 'l')
                        isLiteral = true;
                    else if (format[i] == 'j')
                        isJson = true;
                }
            }

            for (var ti = 0; ti < messageTemplate.TokenArray.Length; ++ti)
            {
                var token = messageTemplate.TokenArray[ti];
                if (token is TextToken tt)
                {
                    RenderTextToken(tt, output);
                }
                else
                {
                    var pt = (PropertyToken)token;
                    RenderPropertyToken(pt, properties, output, formatProvider, isLiteral, isJson);
                }
            }
        }

        public static void RenderTextToken(TextToken tt, TextWriter output)
        {
            output.Write(tt.Text);
        }

        public static void RenderPropertyToken(PropertyToken pt, IReadOnlyDictionary<string, string> properties, TextWriter output, IFormatProvider formatProvider, bool isLiteral, bool isJson)
        {
            if (!properties.TryGetValue(pt.PropertyName, out var propertyValue))
            {
                output.Write(pt.RawText);
                return;
            }

            if (!pt.Alignment.HasValue)
            {
                RenderValue(propertyValue, isLiteral, isJson, output, pt.Format, formatProvider);
                return;
            }

            var valueOutput = new StringWriter();
            RenderValue(propertyValue, isLiteral, isJson, valueOutput, pt.Format, formatProvider);
            var value = valueOutput.ToString();

            if (value.Length >= pt.Alignment.Value.Width)
            {
                output.Write(value);
                return;
            }

            Padding.Apply(output, value, pt.Alignment.Value);
        }

        //static void RenderValue(LogEventPropertyValue propertyValue, bool literal, bool json, TextWriter output, string format, IFormatProvider formatProvider)
        static void RenderValue(String propertyValue, bool literal, bool json, TextWriter output, string format, IFormatProvider formatProvider)
        {
            //if (literal && propertyValue is ScalarValue sv && sv.Value is string str)
            //{
            //    output.Write(str);
            //}
            //if (json && format == null)
            //{
            //    JsonValueFormatter.Format(propertyValue, output);
            //}
            //else
            //{
            //    propertyValue.Render(output, format, formatProvider);
            //}

            if (format != null && format.StartsWith("int:") && Int32.TryParse(propertyValue, out Int32 valInt))
            {
                String formatInt = format.Substring(4);
                output.Write(valInt.ToString(formatInt));
            }
            else if (format != null && format.StartsWith("dt:") && DateTime.TryParse(propertyValue, out DateTime valDateTime))
            {
                String formatDateTime = format.Substring(3);
                output.Write(valDateTime.ToString(formatDateTime));
            }
            else
            {
                output.Write(propertyValue);
            }
        }
    }
}
