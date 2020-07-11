using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Dreamporter.Core;
using SwagOverFlow.Utils;

namespace Dreamporter.JsonConverters
{
    public class DataContextListJsonConverter : AbstractJsonArrayConverter<List<DataContext>>
    {
        public static DataContextListJsonConverter Instance = new DataContextListJsonConverter();

        protected override List<DataContext> Create(Type objectType, JArray jArray)
        {
            List<DataContext> dataContexts = new List<DataContext>();
            return dataContexts;
        }
    }
}
