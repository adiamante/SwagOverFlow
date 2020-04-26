using System;
using System.Windows.Data;

namespace SwagOverflowWPF.UI
{
    public enum LogicalOperator { AND, OR }

    public class MultiBoolToBoolConverter : IMultiValueConverter
    {
        public LogicalOperator LogicalOperator { get; set; }
        public Boolean Default { get; set; }

        public MultiBoolToBoolConverter()
        {
            LogicalOperator = LogicalOperator.AND;
            Default = false;
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Boolean bHasTrue = false;
            Boolean bHasFalse = false;
            foreach (Boolean bVal in values)
            {
                if (bVal)
                {
                    bHasTrue = true;
                }
                else
                {
                    bHasFalse = true;
                }
            }

            switch (LogicalOperator)
            {
                case LogicalOperator.AND:
                    return bHasTrue && !bHasFalse;
                case LogicalOperator.OR:
                    return bHasTrue;
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
