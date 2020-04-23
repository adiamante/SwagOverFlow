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
using MahApps.Metro.Controls;
using SwagOverflowWPF.UI;
using SwagOverFlow.Utils;
using System.Windows.Input;
using System.Data;

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
                                            typeof(ICollectionView),
                                            typeof(SwagDataGrid),
                                            new UIPropertyMetadata(null, BindableColumnsPropertyChanged));

        private static void BindableColumnsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            SwagDataGrid fdgDataGrid = source as SwagDataGrid;
            DataGrid dataGrid = fdgDataGrid.DataGrid;

            if (dataGrid != null)
            {
                ICollectionView columns = e.NewValue as ICollectionView;
                
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
                                foreach (KeyValuePair<String, SwagDataColumn> kvp in columns)
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
        }
        #endregion Columns

        public SwagDataGrid()
        {
            InitializeComponent();

            BindingOperations.SetBinding(this, SwagDataGrid.ColumnsProperty, new Binding("SwagDataTable.ColumnsView") { RelativeSource = RelativeSource.Self });
            //BindingOperations.SetBinding(this, SwagDataGrid.ColumnsProperty, new Binding("SwagDataTable.Columns") { RelativeSource = RelativeSource.Self });
        }

        private void SwagDataGridInstance_Loaded(object sender, RoutedEventArgs e)
        {
            SwagDataTable.InitSettings();
            SwagDataTable.InitTabs();
        }

        private void Search_OnSearch(object sender, RoutedEventArgs e)
        {
            SearchTextBox searchTextBox = (SearchTextBox)sender;
            SwagData swagData = (SwagData)((SwagTabItem)searchTextBox.DataContext).ViewModel;
            SwagDataResult swagDataResult = SwagDataTable.Search(searchTextBox.Text, searchTextBox.FilterMode,
                (sdc, sdr, searchValue, filterMode) => 
                {
                    String compareTarget = sdr.DataRow[sdc.ColumnName].ToString();
                    String compareValue = searchValue;
                    return SearchHelper.Evaluate(compareTarget, compareValue, false, filterMode, false);
                });

            swagData.SwagDataResult = swagDataResult;
        }

        private void Search_ResultGo_Click(object sender, RoutedEventArgs e)
        {
            SwagDataResult swagDataResult = (SwagDataResult)((MenuItem)sender).DataContext;
            SwagDataResult currentResult = swagDataResult;
            DataRowView drv = null;
            SwagDataColumn column = null;
            SwagDataTable table = null;

            switch (currentResult)
            {
                case SwagDataColumnResultGroup columnResult:
                    column = (SwagDataColumn)columnResult.SwagData;
                    break;
                case SwagDataRowResult rowResult:
                    SwagDataRow row = (SwagDataRow)rowResult.SwagData;
                    column = (SwagDataColumn)rowResult.Parent.SwagData;
                    drv = column.SwagDataTable.DataTable.DefaultView[column.SwagDataTable.DataTable.Rows.IndexOf(row.DataRow)];
                    break;
            }

            DataGridColumn dataGridColumn = DataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == column.ColumnName);
            table = column.SwagDataTable;
            
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (drv == null)        //Just column
                {
                    DataGrid.ScrollIntoView(null, dataGridColumn);
                    foreach (KeyValuePair<String, SwagDataColumn> kvp in table.Columns)
                    {
                        kvp.Value.IsSelected = false;
                    }
                    column.IsSelected = true;
                }
                else
                {
                    DataGridCellInfo cellInfo = new DataGridCellInfo(drv, dataGridColumn);
                    DataGrid.ScrollIntoView(cellInfo.Item, cellInfo.Column);
                    DataGrid.SelectedCells.Clear();
                    DataGrid.SelectedCells.Add(cellInfo);
                    DataGrid.CurrentCell = cellInfo;
                }
            }));
        }

        private void Import_Paste_Click(object sender, RoutedEventArgs e)
        {
            SwagDataTable.Settings["Import"]["Type"].SetValue<SwagTableImportType>(SwagTableImportType.Tsv);
            SwagDataTable.Settings["Import"]["Source"].SetValue<SwagTableSourceType>(SwagTableSourceType.Clipboard);
            SwagDataTable.ImportCommand.Execute(null);
        }
    }
}
