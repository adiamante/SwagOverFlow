using Newtonsoft.Json.Linq;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverFlow.JsonConverters
{
    public class SwagOptionListJsonConverter : AbstractJsonArrayConverter<List<SwagOption>>
    {
        public static SwagOptionListJsonConverter Instance = new SwagOptionListJsonConverter();

        protected override List<SwagOption> Create(Type objectType, JArray jArray)
        {
            List<SwagOption> options = new List<SwagOption>();
            return options;
        }
    }
}
