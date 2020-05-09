using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace SwagOverFlow.WPF.UI
{
    public class EnumMatchToBooleanConverter : MarkupExtension, IValueConverter
    {
        public Boolean TrueValue { get; set; }
        public Boolean FalseValue { get; set; }

        public EnumMatchToBooleanConverter()
        {
            TrueValue = true;
            FalseValue = false;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = Enum.Equals(value, parameter);
            return val ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return TrueValue.Equals(value) ? true : false;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
