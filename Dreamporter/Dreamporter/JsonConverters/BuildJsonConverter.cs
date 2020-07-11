using Dreamporter.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.JsonConverters;
using SwagOverFlow.Utils;
using System;

namespace Dreamporter.JsonConverters
{
    public class BuildJsonConverter : AbstractJsonConverter<Build>
    {
        public static BuildJsonConverter Instance = new BuildJsonConverter();

        protected override Build Create(Type objectType, JObject jObject)
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

            //https://stackoverflow.com/questions/13394401/json-net-deserializing-list-gives-duplicate-items
            //ObjectCreationHandling.Replace is for list properties
            Build build = (Build)JsonConvert.DeserializeObject(jObject.ToString(), type, new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace, Converters = new JsonConverter[] { InstructionJsonConverter.Instance, BooleanExpressionJsonConverter.Instance } });

            switch (build)
            {
                case GroupBuild grp:
                    foreach (JToken token in jaChildren)
                    {
                        JObject jChild = (JObject)token;
                        Build child = (Build)JsonConvert.DeserializeObject(jChild.ToString(), typeof(Build), new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace, Converters = new JsonConverter[] { BuildJsonConverter.Instance, InstructionJsonConverter.Instance, BooleanExpressionJsonConverter.Instance } });
                        grp.Children.Add(child);
                    }
                    break;
            }

            return build;
        }
    }
}
