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
using System.Diagnostics;
using SwagOverFlow.Logger;
using System.Windows.Controls.Primitives;

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

                    if (e.OldValue == null)
                    {
                        columns.CollectionChanged += (sender, e2) =>
                        {
                            NotifyCollectionChangedEventArgs ne = e2 as NotifyCollectionChangedEventArgs;

                            switch (ne.Action)
                            {
                                case NotifyCollectionChangedAction.Reset:
                                case NotifyCollectionChangedAction.Replace:
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
                            }
                        };
                    }
                }));
            }
        }
        #endregion Columns

        public SwagDataGrid()
        {
            InitializeComponent();

            BindingOperations.SetBinding(this, SwagDataGrid.ColumnsProperty, new Binding("SwagDataTable.ColumnsView") { RelativeSource = RelativeSource.Self });
        }

        private void SwagDataGridInstance_Loaded(object sender, RoutedEventArgs e)
        {
            SwagDataTable?.InitSettings();
            SwagDataTable?.InitTabs();
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
            

            switch (currentResult)
            {
                case SwagDataColumnResultGroup columnResult:
                    View((SwagDataColumn)columnResult.SwagData);
                    break;
                case SwagDataRowResult rowResult:
                    SwagDataColumn swagDataColumn = (SwagDataColumn)rowResult.Parent.SwagData;
                    SwagDataRow swagDataRow = (SwagDataRow)rowResult.SwagData;
                    View(swagDataColumn, swagDataRow);
                    break;
            }
        }

        private void View(SwagDataColumn swagDataColumn)
        {
            DataGridColumn dataGridColumn = DataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == swagDataColumn.ColumnName);
            SwagDataTable swagDataTable = swagDataColumn.SwagDataTable;

            DataGrid.ScrollIntoView(null, dataGridColumn);
            foreach (KeyValuePair<String, SwagDataColumn> kvp in swagDataTable.Columns)
            {
                kvp.Value.IsSelected = false;
            }
            swagDataColumn.IsSelected = true;
        }

        private void View(SwagDataColumn swagDataColumn, SwagDataRow swagDataRow)
        {
            DataRowView drv = drv = swagDataColumn.SwagDataTable.DataTable.DefaultView[swagDataColumn.SwagDataTable.DataTable.Rows.IndexOf(swagDataRow.DataRow)]; ;
            SwagDataTable swagDataTable = swagDataColumn.SwagDataTable;
            DataGridColumn dataGridColumn = DataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == swagDataColumn.ColumnName);
            swagDataTable = swagDataColumn.SwagDataTable;

            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                DataGridCellInfo cellInfo = new DataGridCellInfo(drv, dataGridColumn);
                DataGrid.ScrollIntoView(cellInfo.Item, cellInfo.Column);
                DataGrid.SelectedCells.Clear();
                DataGrid.SelectedCells.Add(cellInfo);
                DataGrid.CurrentCell = cellInfo;
            }));
        }

        private void Import_Paste_Click(object sender, RoutedEventArgs e)
        {
            SwagDataTable.Settings["Import"]["Type"].SetValue<SwagTableImportType>(SwagTableImportType.Tsv);
            SwagDataTable.Settings["Import"]["Source"].SetValue<SwagTableSourceType>(SwagTableSourceType.Clipboard);
            SwagDataTable.ImportCommand.Execute(null);
        }

        private void SwagDataColumn_ViewClick(object sender, RoutedEventArgs e)
        {
            SwagDataColumn swagDataColumn = ((KeyValuePair<String, SwagDataColumn>)((MenuItem)sender).DataContext).Value;
            View(swagDataColumn);
        }

        private void SwagColumnHeader_ConvertClick(object sender, RoutedEventArgs e)
        {
            FlogDetail flogDetail = GetFlogDetail("Convert Column", null);
            flogDetail.StartTimer();

            MenuItem menuItem = (MenuItem)sender;
            MenuItem miParent = (MenuItem)menuItem.Parent;
            SwagDataColumn originalSwagDataColumn = (SwagDataColumn)menuItem.DataContext;
            SwagDataTable swagDataTable = originalSwagDataColumn.SwagDataTable;
            Type targetType = (Type)menuItem.Tag;
            Grid grid = miParent.FindLogicalChild<Grid>("gridConvertOptions");
            Boolean keepOriginal = grid.FindVisualChild<CheckBox>().IsChecked ?? false;
            String defaultValueText = grid.FindVisualChild<TextBox>().Text;
            DataTable dt = originalSwagDataColumn.SwagDataTable.DataTable;
            String newColName = $"{originalSwagDataColumn.ColumnName}{targetType.Name}";
            swagDataTable.DelaySave = true;

            Int32 count = 1;
            while (dt.Columns.Contains(newColName))
            {
                newColName = $"{originalSwagDataColumn.ColumnName}{count}{targetType.Name}";
                count++;
            }

            SwagDataColumn newSwagDataColumn = new SwagDataColumn() { ColumnName = newColName, DataType = targetType };
            swagDataTable.Columns.Add(newSwagDataColumn.ColumnName, newSwagDataColumn);

            #region Resolve defaultValue
            Object defaultValue = DBNull.Value;
            if (targetType.GetTypeCode() != TypeCode.String)
            {
                try
                {
                    defaultValue = Convert.ChangeType(defaultValueText, targetType);
                }
                catch
                {
                    defaultValue = DBNull.Value;
                }
            }
            else
            {
                defaultValue = defaultValueText;
            }
            #endregion Resolve defaultValue

            #region Resolve Rows
            swagDataTable.DelaySave = true;
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    dr[newColName] = Convert.ChangeType(dr[$"{originalSwagDataColumn.ColumnName}"].ToString(), targetType);
                }
                catch
                {
                    dr[newColName] = defaultValue;
                }
            }
            swagDataTable.DelaySave = false;
            #endregion Resolve Rows

            if (!keepOriginal)
            {
                newSwagDataColumn.SetSequence(originalSwagDataColumn.Sequence);
                swagDataTable.Columns.Remove(originalSwagDataColumn.ColumnName);
                newSwagDataColumn.Rename(originalSwagDataColumn.ColumnName);
            }
            swagDataTable.Save();

            flogDetail.EndTimer();
        }

        private void SwagColumnHeader_MoveClick(object sender, RoutedEventArgs e)
        {
            FlogDetail flogDetail = GetFlogDetail("Move Column", null);
            flogDetail.StartTimer();

            MenuItem menuItem = (MenuItem)sender;
            SwagDataColumn swagDataColumn = (SwagDataColumn)menuItem.DataContext;
            Int32 offSet = Int32.Parse(menuItem.Tag.ToString());
            Int32 targetSequence = swagDataColumn.Sequence + offSet;
            if (targetSequence < 0)
            {
                targetSequence = 0;
            }
            else if (targetSequence > swagDataColumn.SwagDataTable.Columns.Count -1)
            {
                targetSequence = swagDataColumn.SwagDataTable.Columns.Count - 1;
            }
            swagDataColumn.SetSequence(targetSequence);

            flogDetail.EndTimer();
        }

        private void SwagColumnHeader_RenameClick(object sender, RoutedEventArgs e)
        {
            FlogDetail flogDetail = GetFlogDetail("Rename Convert", null);
            flogDetail.StartTimer();

            Button btnRename = (Button)sender;
            MenuItem miParent = (MenuItem)((MenuItem)((Grid)btnRename.Parent).Parent).Parent;
            SwagDataColumn swagDataColumn = (SwagDataColumn)btnRename.DataContext;
            ContextMenu contextMenu = DependencyObjectHelper.TryFindParent<ContextMenu>(btnRename);

            Grid grid = miParent.FindLogicalChild<Grid>("gridRename");
            String newColName = grid.FindVisualChild<TextBox>().Text;
            swagDataColumn.Rename(newColName);
            contextMenu.IsOpen = false;
            flogDetail.EndTimer();
        }

        private static FlogDetail GetFlogDetail(String message, Exception ex)
        {
            return new FlogDetail
            {
                Product = "SwagDataGrid",
                UserName = Environment.UserName,
                Hostname = Environment.MachineName,
                Message = message,
                Exception = ex
            };
        }

        private void SwagColumnHeader_RenameLoad(object sender, RoutedEventArgs e)
        {
            TextBox txtRename = (TextBox)sender;
            txtRename.SelectAll();
            txtRename.Focus();
        }

        private void SwagColumnHeader_RenameKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox txtRename = (TextBox)sender;
                MenuItem menuItem = (MenuItem)DependencyObjectHelper.TryFindParent<MenuItem>(txtRename);
                Button btnRename = menuItem.FindLogicalChild<Button>();
                btnRename.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void DataGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            SwagDataTable.Columns[e.Column.Header.ToString()].SetSequence(e.Column.DisplayIndex);
        }
    }
}
