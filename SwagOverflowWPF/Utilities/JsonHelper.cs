﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

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

        public static T ToObject<T>(String str)
        {
            return JsonConvert.DeserializeObject<T>(str, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        }

        public static String ToJsonString(Object obj, IContractResolver contractResolver)
        {
            String jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
                ContractResolver = contractResolver
            });

            return jsonString;
        }
    }
}
