using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SwagOverFlow.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SwagOverFlow.Utils
{
    public static class JsonHelper
    {
        public static IJsonConverterProviderService JsonConverterProviderService { get; set; }

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
            //String jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings()
            //{
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //    TypeNameHandling = TypeNameHandling.All
            //});

            String jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Converters = JsonConverterProviderService.GetConverters().ToList()
            });

            return jsonString;
        }

        public static T ToObject<T>(String str)
        {
            return JsonConvert.DeserializeObject<T>(str, new JsonSerializerSettings() { Converters = JsonConverterProviderService.GetConverters().ToList() });
            //return JsonConvert.DeserializeObject<T>(str, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
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

        //https://stackoverflow.com/questions/24876082/find-and-return-json-differences-using-newtonsoft-in-c
        public static JObject FindDiff(JToken source, JToken target)
        {
            var diff = new JObject();
            if (JToken.DeepEquals(source, target)) return diff;

            switch (source.Type)
            {
                case JTokenType.Object:
                    {
                        var objSource = source as JObject;
                        var objTarget = target as JObject;
                        var addedKeys = objSource.Properties().Select(c => c.Name).Except(objTarget.Properties().Select(c => c.Name));
                        var removedKeys = objTarget.Properties().Select(c => c.Name).Except(objSource.Properties().Select(c => c.Name));
                        var unchangedKeys = objSource.Properties().Where(c => JToken.DeepEquals(c.Value, target[c.Name])).Select(c => c.Name);
                        foreach (var k in addedKeys)
                        {
                            diff[k] = new JObject
                            {
                                ["+"] = source[k]
                            };
                        }
                        foreach (var k in removedKeys)
                        {
                            diff[k] = new JObject
                            {
                                ["-"] = target[k]
                            };
                        }
                        var potentiallyModifiedKeys = objSource.Properties().Select(c => c.Name).Except(addedKeys).Except(unchangedKeys);
                        foreach (var k in potentiallyModifiedKeys)
                        {
                            var foundDiff = FindDiff(objSource[k], objTarget[k]);
                            if (foundDiff.HasValues) diff[k] = foundDiff;
                        }
                    }
                    break;
                case JTokenType.Array:
                    {
                        var arrSource = source as JArray;
                        var arrTarget = target as JArray;
                        var plus = new JArray(arrSource.Except(arrTarget, new JTokenEqualityComparer()));
                        var minus = new JArray(arrTarget.Except(arrSource, new JTokenEqualityComparer()));
                        if (plus.HasValues) diff["+"] = plus;
                        if (minus.HasValues) diff["-"] = minus;
                    }
                    break;
                default:
                    diff["+"] = source;
                    diff["-"] = target;
                    break;
            }

            return diff;
        }

        public static object ToObject(string str, Type type)
        {
            return JsonConvert.DeserializeObject(str, type, new JsonSerializerSettings() { Converters = JsonConverterProviderService.GetConverters().ToList() });
        }
    }

    //http://www.tomdupont.net/2014/04/deserialize-abstract-classes-with.html
    public abstract class AbstractJsonConverter<T> : JsonConverter
    {
        public override bool CanWrite => false;
        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            T target = Create(objectType, jObject);
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        protected static bool FieldExists(
            JObject jObject,
            string name,
            JTokenType type)
        {
            JToken token;
            return jObject.TryGetValue(name, out token) && token.Type == type;
        }
    }
}
