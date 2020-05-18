using System;
using System.Collections.Generic;

namespace SwagOverFlow.Parsing
{
    public static class MessageTemplateHelper
    {
        static MessageTemplateParser _parser = new MessageTemplateParser();

        public static String ParseTemplate(String messageTemplate, IReadOnlyDictionary<String, String> properties)
        {
            MessageTemplate mt = _parser.Parse(messageTemplate);
            return mt.Render(properties);
        }
    }
}
