using System;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace SwagOverFlow.WPF.UI
{
    public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
    {
        //Key pattern should be a cartesian product of all available public properties
        static ConcurrentDictionary<String, BoolToVisibilityConverter> _converters = new ConcurrentDictionary<string, BoolToVisibilityConverter>();
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public BoolToVisibilityConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = System.Convert.ToBoolean(value);
            return val ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return TrueValue.Equals(value) ? true : false;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var anonKey = new { TrueValue = TrueValue, FalseValue = FalseValue };
            String key = anonKey.ToString();
            if (!_converters.ContainsKey(key))
            {
                _converters.TryAdd(key, this);
            }
            return _converters[key];
        }
    }
}
