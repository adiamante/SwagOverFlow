using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dreamporter.Core;
using SwagOverFlow.Utils;

namespace Dreamporter.JsonConverters
{
    public class DataContextJsonConverter : AbstractJsonConverter<DataContext>
    {
        public static DataContextJsonConverter Instance = new DataContextJsonConverter();
        protected override DataContext Create(Type objectType, JObject jObject)
        {
            String strType = jObject["Type"].ToString();
            Type type = Type.GetType(strType);
            DataContext dc = (DataContext)JsonConvert.DeserializeObject(jObject.ToString(), type);
            return dc;
        }
    }
}
