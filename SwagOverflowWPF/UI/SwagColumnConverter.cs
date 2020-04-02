using SwagOverflowWPF.Controls;
using System;
using System.Windows.Data;

namespace SwagOverflowWPF.UI
{

    public class SwagColumnConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is SwagDataGrid && values[1] is String)
            {
                SwagDataGrid sdg = values[0] as SwagDataGrid;
                String colName = values[1].ToString();

                return sdg.Columns[colName];
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
