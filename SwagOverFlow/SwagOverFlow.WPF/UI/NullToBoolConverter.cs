using System;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace SwagOverFlow.WPF.UI
{
    public class NullToBoolConverter : MarkupExtension, IValueConverter
    {
        //Key pattern should be a cartesian product of all available public properties
        static ConcurrentDictionary<String, NullToBoolConverter> _converters = new ConcurrentDictionary<string, NullToBoolConverter>();
        public Boolean TrueValue { get; set; } = true;
        public Boolean FalseValue { get; set; } = false;

        public NullToBoolConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = value == null;
            return val ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException();
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
