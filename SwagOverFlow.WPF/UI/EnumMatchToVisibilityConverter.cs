using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace SwagOverFlow.WPF.UI
{
    public class EnumMatchToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public EnumMatchToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = Enum.Equals(value, parameter);
            return val ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return Enum.Equals(value, parameter) ? true : false;
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
