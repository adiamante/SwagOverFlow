using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SwagOverflowWPF.Utilities
{
    public static class JsonHelper
    {
        public static T Clone<T>(T target)
        {
            String targetJsonString = JsonConvert.SerializeObject(target, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All
            });

            return JsonConvert.DeserializeObject<T>(targetJsonString, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        }

        public static String ToJsonString(Object obj)
        {
            String jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All
            });

            return jsonString;
        }
    }
}
