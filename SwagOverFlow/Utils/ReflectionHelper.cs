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
        private static TypeConverterCache _typeConverterCache = new TypeConverterCache();
        //Usage: ReflectionHelper.FieldInfoCollection[type][field] instead of type.GetField(field)
        public static FieldInfoCollection FieldInfoCollection => _fieldInfoCollection;
        //Usage: ReflectionHelper.PropertyInfoCollection[type][property] instead of type.GetProperty(property)  
        public static PropertyInfoCollection PropertyInfoCollection => _propertyInfoCollection;
        //Usage: ReflectionHelper.TypeConverterCache[type] instead of TypeDescriptor.GetConverter(type)
        public static TypeConverterCache TypeConverterCache => _typeConverterCache;
    }

    public class TypeConverterCache
    {
        private ConcurrentDictionary<Type, TypeConverter> _typeConverters = new ConcurrentDictionary<Type, TypeConverter>();

        public TypeConverter this[Type type]
        {
            get
            {
                if (!_typeConverters.ContainsKey(type))
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    _typeConverters.TryAdd(type, converter);
                }

                return _typeConverters[type];
            }
        }
    }

    public class FieldInfoCollection
    {
        private ConcurrentDictionary<Type, iFieldInfoCache> _fieldsInfoCaches = new ConcurrentDictionary<Type, iFieldInfoCache>();

        public iFieldInfoCache this[Type type]
        {
            get
            {
                if (!_fieldsInfoCaches.ContainsKey(type))
                {
                    Type genericTemplate = typeof(FieldInfoCache<>);
                    Type[] typeArgs = { type };
                    Type genericType = genericTemplate.MakeGenericType(typeArgs);
                    iFieldInfoCache cache = (iFieldInfoCache)Activator.CreateInstance(genericType);
                    _fieldsInfoCaches.TryAdd(type, cache);
                }

                return _fieldsInfoCaches[type];
            }
        }
    }

    public interface iFieldInfoCache : IEnumerable<FieldInfo>
    {
        FieldInfo this[string field] { get; }
        Boolean IsLoaded { get; }
        void Load();
    }

    public class FieldInfoCache<T> : iFieldInfoCache
    {
        public Boolean IsLoaded { get; private set; } = false;
        private ConcurrentDictionary<String, FieldInfo> _fieldInfos = new ConcurrentDictionary<string, FieldInfo>();
        public FieldInfo this[string field]
        {
            get
            {
                if (!_fieldInfos.ContainsKey(field))
                {
                    FieldInfo fieldInfo = typeof(T).GetField(field);
                    _fieldInfos.TryAdd(field, fieldInfo);
                }
                return _fieldInfos[field];
            }
        }

        public void Load()
        {
            foreach (FieldInfo fieldInfo in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!_fieldInfos.ContainsKey(fieldInfo.Name))
                {
                    _fieldInfos.TryAdd(fieldInfo.Name, fieldInfo);
                }
            }
            IsLoaded = true;
        }

        public IEnumerator<FieldInfo> GetEnumerator()
        {
            foreach (KeyValuePair<String, FieldInfo> kvp in _fieldInfos)
            {
                yield return kvp.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class PropertyInfoCollection
    {
        private ConcurrentDictionary<Type, iPropertyInfoCache> _propertysInfoCaches = new ConcurrentDictionary<Type, iPropertyInfoCache>();

        public iPropertyInfoCache this[Type type]
        {
            get
            {
                if (!_propertysInfoCaches.ContainsKey(type))
                {
                    Type genericTemplate = typeof(PropertyInfoCache<>);
                    Type[] typeArgs = { type };
                    Type genericType = genericTemplate.MakeGenericType(typeArgs);
                    iPropertyInfoCache cache = (iPropertyInfoCache)Activator.CreateInstance(genericType);
                    _propertysInfoCaches.TryAdd(type, cache);
                }

                return _propertysInfoCaches[type];
            }
        }
    }

    public interface iPropertyInfoCache : IEnumerable<PropertyInfo>
    {
        PropertyInfo this[string field] { get; }
        Boolean IsLoaded { get; }
        void Load();
    }

    public class PropertyInfoCache<T> : iPropertyInfoCache
    {
        public Boolean IsLoaded { get; private set; } = false;

        private ConcurrentDictionary<String, PropertyInfo> _propertyInfos = new ConcurrentDictionary<string, PropertyInfo>();
        public PropertyInfo this[string property]
        {
            get
            {
                if (!_propertyInfos.ContainsKey(property))
                {
                    PropertyInfo propertyInfo = typeof(T).GetProperty(property);
                    _propertyInfos.TryAdd(property, propertyInfo);
                }
                return _propertyInfos[property];
            }
        }

        public void Load()
        {
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!_propertyInfos.ContainsKey(propertyInfo.Name))
                {
                    _propertyInfos.TryAdd(propertyInfo.Name, propertyInfo);
                }
            }
            IsLoaded = true;
        }

        public IEnumerator<PropertyInfo> GetEnumerator()
        {
            foreach (KeyValuePair<String, PropertyInfo> kvp in _propertyInfos)
            {
                yield return kvp.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
