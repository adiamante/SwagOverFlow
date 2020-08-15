using System;
using SwagOverFlow.WPF.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Linq;
using System.Windows.Threading;
using SwagOverFlow.WPF.UI;
using SwagOverFlow.Utils;
using System.Windows.Input;
using System.Data;
using SwagOverFlow.Logger;
using System.Windows.Controls.Primitives;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Extensions;
using System.Collections;
using SwagOverFlow.Commands;
using SwagOverFlow.Data.Converters;
using Microsoft.Win32;
using System.IO;
using SwagOverFlow.Iterator;
using System.Threading.Tasks;

namespace SwagOverFlow.WPF.Controls
{
    #region SwagTableImportType
    public enum SwagTableImportType
    {
        Csv,
        Tsv
    }
    #endregion SwagTableImportType

    #region SwagTableSourceType
    public enum SwagTableSourceType
    {
        File,
        Clipboard
    }
    #endregion SwagTableSourceType

    #region SwagTableExportType
    public enum SwagTableExportType
    {
        Csv,
        Sqlite,
        TSql_Command,
        Sqlite_Command
    }
    #endregion SwagTableExportType

    #region SwagTableDestinationType
    public enum SwagTableDestinationType
    {
        Clipboard,
        File,
        New_Window
    }
    #endregion SwagTableDestinationType

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
                    typeof(SwagDataGrid),
                    new FrameworkPropertyMetadata(null, SwagDataTablePropertyChanged));

        private static void SwagDataTablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwagDataGrid swagDataGrid = (SwagDataGrid)d;
            if (swagDataGrid != null && swagDataGrid.SwagDataTable != null && !swagDataGrid.SwagDataTable.IsInitialized && swagDataGrid.SwagDataTable.DataTable != null)
            {
                SwagDataTable swagDataTable = swagDataGrid.SwagDataTable;
                InitSwagDataTable(swagDataTable);
            }
        }

        //private void _tabs_SwagItemChanged(object sender, SwagItemChangedEventArgs e)
        //{
        //    _tabs_PropertyChangedExtended(sender, e.PropertyChangedArgs);
        //}

        //private void _tabs_PropertyChangedExtended(object sender, PropertyChangedExtendedEventArgs e)
        //{
        //    switch (e.PropertyName)
        //    {
        //        case "SelectedIndex":
        //            if (_context != null)
        //            {
        //                SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
        //                this.Tabs = Tabs;
        //                work.DataTables.Update(this);
        //                work.Complete();
        //            }
        //            break;
        //    }
        //}

        //private void _settings_SwagItemChanged(object sender, SwagItemChangedEventArgs e)
        //{
        //    switch (e.PropertyChangedArgs.PropertyName)
        //    {
        //        case "Value":
        //        case "IsExpanded":
        //            if (_context != null)
        //            {
        //                SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
        //                this.Settings = Settings;
        //                work.DataTables.Update(this);
        //                work.Complete();
        //            }
        //            break;
        //    }
        //}

        public static void InitSwagDataTable(SwagDataTable swagDataTable)
        {
            swagDataTable.PropertyChanged += SwagDataTable_PropertyChanged;

            #region Clear Columns and Rows for instance
            swagDataTable.Columns.Clear();
            swagDataTable.Children.Clear();
            #endregion Clear Columns and Rows for instance

            #region Add Columns and Rows for instance
            swagDataTable.InitColumns();
            foreach (DataColumn dc in swagDataTable.DataTable.Columns)
            {
                SwagDataColumn sdc = new SwagDataColumn() { ColumnName = dc.ColumnName, DataType = dc.DataType };
                sdc.SwagDataTable = swagDataTable;
                sdc.DataTypeString = sdc.DataTypeString;
                swagDataTable.Children.Add(sdc);
                swagDataTable.Columns.Add(dc.ColumnName, sdc);
            }

            swagDataTable.DictRows.Clear();
            foreach (DataRow dr in swagDataTable.DataTable.Rows)
            {
                SwagDataRow row = new SwagDataRow(dr);
                row.Value = row.Value;
                row.ValueTypeString = row.ValueTypeString;
                swagDataTable.Children.Add(row);
                swagDataTable.DictRows.Add(row.DataRow, row);
            }
            #endregion Add Columns and Rows for instance

            #region InitViews
            CollectionViewSource columnsVisibilitySource, columnsFilterSource;
            columnsVisibilitySource = new CollectionViewSource() { Source = swagDataTable.Columns };
            columnsFilterSource = new CollectionViewSource() { Source = swagDataTable.Columns };
            columnsFilterSource.View.Filter = (itm) =>
            {
                KeyValuePair<String, SwagDataColumn> kvp = (KeyValuePair<String, SwagDataColumn>)itm;
                return kvp.Value.HasAppliedFilter;
            };
            swagDataTable.ColumnsVisibilityView = columnsVisibilitySource.View;
            swagDataTable.ColumnsFilterView = columnsFilterSource.View;
            #endregion InitViews

            #region FilterCommand
            swagDataTable.FilterCommand = new RelayCommand(() =>
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(swagDataTable.DataTable.DefaultView);
                if (view is BindingListCollectionView)      //Assuming you are DataView for now
                {
                    BindingListCollectionView bindingView = (BindingListCollectionView)view;
                    //https://stackoverflow.com/questions/9385489/why-errors-when-filters-datatable-with-collectionview
                    bindingView.CancelEdit();

                    String combinedFilter = "";
                    foreach (KeyValuePair<string, SwagDataColumn> kvp in swagDataTable.Columns)
                    {
                        if (kvp.Value.HasAppliedFilter)
                        {
                            String filterTemp = kvp.Value.AppliedFilter;
                            if (kvp.Value.AppliedFilter.Contains("'(Blanks)'"))
                            {
                                filterTemp = string.Format("({0} OR CONVERT([{1}], 'System.String') = '' OR [{1}] IS NULL)", kvp.Value.AppliedFilter, kvp.Key);
                            }
                            combinedFilter = string.Format("{0}{1} AND ", combinedFilter, filterTemp);
                        }
                    }

                    if (combinedFilter.EndsWith("AND "))
                    {
                        combinedFilter = combinedFilter.Substring(0, combinedFilter.Length - 4);
                    }

                    bindingView.CustomFilter = combinedFilter;
                }

                columnsFilterSource.View.Refresh();
            });
            #endregion FilterCommand

            #region ExportCommand
            swagDataTable.ExportCommand = new RelayCommand(() =>
            {
                IDataTableConverter converter = null;
                string dialogFilter = "";
                SwagTableExportType exportType = swagDataTable.Settings["Export"]["Type"].GetValue<SwagTableExportType>();
                switch (exportType)
                {
                    case SwagTableExportType.Csv:
                        converter = new DataTableCsvStringConverter();
                        dialogFilter = "CSV files (*.csv)|*.csv";
                        break;
                    case SwagTableExportType.TSql_Command:
                        converter = new DataTableTSqlCommandConverter();
                        dialogFilter = "SQL files (*.sql)|*.sql";
                        break;
                    case SwagTableExportType.Sqlite:
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        sfd.FileName = Path.ChangeExtension(swagDataTable.DataTable.TableName, null);
                        sfd.Filter = "SQLite files (*.db;*.sqlite)|*.db;*.sqlite";
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            if (sfd.ShowDialog() ?? false)
                            {
                                DataSetSqliteFileConverter dsConverter = new DataSetSqliteFileConverter();
                                DataSet ds = new DataSet();
                                ds.Tables.Add(swagDataTable.DataTable.Copy());
                                dsConverter.FromDataSet(null, ds, sfd.FileName);
                            }
                        }));
                        return;
                    case SwagTableExportType.Sqlite_Command:
                        converter = new DataTableSqliteCommandConverter();
                        dialogFilter = "SQLite command files (*.cmd)|*.cmd";
                        break;
                }

                Object output = converter.FromDataTableToObject(new DataTableConvertParams(), swagDataTable.DataTable);

                switch (swagDataTable.Settings["Export"]["Destination"].GetValue<SwagTableDestinationType>())
                {
                    case SwagTableDestinationType.Clipboard:
                        Clipboard.SetText(output.ToString());
                        break;
                    case SwagTableDestinationType.File:
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        sfd.FileName = swagDataTable.DataTable.TableName;
                        sfd.Filter = dialogFilter;

                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            if (sfd.ShowDialog() ?? false)
                            {
                                File.WriteAllText(sfd.FileName, output.ToString());
                            }
                        }));
                        break;
                    case SwagTableDestinationType.New_Window:
                        Window window = new Window();
                        TextBox textBox = new TextBox();
                        textBox.AcceptsReturn = textBox.AcceptsTab = true;
                        textBox.Text = output.ToString();
                        window.Content = textBox;
                        window.Show();
                        break;
                }
            });
            #endregion ExportCommand

            #region ImportCommand
            swagDataTable.ImportCommand = new RelayCommand(() =>
            {
                IDataTableConverter converter = null;
                DataTableConvertParams cp = new DataTableConvertParams();

                string dialogFilter = "";
                String inputText = "";

                switch (swagDataTable.Settings["Import"]["Type"].GetValue<SwagTableImportType>())
                {
                    case SwagTableImportType.Csv:
                        converter = new DataTableCsvStringConverter();
                        dialogFilter = "CSV files (*.csv)|*.csv";
                        break;
                    case SwagTableImportType.Tsv:
                        converter = new DataTableCsvStringConverter();
                        cp.FieldDelim = '\t';
                        dialogFilter = "TSV files (*.tsv)|*.tsv";
                        break;
                }

                switch (swagDataTable.Settings["Import"]["Source"].GetValue<SwagTableSourceType>())
                {
                    case SwagTableSourceType.File:
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        ofd.Filter = dialogFilter;

                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            if (ofd.ShowDialog() ?? false)
                            {
                                inputText = File.ReadAllText(ofd.FileName);
                            }
                            else
                            {
                                return;
                            }
                        }));
                        break;
                    case SwagTableSourceType.Clipboard:
                        inputText = Clipboard.GetText();
                        break;
                }

                DataTable dtInput = converter.ToDataTable(cp, inputText);
                swagDataTable.DataTable = dtInput;
            });
            #endregion ImportCommand

            #region ApplyColumnVisibilityFilterCommand
            swagDataTable.ApplyColumnVisibilityFilterCommand =
            new RelayCommand(() =>
            {
                columnsVisibilitySource.View.Filter = (itm) =>
                {
                    KeyValuePair<String, SwagDataColumn> kvp = (KeyValuePair<String, SwagDataColumn>)itm;
                    return SearchHelper.Evaluate(
                        kvp.Key,
                        swagDataTable.Settings["ColumnEditor"]["Visibility"]["Search"]["Text"].GetValue<string>(),
                        false,
                        swagDataTable.Settings["ColumnEditor"]["Visibility"]["Search"]["FilterMode"].GetValue<FilterMode>(),
                        false);
                };
            });
            #endregion ApplyColumnVisibilityFilterCommand

            #region ApplyColumnFiltersFilterCommand
            swagDataTable.ApplyColumnFiltersFilterCommand =
            new RelayCommand(() =>
            {
                columnsFilterSource.View.Filter = (itm) =>
                {
                    KeyValuePair<String, SwagDataColumn> kvp = (KeyValuePair<String, SwagDataColumn>)itm;
                    return kvp.Value.HasAppliedFilter && (SearchHelper.Evaluate(
                        kvp.Key,
                        swagDataTable.Settings["ColumnEditor"]["Filters"]["Search"]["Text"].GetValue<string>(),
                        false,
                        swagDataTable.Settings["ColumnEditor"]["Filters"]["Search"]["FilterMode"].GetValue<FilterMode>(),
                        false) || SearchHelper.Evaluate(
                        kvp.Value.AppliedFilter,
                        swagDataTable.Settings["ColumnEditor"]["Filters"]["Search"]["Text"].GetValue<string>(),
                        false,
                        swagDataTable.Settings["ColumnEditor"]["Filters"]["Search"]["FilterMode"].GetValue<FilterMode>(),
                        false));
                };
            });
            #endregion ApplyColumnFiltersFilterCommand

            #region ResetColumnsCommand
            swagDataTable.ResetColumnsCommand =
            new RelayCommand(() =>
            {
                swagDataTable.DelaySave = true;
                columnsVisibilitySource.Source = columnsFilterSource.Source = swagDataTable.Columns;
                columnsVisibilitySource.View.Refresh();
                columnsFilterSource.View.Refresh();
                swagDataTable.OnPropertyChangedPublic("ColumnsView");
                swagDataTable.OnPropertyChangedPublic("ColumnCount");
                swagDataTable.DelaySave = false;
                var tempCols = swagDataTable.Columns;
                swagDataTable.Columns = null;
                swagDataTable.Columns = tempCols;
            });
            #endregion ResetColumnsCommand

            #region InitSettings
            swagDataTable.Settings.TryAddChildSetting("ColumnEditor", new SwagSettingGroup() { Icon = PackIconCustomKind.TableColumnEdit });
            swagDataTable.Settings["ColumnEditor"].TryAddChildSetting("Visibility", new SwagSettingGroup() { Icon = PackIconCustomKind.Eye });
            swagDataTable.Settings["ColumnEditor"]["Visibility"].TryAddChildSetting("Search", new SwagSettingGroup() { Icon = PackIconCustomKind.Search });
            swagDataTable.Settings["ColumnEditor"]["Visibility"]["Search"].TryAddChildSetting("Text", new SwagSetting<String>() { Icon = PackIconCustomKind.KeyValue });
            swagDataTable.Settings["ColumnEditor"]["Visibility"]["Search"].TryAddChildSetting("FilterMode", new SwagSetting<FilterMode>() { SettingType = SettingType.DropDown, Value = FilterMode.CONTAINS, Icon = PackIconCustomKind.Filter, ItemsSource = (FilterMode[])Enum.GetValues(typeof(FilterMode)) });
            swagDataTable.Settings["ColumnEditor"].TryAddChildSetting("Filters", new SwagSettingGroup() { Icon = PackIconCustomKind.TableColumnFilter });
            swagDataTable.Settings["ColumnEditor"]["Filters"].TryAddChildSetting("Search", new SwagSettingGroup() { Icon = PackIconCustomKind.Search });
            swagDataTable.Settings["ColumnEditor"]["Filters"]["Search"].TryAddChildSetting("Text", new SwagSetting<String>() { Icon = PackIconCustomKind.KeyValue });
            swagDataTable.Settings["ColumnEditor"]["Filters"]["Search"].TryAddChildSetting("FilterMode", new SwagSetting<FilterMode>() { SettingType = SettingType.DropDown, Value = FilterMode.CONTAINS, Icon = PackIconCustomKind.Filter, ItemsSource = (FilterMode[])Enum.GetValues(typeof(FilterMode)) });
            swagDataTable.Settings.TryAddChildSetting("Search", new SwagSettingGroup() { Icon = PackIconCustomKind.TableSearch });
            swagDataTable.Settings["Search"].TryAddChildSetting("Text", new SwagSetting<String>() { Icon = PackIconCustomKind.KeyValue });
            swagDataTable.Settings["Search"].TryAddChildSetting("FilterMode", new SwagSetting<FilterMode>() { SettingType = SettingType.DropDown, Value = FilterMode.CONTAINS, Icon = PackIconCustomKind.Filter, ItemsSource = (FilterMode[])Enum.GetValues(typeof(FilterMode)) });
            swagDataTable.Settings.TryAddChildSetting("Export", new SwagSettingGroup() { Icon = PackIconCustomKind.Export });
            swagDataTable.Settings["Export"].TryAddChildSetting("Type", new SwagSetting<SwagTableExportType>() { SettingType = SettingType.DropDown, Value = SwagTableExportType.Csv, Icon = PackIconCustomKind.ExportType, ItemsSource = (SwagTableExportType[])Enum.GetValues(typeof(SwagTableExportType)) });
            swagDataTable.Settings["Export"].TryAddChildSetting("Destination", new SwagSetting<SwagTableDestinationType>() { SettingType = SettingType.DropDown, Value = SwagTableDestinationType.Clipboard, Icon = PackIconCustomKind.Destination, ItemsSource = (SwagTableDestinationType[])Enum.GetValues(typeof(SwagTableDestinationType)) });
            swagDataTable.Settings.TryAddChildSetting("Import", new SwagSettingGroup() { Icon = PackIconCustomKind.Import });
            swagDataTable.Settings["Import"].TryAddChildSetting("Type", new SwagSetting<SwagTableImportType>() { SettingType = SettingType.DropDown, Value = SwagTableImportType.Tsv, Icon = PackIconCustomKind.ExportType, ItemsSource = (SwagTableImportType[])Enum.GetValues(typeof(SwagTableImportType)) });
            swagDataTable.Settings["Import"].TryAddChildSetting("Source", new SwagSetting<SwagTableSourceType>() { SettingType = SettingType.DropDown, Value = SwagTableSourceType.Clipboard, Icon = PackIconCustomKind.Destination, ItemsSource = (SwagTableSourceType[])Enum.GetValues(typeof(SwagTableSourceType)) });
            #endregion InitSettings

            #region InitTabs
            SwagTabGroup tabs = new SwagTabGroup();
            tabs["ColumnEditor"] = new SwagTabGroup() { Icon = PackIconCustomKind.TableColumnEdit, Display = "Column Editor" };
            tabs["Search"] = new SwagTabItem() { Icon = PackIconCustomKind.TableSearch };
            tabs["Export"] = new SwagTabItem() { Icon = PackIconCustomKind.TableExport };
            tabs["Import"] = new SwagTabItem() { Icon = PackIconCustomKind.TableImport };
            tabs["Settings"] = new SwagTabItem() { Icon = PackIconCustomKind.TableSettings };
            tabs["ColumnEditor"]["Visibility"] = new SwagTabItem() { Icon = PackIconCustomKind.TableColumnVisibility };
            tabs["ColumnEditor"]["Filters"] = new SwagTabItem() { Icon = PackIconCustomKind.TableColumnFilter };
            //tabs["ColumnEditor"]["Add"] = new SwagTabItem() { Icon = PackIconCustomKind.TableColumnAdd };
            //tabs["ColumnEditor"]["View"] = new SwagTabItem() { Icon = PackIconCustomKind.TableColumnView };
            swagDataTable.Tabs = tabs;
            SwagItemPreOrderIterator<SwagTabItem> iterator = tabs.CreateIterator();
            for (SwagTabItem tabItem = iterator.First(); !iterator.IsDone; tabItem = iterator.Next())
            {
                tabItem.ViewModel = swagDataTable;
            }
            //swagDataTable.Tabs.SwagItemChanged += _tabs_SwagItemChanged;
            //swagDataTable.Tabs.PropertyChangedExtended += _tabs_PropertyChangedExtended;
            #endregion InitTabs

            swagDataTable.InitDataTable();
            swagDataTable.IsInitialized = true;
            //swagDataTable.Settings.SwagItemChanged += _settings_SwagItemChanged;
        }

        private static void SwagDataTable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SwagDataTable swagDataTable = (SwagDataTable)sender;
            switch (e.PropertyName)
            {
                case "DataTable":
                    swagDataTable.Columns.Clear();
                    InitSwagDataTable(swagDataTable);
                    break;
            }
        }

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
                                            typeof(IDictionary<String, SwagDataColumn>),
                                            typeof(SwagDataGrid),
                                            new UIPropertyMetadata(null, BindableColumnsPropertyChanged));

        private static void BindableColumnsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            SwagDataGrid fdgDataGrid = source as SwagDataGrid;
            DataGrid dataGrid = fdgDataGrid.DataGrid;

            if (dataGrid != null)
            {
                dataGrid.Dispatcher.Invoke(new Action(() =>
                {
                    #region columnCollectionChanged
                    Action<object, NotifyCollectionChangedEventArgs> columnCollectionChanged = (s, ne) =>
                    {
                        
                    };
                    #endregion columnCollectionChanged

                    if (e.NewValue != null)
                    {
                        ICollectionView columns = UIHelper.GetCollectionView((IEnumerable)e.NewValue);
                        SortDescription sortDescription = new SortDescription("Value.ColSeq", ListSortDirection.Ascending);
                        if (!columns.SortDescriptions.Contains(sortDescription))
                        {
                            columns.SortDescriptions.Add(sortDescription);
                        }
                        columns.Filter = (itm) =>
                        {
                            KeyValuePair<String, SwagDataColumn> kvp = (KeyValuePair<String, SwagDataColumn>)itm;
                            return kvp.Value.IsVisible;
                        };

                        dataGrid.Columns.Clear();
                        foreach (KeyValuePair<String, SwagDataColumn> sdcKvp in columns)
                        {
                            dataGrid.Columns.Add(sdcKvp.Value.DataGridColumn());
                            sdcKvp.Value.Init();
                        }

                        columns.CollectionChanged += fdgDataGrid.Columns_CollectionChanged;
                    }

                    if (e.OldValue is IEnumerable oldCol)
                    {
                        ICollectionView columns = UIHelper.GetCollectionView(oldCol);
                        columns.CollectionChanged -= fdgDataGrid.Columns_CollectionChanged;
                    }
                }));
            }
        }

        private void Columns_CollectionChanged(object s, NotifyCollectionChangedEventArgs e)
        {
            ICollectionView cols = (ICollectionView)s;
            DataGrid dataGrid = DataGrid;   //This is kind of dirty but gotta get it working yo
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Replace:
                    dataGrid.Columns.Clear();
                    foreach (KeyValuePair<String, SwagDataColumn> kvp in cols)
                    {
                        dataGrid.Columns.Add(kvp.Value.DataGridColumn());
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                    foreach (KeyValuePair<String, SwagDataColumn> kvp in e.NewItems)
                    {
                        kvp.Value.Init();
                        dataGrid.Columns.Add(kvp.Value.DataGridColumn());
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    foreach (KeyValuePair<String, SwagDataColumn> kvp in e.NewItems)
                    {
                        dataGrid.Columns.RemoveAt(e.OldStartingIndex);
                        dataGrid.Columns.Add(kvp.Value.DataGridColumn());
                    }
                    //dataGrid.Columns.Move(ne.OldStartingIndex, ne.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (KeyValuePair<String, SwagDataColumn> kvp in e.OldItems)
                    {
                        dataGrid.Columns.RemoveAt(e.OldStartingIndex);
                    }
                    break;
            }
        }

        public IDictionary<String, SwagDataColumn> Columns
        {
            get { return (IDictionary<String, SwagDataColumn>)GetValue(ColumnsProperty); }
            set
            {
                SetValue(ColumnsProperty, value);
                OnPropertyChanged();
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

            if (e.NewValue != null)
            {
                fdgDataGrid.View((SwagDataColumn)e.NewValue);
            }
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

            if (e.NewValue != null)
            {
                SwagDataRowResult rowResult = (SwagDataRowResult)e.NewValue;
                fdgDataGrid.View(rowResult);
            }
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

            BindingOperations.SetBinding(this, SwagDataGrid.ColumnsProperty, new Binding("SwagDataTable.Columns") { RelativeSource = RelativeSource.Self });
            BindingOperations.SetBinding(this, SwagDataGrid.SelectedColumnProperty, new Binding("SwagDataTable.SelectedColumn") { RelativeSource = RelativeSource.Self });
            BindingOperations.SetBinding(this, SwagDataGrid.SelectedRowProperty, new Binding("SwagDataTable.SelectedRow") { RelativeSource = RelativeSource.Self });
        }

        private void SwagDataGridInstance_Loaded(object sender, RoutedEventArgs e)
        {
            //FIX_THIS
            //SwagDataTable?.InitSettings();
            //SwagDataTable?.InitTabs();
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
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                DataGrid.Focus();
            }));
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
                DataGrid.Focus();
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
        private void Search_ResultGo_Opened(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = (FrameworkElement)sender;
            SwagDataResult swagDataResult = (SwagDataResult)frameworkElement.DataContext;
            swagDataResult.IsSelected = true;
        }

        private async void SwagColumnHeader_ConvertClick(object sender, RoutedEventArgs e)
        {
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

            SwagLogger.LogStart(this, "Convert Column |col={Column}|", originalSwagDataColumn.ColumnName);

            Int32 count = 1;
            while (dt.Columns.Contains(newColName))
            {
                newColName = $"{originalSwagDataColumn.ColumnName}{count}{targetType.Name}";
                count++;
            }

            SwagDataColumn newSwagDataColumn = new SwagDataColumn() { ColumnName = newColName, DataType = targetType };
            swagDataTable.Columns.Add(newSwagDataColumn.ColumnName, newSwagDataColumn);
            SwagWindow.GlobalIsBusy = true;
            await Task.Run(() =>
            {
                Func<Type, String, object, object> convert = (type, input, defaultOutput) =>
                {
                    switch (type.Name)
                    {
                        case "Int32":
                            if (Int32.TryParse(input, out Int32 outInt32))
                            {
                                return outInt32;
                            }
                            break;
                        case "Decimal":
                            if (Decimal.TryParse(input, out Decimal outDecimal))
                            {
                                return outDecimal;
                            }
                            break;
                        case "DateTime":
                            if (DateTime.TryParse(input, out DateTime outDateTime))
                            {
                                return outDateTime;
                            }
                            break;
                        case "TimeSpan":
                            if (TimeSpan.TryParse(input, out TimeSpan outTimeSpan))
                            {
                                return outTimeSpan;
                            }
                            break;
                        case "String":
                        default:
                            return input.ToString();
                    }
                    return defaultOutput;
                };

                #region Resolve defaultValue
                Object defaultValue = DBNull.Value;
                if (targetType.GetTypeCode() != TypeCode.String)
                {
                    defaultValue = convert(targetType, defaultValueText, DBNull.Value);
                }
                else
                {
                    defaultValue = defaultValueText;
                }
                #endregion Resolve defaultValue

                #region Resolve Rows
                swagDataTable.DelaySave = true;
                using (SwagDataTable.FreezeList freeze = new SwagDataTable.FreezeList(swagDataTable))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr[newColName] = convert(targetType, dr[$"{originalSwagDataColumn.ColumnName}"].ToString(), defaultValue);
                    }
                }
                swagDataTable.DelaySave = false;
                #endregion Resolve Rows
            });

            SwagWindow.GlobalIsBusy = false;
            if (!keepOriginal)
            {
                newSwagDataColumn.SetSequence(originalSwagDataColumn.ColSeq);
                originalSwagDataColumn.Remove();
                newSwagDataColumn.Rename(originalSwagDataColumn.ColumnName);
            }

            SwagLogger.LogEnd(this, "Convert Column |col={Column}|", originalSwagDataColumn.ColumnName);
        }

        private void SwagColumnHeader_MoveClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            SwagDataColumn swagDataColumn = (SwagDataColumn)menuItem.DataContext;
            SwagLogger.LogStart(this, "Move Column |col={Column}|", swagDataColumn.ColumnName);

            Int32 offSet = Int32.Parse(menuItem.Tag.ToString());
            Int32 targetSequence = swagDataColumn.ColSeq + offSet;
            if (targetSequence < 0)
            {
                targetSequence = 0;
            }
            else if (targetSequence > swagDataColumn.SwagDataTable.Columns.Count - 1)
            {
                targetSequence = swagDataColumn.SwagDataTable.Columns.Count - 1;
            }
            swagDataColumn.SetSequence(targetSequence);
            swagDataColumn.SwagDataTable.ResetColumnsCommand.Execute(null);

            SwagLogger.LogEnd(this, "Move Column |col={Column}|", swagDataColumn.ColumnName);
        }

        private void SwagColumnHeader_RenameClick(object sender, RoutedEventArgs e)
        {
            Button btnRename = (Button)sender;
            MenuItem miParent = (MenuItem)((MenuItem)((Grid)btnRename.Parent).Parent).Parent;
            SwagDataColumn swagDataColumn = (SwagDataColumn)btnRename.DataContext;
            ContextMenu contextMenu = DependencyObjectHelper.TryFindParent<ContextMenu>(btnRename);

            Grid grid = miParent.FindLogicalChild<Grid>("gridRename");
            String originalName = swagDataColumn.ColumnName;
            String newColName = grid.FindVisualChild<TextBox>().Text;
            SwagLogger.LogStart(this, "Column Rename |orig={Column}|new={NewColName}|", originalName, newColName);
            swagDataColumn.Rename(newColName);
            contextMenu.IsOpen = false;

            SwagLogger.LogEnd(this, "Column Rename |orig={Column}|new={NewColName}|", originalName, newColName);
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

            SwagLogger.LogStart(this, "Fill Column with Default |col={Column}|", swagDataColumn.ColumnName);
            swagDataTable.DelaySave = true;
            using (SwagDataTable.FreezeList freeze = new SwagDataTable.FreezeList(swagDataTable))
            {
                foreach (DataRowView drv in swagDataTable.DataTable.DefaultView)
                {
                    if (drv[colName] == null || drv[colName] == DBNull.Value || drv[colName].ToString() == "")
                    {
                        drv[colName] = Activator.CreateInstance(targetType);
                    }
                }
            }
            swagDataTable.DelaySave = false;
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

            SwagLogger.LogStart(this, "Fill Column with Input |col={Column}|inpt={Input}|", swagDataColumn.ColumnName, defaultValueText);

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
                using (SwagDataTable.FreezeList freeze = new SwagDataTable.FreezeList(swagDataTable))
                {
                    foreach (DataRowView drv in swagDataTable.DataTable.DefaultView)
                    {
                        if (drv[colName] == null || drv[colName] == DBNull.Value || drv[colName].ToString() == "")
                        {
                            drv[colName] = defaultValue;
                        }
                    }
                }
                swagDataTable.DelaySave = false;
            }

            contextMenu.IsOpen = false;
            SwagLogger.LogEnd(this, "Fill Column with Input |col={Column}|inpt={Input}|", swagDataColumn.ColumnName, defaultValueText);
        }

        private void SwagColumnHeader_SelectColumnValueClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = DependencyObjectHelper.TryFindParent<ContextMenu>(menuItem);
            FrameworkElement placementTarget = (FrameworkElement)contextMenu.PlacementTarget;
            DataGridColumnHeader dataGridColumnHeader = DependencyObjectHelper.TryFindParent<DataGridColumnHeader>(placementTarget);
            SwagDataColumn swagDataColumn = (SwagDataColumn)menuItem.DataContext;
            SwagDataTable swagDataTable = swagDataColumn.SwagDataTable;
            SwagLogger.LogStart(this, "Select Column |col={Column}|", swagDataColumn.ColumnName);

            DataGrid.SelectedCellsChanged -= DataGrid_SelectedCellsChanged;
            DataGrid.SelectedCells.Clear();
            foreach (DataRowView drv in swagDataTable.DataTable.DefaultView)
            {
                DataGrid.SelectedCells.Add(new DataGridCellInfo(drv, dataGridColumnHeader.Column));
            }
            DataGrid.SelectedCellsChanged += DataGrid_SelectedCellsChanged;
            DataGrid_SelectedCellsChanged(null, null);
            SwagLogger.LogStart(this, "Select Column |col={Column}|", swagDataColumn.ColumnName);
        }

        #endregion SwagColumnHeader

    }
}