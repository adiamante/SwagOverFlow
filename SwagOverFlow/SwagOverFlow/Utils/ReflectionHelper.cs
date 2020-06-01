using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace SwagOverFlow.Utils
{
    public static class ReflectionHelper
    {
        private static FieldInfoCollection _fieldInfoCollection = new FieldInfoCollection();
        private static PropertyInfoCollection _propertyInfoCollection = new PropertyInfoCollection();
        private static MethodInfoCollection _methodInfoCollection = new MethodInfoCollection();
        private static TypeConverterCache _typeConverterCache = new TypeConverterCache();
        //Usage: ReflectionHelper.FieldInfoCollection[type][field] instead of type.GetField(field)
        public static FieldInfoCollection FieldInfoCollection => _fieldInfoCollection;
        //Usage: ReflectionHelper.PropertyInfoCollection[type][property] instead of type.GetProperty(property)  
        public static PropertyInfoCollection PropertyInfoCollection => _propertyInfoCollection;
        //Usage: ReflectionHelper.MethodInfoCollection[type][method] instead of type.GetMethod(Method)
        //To properly cache MethodInfo, method subKey needs to be the same reference (this key is of type object; currently only hanling string and object[2])
        public static MethodInfoCollection MethodInfoCollection => _methodInfoCollection;
        //Usage: ReflectionHelper.TypeConverterCache[type] instead of TypeDescriptor.GetConverter(type)
        public static TypeConverterCache TypeConverterCache => _typeConverterCache;
    }

    #region TypeConverterCache
    public class TypeConverterCache : ReflectionCache<Type, TypeConverter>
    {
        public override TypeConverter this[Type type]
        {
            get
            {
                if (!_cache.ContainsKey(type))
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    _cache.TryAdd(type, converter);
                }

                return _cache[type];
            }
        }
    }
    #endregion TypeConverterCache

    #region FieldInfoCollection
    public class FieldInfoCollection : ReflectionCache<Type, iReflectionCache<String, FieldInfo>>
    {
        public override iReflectionCache<String, FieldInfo> this[Type type]
        {
            get
            {
                if (!_cache.ContainsKey(type))
                {
                    Type genericTemplate = typeof(FieldInfoCache<>);
                    Type[] typeArgs = { type };
                    Type genericType = genericTemplate.MakeGenericType(typeArgs);
                    iReflectionCache<String, FieldInfo> subCache = (iReflectionCache<String, FieldInfo>)Activator.CreateInstance(genericType);
                    _cache.TryAdd(type, subCache);
                }

                return _cache[type];
            }
        }
    }
    #endregion FieldInfoCollection

    #region FieldInfoCache<T>
    public class FieldInfoCache<T> : ReflectionCache<String, FieldInfo>
    {
        public override FieldInfo this[string field]
        {
            get
            {
                if (!_cache.ContainsKey(field))
                {
                    FieldInfo fieldInfo = typeof(T).GetField(field);
                    _cache.TryAdd(field, fieldInfo);
                }
                return _cache[field];
            }
        }
    }
    #endregion FieldInfoCache<T>

    #region iReflectionCache
    public interface iReflectionCache<TKey, TValue> : IEnumerable<TValue>
    {
        TValue this[TKey key] { get; }
        Boolean IsLoaded { get; }
        void Load();
    }
    #endregion iReflectionCache

    #region ReflectionCache
    public abstract class ReflectionCache<TKey, TValue> : iReflectionCache<TKey, TValue>
    {
        protected ConcurrentDictionary<TKey, TValue> _cache = new ConcurrentDictionary<TKey, TValue>();
        public Boolean IsLoaded { get; protected set; } = false;

        public abstract TValue this[TKey key] { get; }

        #region Initialization
        public virtual void Load()
        {
        }
        #endregion Initialization

        #region IEnumerator
        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (KeyValuePair<TKey, TValue> kvp in _cache)
            {
                yield return kvp.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion IEnumerator
    }
    #endregion ReflectionCache

    #region PropertyInfoCollection
    public class PropertyInfoCollection : ReflectionCache<Type, iReflectionCache<String, PropertyInfo>>
    {
        public override iReflectionCache<String, PropertyInfo> this[Type type]
        {
            get
            {
                if (!_cache.ContainsKey(type))
                {
                    Type genericTemplate = typeof(PropertyInfoCache<>);
                    Type[] typeArgs = { type };
                    Type genericType = genericTemplate.MakeGenericType(typeArgs);
                    iReflectionCache<String, PropertyInfo> subCache = (iReflectionCache<String, PropertyInfo>)Activator.CreateInstance(genericType);
                    _cache.TryAdd(type, subCache);
                }

                return _cache[type];
            }
        }
    }
    #endregion PropertyInfoCollection

    #region PropertyInfoCache<T>
    public class PropertyInfoCache<T> : ReflectionCache<String, PropertyInfo>
    {
        public override PropertyInfo this[string key]
        {
            get
            {
                if (!_cache.ContainsKey(key))
                {
                    PropertyInfo propertyInfo = typeof(T).GetProperty(key);
                    _cache.TryAdd(key, propertyInfo);
                }
                return _cache[key];
            }
        }

        public override void Load()
        {
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!_cache.ContainsKey(propertyInfo.Name))
                {
                    _cache.TryAdd(propertyInfo.Name, propertyInfo);
                }
            }
            IsLoaded = true;
        }
    }
    #endregion PropertyInfoCache<T>

    #region MethodInfoCollection
    public class MethodInfoCollection : ReflectionCache<Type, iReflectionCache<Object, MethodInfo>>
    {
        public override iReflectionCache<Object, MethodInfo> this[Type type]
        {
            get
            {
                if (!_cache.ContainsKey(type))
                {
                    Type genericTemplate = typeof(MethodInfoCache<>);
                    Type[] typeArgs = { type };
                    Type genericType = genericTemplate.MakeGenericType(typeArgs);
                    iReflectionCache<Object, MethodInfo> subCache = (iReflectionCache<Object, MethodInfo>)Activator.CreateInstance(genericType);
                    _cache.TryAdd(type, subCache);
                }

                return _cache[type];
            }
        }
    }
    #endregion MethodInfoCollection

    #region MethodInfoCache<T>
    public class MethodInfoCache<T> : ReflectionCache<object, MethodInfo>
    {
        public override MethodInfo this[object objKey]
        {
            get
            {
                if (!_cache.ContainsKey(objKey))
                {
                    MethodInfo methodInfo = null;

                    switch (objKey)
                    {
                        case String strKey:
                            methodInfo = typeof(T).GetMethod(strKey);
                            break;
                        case Object [] arr:
                            if (arr.Length == 2)
                            {
                                String methodName = arr[0].ToString();
                                Type[] types = (Type[])arr[1];
                                methodInfo = typeof(T).GetMethod(methodName, types);
                            }
                            break;
                    }

                    _cache.TryAdd(objKey, methodInfo);
                }
                return _cache[objKey];
            }
        }
    }
    #endregion MethodInfoCache<T>
}
