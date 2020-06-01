using SwagOverFlow.Utils;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace SwagOverFlow.WPF.UI
{
    public class AncestorFindConverter : MarkupExtension, IValueConverter
    {
        //Key pattern should be a cartesian product of all available public properties
        static ConcurrentDictionary<String, AncestorFindConverter> _converters = new ConcurrentDictionary<string, AncestorFindConverter>();
        static Object[] methodInfoKey = new object[] { "TryFindParent", new Type[] { typeof(FrameworkElement) } };
        public String Property { get; set; }

        public AncestorFindConverter()
        {
            Property = "";
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FrameworkElement f = (FrameworkElement)value;
            Type t = (Type)parameter;

            if (f != null && t != null)
            {
                MethodInfo method = ReflectionHelper.MethodInfoCollection[typeof(DependencyObjectHelper)][methodInfoKey];
                MethodInfo generic = method.MakeGenericMethod(new[] { t });
                object parentObj = generic.Invoke(f, new Object[] { f });

                if (Property == "" || parentObj == null)
                {
                    return parentObj;
                }
                else
                {
                    return ReflectionHelper.PropertyInfoCollection[parentObj.GetType()][Property].GetValue(parentObj);
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            String key = Property;
            if (!_converters.ContainsKey(key))
            {
                _converters.TryAdd(key, this);
            }
            return _converters[key];
        }
    }
}
