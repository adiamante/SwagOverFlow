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
        #region Properties
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

        #region SelectedColumn
        public static DependencyProperty SelectedColumnProperty =
                DependencyProperty.RegisterAttached("SelectedColumn",
                                                    typeof(SwagDataColumn),
                                                    typeof(SwagDataGrid),
                                                    new UIPropertyMetadata(null, BindableSelectedColumnPropertyChanged));

        private static void BindableSelectedColumnPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            SwagDataGrid fdgDataGrid = source as SwagDataGrid;
            fdgDataGrid.View((SwagDataColumn)e.NewValue);
        }
        #endregion SelectedColumn

        #region SelectedRow
        public static DependencyProperty SelectedRowProperty =
                DependencyProperty.RegisterAttached("SelectedRow",
                                                    typeof(SwagDataRowResult),
                                                    typeof(SwagDataGrid),
                                                    new UIPropertyMetadata(null, BindableSelectedRowPropertyChanged));

        private static void BindableSelectedRowPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            SwagDataGrid fdgDataGrid = source as SwagDataGrid;
            SwagDataRowResult rowResult = (SwagDataRowResult)e.NewValue;
            fdgDataGrid.View(rowResult);
        }
        #endregion SelectedRow

        #region SelectedTotal
        public static DependencyProperty SelectedTotalProperty =
                DependencyProperty.Register(
                    "SelectedTotal",
                    typeof(Decimal),
                    typeof(SwagDataGrid));

        public Decimal SelectedTotal
        {
            get { return (Decimal)GetValue(SelectedTotalProperty); }
            set
            {
                SetValue(SelectedTotalProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion SelectedTotal
        #region SelectedCount
        public static DependencyProperty SelectedCountProperty =
                DependencyProperty.Register(
                    "SelectedCount",
                    typeof(Int32),
                    typeof(SwagDataGrid));

        public Int32 SelectedCount
        {
            get { return (Int32)GetValue(SelectedCountProperty); }
            set
            {
                SetValue(SelectedCountProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion SelectedCount
        #endregion Properties

        #region Initialization
        public SwagDataGrid()
        {
            InitializeComponent();

            BindingOperations.SetBinding(this, SwagDataGrid.ColumnsProperty, new Binding("SwagDataTable.ColumnsView") { RelativeSource = RelativeSource.Self });
            BindingOperations.SetBinding(this, SwagDataGrid.SelectedColumnProperty, new Binding("SwagDataTable.SelectedColumn") { RelativeSource = RelativeSource.Self });
            BindingOperations.SetBinding(this, SwagDataGrid.SelectedRowProperty, new Binding("SwagDataTable.SelectedRow") { RelativeSource = RelativeSource.Self });
        }

        private void SwagDataGridInstance_Loaded(object sender, RoutedEventArgs e)
        {
            SwagDataTable?.InitSettings();
            SwagDataTable?.InitTabs();
        }
        #endregion Initialization

        #region DataGrid Events
        private void DataGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            SwagDataTable.Columns[e.Column.Header.ToString()].SetSequence(e.Column.DisplayIndex);
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (DataGrid.SelectedCells.Count > 0)
            {
                Decimal selectedTotal = 0.0m;
                foreach (DataGridCellInfo cellInfo in DataGrid.SelectedCells)
                {
                    DataGridColumn dgCol = cellInfo.Column;
                    DataRowView drv = (DataRowView)cellInfo.Item;

                    if (Decimal.TryParse(drv[dgCol.Header.ToString()].ToString(), out Decimal val))
                    {
                        selectedTotal += val;
                    }
                }

                this.SelectedTotal = selectedTotal;
                this.SelectedCount = DataGrid.SelectedCells.Count;
            }
        }
        #endregion DataGrid Events

        #region Search
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
                    View(rowResult);
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

        private void View(SwagDataRowResult rowResult)
        {
            SwagDataColumn swagDataColumn = (SwagDataColumn)rowResult.Parent.SwagData;
            SwagDataRow swagDataRow = (SwagDataRow)rowResult.SwagData;
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

        #endregion Search

        #region Import
        private void Import_Paste_Click(object sender, RoutedEventArgs e)
        {
            SwagDataTable.Settings["Import"]["Type"].SetValue<SwagTableImportType>(SwagTableImportType.Tsv);
            SwagDataTable.Settings["Import"]["Source"].SetValue<SwagTableSourceType>(SwagTableSourceType.Clipboard);
            SwagDataTable.ImportCommand.Execute(null);
        }
        #endregion Import

        #region ColumnEditor
        private void SwagDataColumn_ViewClick(object sender, RoutedEventArgs e)
        {
            SwagDataColumn swagDataColumn = ((KeyValuePair<String, SwagDataColumn>)((MenuItem)sender).DataContext).Value;
            View(swagDataColumn);
        }
        #endregion ColumnEditor

        #region SwagColumnHeader
        private void SwagColumnHeader_ConvertClick(object sender, RoutedEventArgs e)
        {
            SwagLogger.LogStart(this, "Convert Column");

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
                originalSwagDataColumn.Remove();
                newSwagDataColumn.Rename(originalSwagDataColumn.ColumnName);
            }
            swagDataTable.Save();

            SwagLogger.LogEnd(this, "Convert Column");
        }

        private void SwagColumnHeader_MoveClick(object sender, RoutedEventArgs e)
        {
            SwagLogger.LogStart(this, "Move Column");

            MenuItem menuItem = (MenuItem)sender;
            SwagDataColumn swagDataColumn = (SwagDataColumn)menuItem.DataContext;
            Int32 offSet = Int32.Parse(menuItem.Tag.ToString());
            Int32 targetSequence = swagDataColumn.Sequence + offSet;
            if (targetSequence < 0)
            {
                targetSequence = 0;
            }
            else if (targetSequence > swagDataColumn.SwagDataTable.Columns.Count - 1)
            {
                targetSequence = swagDataColumn.SwagDataTable.Columns.Count - 1;
            }
            swagDataColumn.SetSequence(targetSequence);
            swagDataColumn.SwagDataTable.ResetColumns();

            SwagLogger.LogEnd(this, "Convert Move");
        }

        private void SwagColumnHeader_RenameClick(object sender, RoutedEventArgs e)
        {
            SwagLogger.LogStart(this, "Convert Rename");

            Button btnRename = (Button)sender;
            MenuItem miParent = (MenuItem)((MenuItem)((Grid)btnRename.Parent).Parent).Parent;
            SwagDataColumn swagDataColumn = (SwagDataColumn)btnRename.DataContext;
            ContextMenu contextMenu = DependencyObjectHelper.TryFindParent<ContextMenu>(btnRename);

            Grid grid = miParent.FindLogicalChild<Grid>("gridRename");
            String newColName = grid.FindVisualChild<TextBox>().Text;
            swagDataColumn.Rename(newColName);
            contextMenu.IsOpen = false;

            SwagLogger.LogEnd(this, "Convert Rename");
        }

        private void SwagColumnHeader_TextBoxLoad(object sender, RoutedEventArgs e)
        {
            TextBox txtText = (TextBox)sender;
            txtText.SelectAll();
            txtText.Focus();
        }

        private void SwagColumnHeader_TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox txtText = (TextBox)sender;
                MenuItem menuItem = (MenuItem)DependencyObjectHelper.TryFindParent<MenuItem>(txtText);
                Button btn = menuItem.FindLogicalChild<Button>();
                btn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void SwagColumnHeader_FillEmptyDefaultClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            SwagDataColumn swagDataColumn = (SwagDataColumn)menuItem.DataContext;
            SwagDataTable swagDataTable = swagDataColumn.SwagDataTable;
            String colName = swagDataColumn.ColumnName;
            Type targetType = swagDataColumn.DataType;

            swagDataTable.DelaySave = true;
            foreach (DataRowView drv in swagDataTable.DataTable.DefaultView)
            {
                if (drv[colName] == null || drv[colName] == DBNull.Value || drv[colName].ToString() == "")
                {
                    drv[colName] = Activator.CreateInstance(targetType);
                }
            }
            swagDataTable.DelaySave = false;
            swagDataTable.Save();

        }

        private void SwagColumnHeader_FillEmptyInputClick(object sender, RoutedEventArgs e)
        {
            Button btnFill = (Button)sender;
            MenuItem miParent = (MenuItem)((MenuItem)((Grid)btnFill.Parent).Parent).Parent;
            SwagDataColumn swagDataColumn = (SwagDataColumn)btnFill.DataContext;
            SwagDataTable swagDataTable = swagDataColumn.SwagDataTable;
            ContextMenu contextMenu = DependencyObjectHelper.TryFindParent<ContextMenu>(btnFill);
            String colName = swagDataColumn.ColumnName;
            Type targetType = swagDataColumn.DataType;

            Grid grid = miParent.FindLogicalChild<Grid>("gridFillEmptyInput");
            String defaultValueText = grid.FindVisualChild<TextBox>().Text;

            if (defaultValueText != "")
            {
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

                swagDataTable.DelaySave = true;
                foreach (DataRowView drv in swagDataTable.DataTable.DefaultView)
                {
                    if (drv[colName] == null || drv[colName] == DBNull.Value || drv[colName].ToString() == "")
                    {
                        drv[colName] = defaultValue;
                    }
                }
                swagDataTable.DelaySave = false;
                swagDataTable.Save();
            }

            contextMenu.IsOpen = false;
        }

        private void SwagColumnHeader_SelectColumnValueClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = DependencyObjectHelper.TryFindParent<ContextMenu>(menuItem);
            FrameworkElement placementTarget = (FrameworkElement)contextMenu.PlacementTarget;
            DataGridColumnHeader dataGridColumnHeader = DependencyObjectHelper.TryFindParent<DataGridColumnHeader>(placementTarget);
            SwagDataColumn swagDataColumn = (SwagDataColumn)menuItem.DataContext;
            SwagDataTable swagDataTable = swagDataColumn.SwagDataTable;

            DataGrid.SelectedCellsChanged -= DataGrid_SelectedCellsChanged;
            DataGrid.SelectedCells.Clear();
            foreach (DataRowView drv in swagDataTable.DataTable.DefaultView)
            {
                DataGrid.SelectedCells.Add(new DataGridCellInfo(drv, dataGridColumnHeader.Column));
            }
            DataGrid.SelectedCellsChanged += DataGrid_SelectedCellsChanged;
            DataGrid_SelectedCellsChanged(null, null);
        }

        #endregion SwagColumnHeader
    }
}