using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System;

namespace SwagOverFlow.JsonConverters
{
    public class SwagOptionJsonConverter : AbstractJsonConverter<SwagOption>
    {
        public static SwagOptionJsonConverter Instance = new SwagOptionJsonConverter();
        protected override SwagOption Create(Type objectType, JObject jObject)
        {
            String strType = jObject["Type"].ToString();
            Type type = Type.GetType(strType);
            JArray jaChildren = null;

            if (jObject.ContainsKey("Parent"))  //Prevent ReadJson Null error
            {
                jObject.Remove("Parent");
            }

            if (jObject.ContainsKey("Children")) //Children are handled below
            {
                jaChildren = (JArray)jObject["Children"];
                jObject.Remove("Children");
            }

            SwagOption opt = (SwagOption)JsonConvert.DeserializeObject(jObject.ToString(), type);

            switch (opt)
            {
                case SwagOptionGroup grp:
                    foreach (JToken token in jaChildren)
                    {
                        JObject jChild = (JObject)token;
                        SwagOption child = (SwagOption)JsonConvert.DeserializeObject(jChild.ToString(), typeof(SwagOption), SwagOptionJsonConverter.Instance);
                        grp.Children.Add(child);
                    }
                    break;
            }

            return opt;
        }
    }
}
