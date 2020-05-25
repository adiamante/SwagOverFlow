using System;
using System.Windows;
using System.Windows.Data;

namespace SwagOverFlow.WPF.UI
{
    public class MultiBoolToVisibilityConverter : IMultiValueConverter
    {
        public LogicalOperator LogicalOperator { get; set; }
        public Visibility Default { get; set; }
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public MultiBoolToVisibilityConverter()
        {
            LogicalOperator = LogicalOperator.AND;
            Default = TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Boolean bHasTrue = false;
            Boolean bHasFalse = false;

            foreach (object val in values)
            {
                if (val is Boolean)
                {
                    Boolean bVal = (Boolean)val;
                    if (bVal)
                    {
                        bHasTrue = true;
                    }
                    else
                    {
                        bHasFalse = true;
                    }
                }
                else //if any value is not a boolean return default value
                {
                    return Default;
                }
            }

            switch (LogicalOperator)
            {
                case LogicalOperator.AND:
                    return bHasTrue && !bHasFalse ? TrueValue : FalseValue;
                case LogicalOperator.OR:
                    return bHasTrue ? TrueValue : FalseValue;
                default:
                    return Default;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
