using System;
using SwagOverflowWPF.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using SwagOverflowWPF.Collections;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Linq;
using System.Windows.Threading;

namespace SwagOverflowWPF.Controls
{
    /// <summary>
    /// Interaction logic for SwagDataGrid.xaml
    /// </summary>
    public partial class SwagDataGrid : SwagControlBase
    {
        #region SwagDataTable
        public static DependencyProperty SwagDataTableProperty =
                DependencyProperty.Register(
                    "SwagDataTable",
                    typeof(SwagDataTable),
                    typeof(SwagDataGrid));

        public SwagDataTable SwagDataTable
        {
            get { return (SwagDataTable)GetValue(SwagDataTableProperty); }
            set
            {
                SetValue(SwagDataTableProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion SwagDataTable

        #region Columns
        public static DependencyProperty ColumnsProperty =
        DependencyProperty.RegisterAttached("Columns",
                                            typeof(ConcurrentObservableOrderedDictionary<String, SwagDataColumn>),
                                            typeof(SwagDataGrid),
                                            new UIPropertyMetadata(null, BindableColumnsPropertyChanged));

        private static void BindableColumnsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            SwagDataGrid fdgDataGrid = source as SwagDataGrid;
            DataGrid dataGrid = fdgDataGrid.dgGrid;

            ConcurrentObservableOrderedDictionary<String, SwagDataColumn> columns = e.NewValue as ConcurrentObservableOrderedDictionary<String, SwagDataColumn>;

            dataGrid.Dispatcher.Invoke(new Action(() =>
            {
                dataGrid.Columns.Clear();
                if (columns == null)
                {
                    return;
                }

                foreach (KeyValuePair<String, SwagDataColumn> sdcKvp in columns)
                {
                    dataGrid.Columns.Add(sdcKvp.Value.DataGridColumn);
                }

                columns.CollectionChanged += (sender, e2) =>
                {
                    NotifyCollectionChangedEventArgs ne = e2 as NotifyCollectionChangedEventArgs;

                    switch (ne.Action)
                    {
                        case NotifyCollectionChangedAction.Reset:
                            dataGrid.Columns.Clear();
                            foreach (KeyValuePair<String, SwagDataColumn> kvp in ne.NewItems)
                            {
                                dataGrid.Columns.Add(kvp.Value.DataGridColumn);
                            }
                            break;
                        case NotifyCollectionChangedAction.Add:
                            foreach (KeyValuePair<String, SwagDataColumn> kvp in ne.NewItems)
                            {
                                dataGrid.Columns.Insert(ne.NewStartingIndex, kvp.Value.DataGridColumn);
                            }
                            break;
                        case NotifyCollectionChangedAction.Move:
                            foreach (KeyValuePair<String, SwagDataColumn> kvp in ne.NewItems)
                            {
                                dataGrid.Columns.RemoveAt(ne.OldStartingIndex);
                                dataGrid.Columns.Insert(ne.NewStartingIndex, kvp.Value.DataGridColumn);
                            }
                            //dataGrid.Columns.Move(ne.OldStartingIndex, ne.NewStartingIndex);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (KeyValuePair<String, SwagDataColumn> kvp in ne.OldItems)
                            {
                                dataGrid.Columns.RemoveAt(ne.OldStartingIndex);
                            }
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            dataGrid.Columns[ne.NewStartingIndex] = ne.NewItems[0] as DataGridColumn;
                            break;
                    }
                };
            }));
        }

        public ConcurrentObservableOrderedDictionary<String, SwagDataColumn> Columns
        {
            get { return (ConcurrentObservableOrderedDictionary<String, SwagDataColumn>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        #endregion Columns

        public SwagDataGrid()
        {
            InitializeComponent();

            BindingOperations.SetBinding(this, SwagDataGrid.ColumnsProperty, new Binding("SwagDataTable.Columns") { RelativeSource = RelativeSource.Self });
        }
    }
}
