using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System;

namespace SwagOverFlow.JsonConverters
{
    public class BooleanExpressionJsonConverter : AbstractJsonConverter<BooleanExpression>
    {
        public static BooleanExpressionJsonConverter Instance = new BooleanExpressionJsonConverter();

        protected override BooleanExpression Create(Type objectType, JObject jObject)
        {
            String strType = jObject["Type"].ToString();
            Type type = Type.GetType(strType);
            JObject jRoot = null;
            JArray jaChildren = null; 

            if (jObject.ContainsKey("Parent"))  //Prevent ReadJson Null error
            {
                jObject.Remove("Parent");
            }

            if (jObject.ContainsKey("Root"))  //Prevent ReadJson Null error
            {
                if (jObject["Root"] is JObject r)
                {
                    jRoot = r;
                }
                jObject.Remove("Root");
            }

            if (jObject.ContainsKey("Children")) //Children are handled below
            {
                jaChildren = (JArray)jObject["Children"];
                jObject.Remove("Children");
            }

            BooleanExpression exp = (BooleanExpression)JsonConvert.DeserializeObject(jObject.ToString(), type);

            switch (exp)
            {
                case BooleanContainerExpression cnt:
                    if (jRoot != null)
                    {
                        BooleanExpression root = (BooleanExpression)JsonConvert.DeserializeObject(jRoot.ToString(), typeof(BooleanExpression), BooleanExpressionJsonConverter.Instance);
                        cnt.Root = root;
                    }
                    break;
                case BooleanOperationExpression op:
                    foreach (JToken token in jaChildren)
                    {
                        JObject jChild = (JObject)token;
                        BooleanExpression child = (BooleanExpression)JsonConvert.DeserializeObject(jChild.ToString(), typeof(BooleanExpression), BooleanExpressionJsonConverter.Instance);
                        op.Children.Add(child);
                    }
                    break;
            }

            return exp;
        }
    }
}
