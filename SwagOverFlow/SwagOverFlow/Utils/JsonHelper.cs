using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SwagOverFlow.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SwagOverFlow.Utils
{
    public static class JsonHelper
    {
        static IJsonConverterProviderService _jsonConverterProviderService;

        //Does not look to good architectually but gotta get this working
        public static void ResolveConverterProvider(IJsonConverterProviderService jsonConverterProvider)
        {
            _jsonConverterProviderService = jsonConverterProvider;
        }

        public static void ResolveServices(IServiceProvider serviceProvider)
        {
            _jsonConverterProviderService = (IJsonConverterProviderService)serviceProvider.GetService(typeof(IJsonConverterProviderService));
        }

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
                //ObjectCreationHandling = ObjectCreationHandling.Replace,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Converters = _jsonConverterProviderService.GetConverters().ToList()
            });

            return jsonString;
        }

        public static T ToObject<T>(String str)
        {
            return JsonConvert.DeserializeObject<T>(str, new JsonSerializerSettings() { 
                ObjectCreationHandling = ObjectCreationHandling.Replace, 
                Converters = _jsonConverterProviderService.GetConverters().ToList() 
            });
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
            return JsonConvert.DeserializeObject(str, type, new JsonSerializerSettings() { Converters = _jsonConverterProviderService.GetConverters().ToList() });
        }

        public static void ApplyJson(Object target, JObject jApply)
        {
            Type classType = target.GetType();
            foreach (KeyValuePair<String, JToken> kvpApply in jApply)
            {
                PropertyInfo prop = ReflectionHelper.PropertyInfoCollection[classType][kvpApply.Key]; //classType.GetProperty(kvpApply.Key);

                if (prop != null && prop.GetSetMethod() != null)
                {
                    Type destType = prop.PropertyType;
                    TypeConverter converter = ReflectionHelper.TypeConverterCache[destType]; //TypeDescriptor.GetConverter(destType);

                    if (kvpApply.Value is JValue)
                    {
                        JValue jVal = kvpApply.Value as JValue;

                        if (jVal.Value != null)
                        {
                            Type sourceType = jVal.Value.GetType();

                            if (converter.CanConvertFrom(sourceType))
                            {
                                prop.SetValue(target, converter.ConvertFrom(jVal.Value));
                            }
                            //If source and destination types are numeric, try converting from string
                            else if ((sourceType.IsNumericType() && (destType.IsNumericType() || Nullable.GetUnderlyingType(destType).IsNumericType())) ||
                                      destType.GetTypeCode() == TypeCode.DateTime)
                            {
                                prop.SetValue(target, converter.ConvertFrom(jVal.Value.ToString()));
                            }
                            else if (sourceType == destType)
                            {
                                prop.SetValue(target, jVal.Value);
                            }
                        }
                    }
                    else if (kvpApply.Value is JArray && destType.IsGenericType && destType.GetInterface(nameof(IDictionary)) != null)
                    {
                        var collection = Activator.CreateInstance(destType);
                        JArray jArray = kvpApply.Value as JArray;

                        foreach (JToken jToken in jArray)
                        {
                            if (jToken.Type == JTokenType.Object)
                            {
                                //Just doing String Key and String Value
                                JObject jObject = (JObject)jToken;
                                var key = jObject["Key"].ToString();
                                var value = jObject["Value"].ToString();
                                
                                ReflectionHelper.MethodInfoCollection[collection.GetType()]["Add"].Invoke(collection, new object[] { key, value });
                            }
                        }

                        prop.SetValue(target, collection);
                    }
                    //else if (kvpApply.Value is JArray && destType.IsGenericType && destType.GetInterface(nameof(ICollection)) != null)
                    //{
                    //    //This was made for dictionary and uses JOjbect item fields (in order the given order) in the Add method of the Collection no clue if it works for other generics
                    //    var collection = Activator.CreateInstance(destType);
                    //    JArray jArray = kvpApply.Value as JArray;

                    //    foreach (JToken jToken in jArray)
                    //    {
                    //        if (jToken.Type == JTokenType.Object)
                    //        {
                    //            JObject jObject = (JObject)jToken;
                    //            List<Object> parameters = new List<Object>();
                    //            foreach (KeyValuePair<String, JToken> itemKvp in jObject)
                    //            {
                    //                if (itemKvp.Value is JValue)
                    //                {
                    //                    JValue val = (JValue)itemKvp.Value;
                    //                    parameters.Add(val.Value);
                    //                }
                    //            }
                    //            collection.GetType().GetMethod("Add").Invoke(collection, parameters.ToArray());
                    //        }
                    //    }

                    //    prop.SetValue(target, collection);
                    //}
                    //else if (kvpApply.Value is JArray && destType.IsGenericType)
                    //{
                    //    JArray jArray = kvpApply.Value as JArray;
                    //    Type listGenericType = typeof(List<>);
                    //    Type listType = listGenericType.MakeGenericType(destType.GenericTypeArguments);

                    //    var genericCollection = JsonConvert.DeserializeObject(jArray.ToString(), listType);
                    //    prop.SetValue(target, genericCollection);
                    //}
                    //else if (kvpApply.Value is JArray)
                    //{
                    //    JArray jArray = kvpApply.Value as JArray;

                    //    var genericCollection = JsonConvert.DeserializeObject(jArray.ToString(), destType);
                    //    prop.SetValue(target, genericCollection);
                    //}
                    //else if (kvpApply.Value is JObject && !destType.IsAbstract && !destType.IsInterface)
                    //{
                    //    var destClass = JsonConvert.DeserializeObject(kvpApply.Value.ToString(), destType);
                    //    prop.SetValue(target, destClass);
                    //}                    //else if (kvpApply.Value is JArray && destType.IsGenericType && destType.GetInterface(nameof(ICollection)) != null)
                    //{
                    //    //This was made for dictionary and uses JOjbect item fields (in order the given order) in the Add method of the Collection no clue if it works for other generics
                    //    var collection = Activator.CreateInstance(destType);
                    //    JArray jArray = kvpApply.Value as JArray;

                    //    foreach (JToken jToken in jArray)
                    //    {
                    //        if (jToken.Type == JTokenType.Object)
                    //        {
                    //            JObject jObject = (JObject)jToken;
                    //            List<Object> parameters = new List<Object>();
                    //            foreach (KeyValuePair<String, JToken> itemKvp in jObject)
                    //            {
                    //                if (itemKvp.Value is JValue)
                    //                {
                    //                    JValue val = (JValue)itemKvp.Value;
                    //                    parameters.Add(val.Value);
                    //                }
                    //            }
                    //            collection.GetType().GetMethod("Add").Invoke(collection, parameters.ToArray());
                    //        }
                    //    }

                    //    prop.SetValue(target, collection);
                    //}
                    //else if (kvpApply.Value is JArray && destType.IsGenericType)
                    //{
                    //    JArray jArray = kvpApply.Value as JArray;
                    //    Type listGenericType = typeof(List<>);
                    //    Type listType = listGenericType.MakeGenericType(destType.GenericTypeArguments);

                    //    var genericCollection = JsonConvert.DeserializeObject(jArray.ToString(), listType);
                    //    prop.SetValue(target, genericCollection);
                    //}
                    //else if (kvpApply.Value is JArray)
                    //{
                    //    JArray jArray = kvpApply.Value as JArray;

                    //    var genericCollection = JsonConvert.DeserializeObject(jArray.ToString(), destType);
                    //    prop.SetValue(target, genericCollection);
                    //}
                    //else if (kvpApply.Value is JObject && !destType.IsAbstract && !destType.IsInterface)
                    //{
                    //    var destClass = JsonConvert.DeserializeObject(kvpApply.Value.ToString(), destType);
                    //    prop.SetValue(target, destClass);
                    //}
                }
            }
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
            //https://stackoverflow.com/questions/34185295/handling-null-objects-in-custom-jsonconverters-readjson-method
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

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

    public abstract class AbstractJsonArrayConverter<T> : JsonConverter
    {
        public override bool CanWrite => false;
        protected abstract T Create(Type objectType, JArray jObject);

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
            //https://stackoverflow.com/questions/34185295/handling-null-objects-in-custom-jsonconverters-readjson-method
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jObject = JArray.Load(reader);

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
