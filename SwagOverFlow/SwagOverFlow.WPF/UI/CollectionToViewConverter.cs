using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace SwagOverFlow.WPF.UI
{
    public class CollectionToViewConverter : MarkupExtension, IValueConverter
    {
        //Key pattern should be a cartesian product of all available public properties
        static ConcurrentDictionary<String, CollectionToViewConverter> _converters = new ConcurrentDictionary<string, CollectionToViewConverter>();
        public String Sort { get; set; }

        public CollectionToViewConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IEnumerable col)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(col);
                SortDescription sort = new SortDescription(Sort, ListSortDirection.Ascending);
                if (!view.SortDescriptions.Contains(sort))
                {
                    view.SortDescriptions.Add(sort);
                }
                
                return view;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var anonKey = new { Sort = Sort };
            String key = anonKey.ToString();
            if (!_converters.ContainsKey(key))
            {
                _converters.TryAdd(key, this);
            }
            return _converters[key];
        }
    }
}
