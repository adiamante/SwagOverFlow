using SwagOverFlow.WPF.Controls;
using System;
using System.Windows.Data;

namespace SwagOverFlow.WPF.UI
{

    public class SwagColumnConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is SwagDataGrid && values[1] is String)
            {
                SwagDataGrid sdg = values[0] as SwagDataGrid;
                String colName = values[1].ToString();

                return sdg.SwagDataTable.Columns[colName];
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
