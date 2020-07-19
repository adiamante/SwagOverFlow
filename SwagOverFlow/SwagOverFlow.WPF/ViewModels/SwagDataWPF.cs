using Microsoft.Win32;
using Newtonsoft.Json;
using SwagOverFlow.Iterator;
using SwagOverFlow.ViewModels;
using SwagOverFlow.Data.Converters;
using SwagOverFlow.Logger;
using SwagOverFlow.Utils;
using SwagOverFlow.WPF.Commands;
using SwagOverFlow.WPF.Collections;
using SwagOverFlow.WPF.UI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using SwagOverFlow.Collections;
using SwagOverFlow.Data.Persistence;

namespace SwagOverFlow.WPF.ViewModels
{
    #region SwagData
    //public abstract class SwagData : SwagItem<SwagDataGroup, SwagData>
    //{
    //    SwagDataResult _swagDataResult;
    //    ICommand _selectCommand;
    //    ICommand _removeCommand;

    //    #region SwagDataResult
    //    [NotMapped]
    //    [JsonIgnore]
    //    public SwagDataResult SwagDataResult
    //    {
    //        get { return _swagDataResult; }
    //        set { SetValue(ref _swagDataResult, value); }
    //    }
    //    #endregion SwagDataResult
    //    #region SelectCommand
    //    [JsonIgnore]
    //    [NotMapped]
    //    public ICommand SelectCommand
    //    {
    //        get
    //        {
    //            return _selectCommand ?? (_selectCommand =
    //                new RelayCommand(() =>
    //                {
    //                    IsSelected = true;
    //                }));
    //        }
    //    }
    //    #endregion SelectCommand
    //    #region RemoveCommand
    //    public ICommand RemoveCommand
    //    {
    //        get
    //        {
    //            return _removeCommand ?? (_removeCommand =
    //                new RelayCommand(() =>
    //                {
    //                    Parent.Children.Remove(this);
    //                }));
    //        }
    //    }
    //    #endregion RemoveCommand

    //}
    #endregion SwagData

    #region SwagDataGroup
    //public abstract class SwagDataGroup : SwagData, ISwagParent<SwagData>
    //{
    //    #region Private/Protected Members
    //    protected Boolean _listening = true;
    //    CollectionViewSource _childrenCollectionViewSource;
    //    protected ObservableCollection<SwagData> _children = new ObservableCollection<SwagData>();
    //    #endregion Private/Protected Members

    //    #region Events
    //    public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

    //    public virtual void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedExtendedEventArgs e)
    //    {
    //        SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs() { SwagItem = swagItem, PropertyChangedArgs = e, Message = e.Message });
    //        Parent?.OnSwagItemChanged(swagItem, e);
    //    }
    //    #endregion Events

    //    #region Properties
    //    #region Children
    //    public ObservableCollection<SwagData> Children
    //    {
    //        get { return _children; }
    //        set { SetValue(ref _children, value); }
    //    }
    //    #endregion Children
    //    #region ChildrenView
    //    public ICollectionView ChildrenView
    //    {
    //        get { return _childrenCollectionViewSource.View; }
    //    }
    //    #endregion ChildrenView
    //    #endregion Properties

    //    #region Initialization
    //    public SwagDataGroup()
    //    {
    //        _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
    //        _children.CollectionChanged += _children_CollectionChanged;
    //    }

    //    private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //    {
    //        if (e.NewItems != null)
    //        {

    //            foreach (SwagData newItem in e.NewItems)
    //            {
    //                newItem.CanUndo = false;
    //                newItem.Parent = this;
    //                if (newItem.Sequence <= 0)
    //                {
    //                    newItem.Sequence = this.Children.Count;
    //                }
    //                newItem.CanUndo = true;
    //            }
    //        }

    //        _childrenCollectionViewSource.View.Refresh();
    //    }
    //    #endregion Initialization
    //}
    #endregion SwagDataGroup

    #region SwagColumnDistinctValues
    public class SwagColumnDistinctValues : SwagObservableDictionary<Object, SwagDataCell>
    {
        #region Private Members
        SwagDataColumn _column;
        #endregion Private Members

        #region Initialization
        public SwagColumnDistinctValues(SwagDataColumn column)
        {
            _column = column;
            if (_column.SwagDataTable != null && _column.SwagDataTable.DataTable != null)
            {
                _column.SwagDataTable.DataTable.DefaultView.ListChanged += DefaultView_ListChanged;
                Init();
            }
        }

        public void Init()
        {
            foreach (DataRow dr in _column.SwagDataTable.DataTable.Rows)
            {
                Object val = dr[_column.ColumnName];
                if (!this.ContainsKey(val))
                {
                    this.Add(val, new SwagDataCell() { Value = val, Column = _column });
                }
            }

            foreach (DataRowView drv in _column.SwagDataTable.DataTable.DefaultView)
            {
                Object val = drv[_column.ColumnName];
                this[val].Count++;
            }
        }
        #endregion Initialization

        #region Events
        private void DefaultView_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset)
            {
                Reset();
            }
        }
        #endregion Events

        #region Methods
        public void Reset()
        {
            foreach (KeyValuePair<Object, SwagDataCell> kvp in this)
            {
                kvp.Value.Count = 0;
            }

            foreach (DataRowView drv in _column.SwagDataTable.DataTable.DefaultView)
            {
                Object val = drv[_column.ColumnName];
                this[val].Count++;
            }
        }
        #endregion Methods
    }
    #endregion SwagColumnDistinctValues

    #region SwagDataColumnWPF
    public class SwagDataColumnWPF : SwagDataColumn
    {
        #region Private Members
        SwagColumnDistinctValues _distinctValuesSource;
        CollectionViewSource _distinctValues;
        Binding _binding;
        ICommand _applySearchFilterCommand, _applyListFilterCommand, _applyListValuesFilterCommand, _toggleListCheckAllCommand, _clearFilterCommand,
            _hideCommand, _removeCommand;
        #endregion Private Members

        #region Properties
        #region DataTemplate
        public String DataTemplate
        {
            get { return _dataTemplate; }
            set { SetValue(ref _dataTemplate, value); }
        }
        #endregion DataTemplate
        #region Binding
        [NotMapped]
        public Binding Binding
        {
            get
            {
                if (_binding == null)
                {
                    _binding = new Binding(_display);
                }
                return _binding;
            }
            set { SetValue(ref _binding, value); }
        }
        #endregion Binding
        #region DataGridColumn
        [NotMapped]
        public DataGridColumn DataGridColumn
        {
            get
            {
                if (!String.IsNullOrEmpty(DataTemplate))
                {
                    StringReader stringReader = new StringReader(DataTemplate);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    DataTemplate template = XamlReader.Load(xmlReader) as DataTemplate;
                    DataGridTemplateColumn dgtc = new DataGridTemplateColumn();
                    PropertyCopy.Copy(this, dgtc);
                    dgtc.CellTemplate = template;
                    return dgtc;
                }

                DataGridTextColumn dgc = new DataGridTextColumn();
                PropertyCopy.Copy(this, dgc);

                return dgc;
            }
        }
        #endregion DataGridColumn
        #region DataColumn
        [NotMapped]
        public DataColumn DataColumn
        {
            get
            {
                DataColumn dc = new DataColumn();
                if (String.IsNullOrEmpty(Expression))
                {
                    PropertyCopy.Copy(this, dc);
                }
                else
                {
                    PropertyCopy.Copy(this, dc, new List<string>() { "ReadOnly" });
                }
                return dc;
            }
        }
        #endregion DataColumn
        #region ApplySearchFilterCommand
        [NotMapped]
        public ICommand ApplySearchFilterCommand
        {
            get
            {
                return _applySearchFilterCommand ?? (_applySearchFilterCommand =
                    new RelayCommand(() =>
                    {
                        String filterFormat = "";

                        switch (_searchFilterMode)
                        {
                            default:
                            case FilterMode.CONTAINS:
                                filterFormat = "CONVERT([{0}], 'System.String') LIKE '%{1}%'";
                                break;
                            case FilterMode.EQUALS:
                                filterFormat = "CONVERT([{0}], 'System.String') = '{1}'";
                                break;
                            case FilterMode.STARTS_WITH:
                                filterFormat = "CONVERT([{0}], 'System.String') LIKE '{1}%'";
                                break;
                            case FilterMode.ENDS_WITH:
                                filterFormat = "CONVERT([{0}], 'System.String') LIKE '%{1}'";
                                break;
                        }

                        AppliedFilter = string.Format(filterFormat, ColumnName, SearchFilter);                        
                        ApplyDistinctValuesFilter();
                    }));
            }
        }
        #endregion ApplySearchFilterCommand
        #region ApplyListValuesFilterCommand
        [NotMapped]
        public ICommand ApplyListValuesFilterCommand
        {
            get
            {
                return _applyListValuesFilterCommand ?? (_applyListValuesFilterCommand =
                    new RelayCommand(() =>
                    {
                        ApplyDistinctValuesFilter();
                    }));
            }
        }
        #endregion ApplyListValuesFilterCommand
        #region ApplyListFilterCommand
        [NotMapped]
        public ICommand ApplyListFilterCommand
        {
            get
            {
                return _applyListFilterCommand ?? (_applyListFilterCommand =
                    new RelayCommand(() =>
                    {
                        List<String> lstItemPicks = new List<string>();

                        foreach (KeyValuePair<Object, SwagDataCell> kvp in _distinctValuesSource)
                        {
                            SwagDataCell cell = kvp.Value;
                            if (cell.IsChecked)
                            {
                                lstItemPicks.Add(string.Format("'{0}'", cell.Value.ToString().Replace("'", "''")));
                            }
                        }

                        string items = string.Join(",", lstItemPicks);
                        String filterFormat = "CONVERT([{0}], 'System.String') IN ({1})";
                        AppliedFilter = string.Format(filterFormat, ColumnName, items);
                        ApplyDistinctValuesFilter();
                    }));
            }
        }
        #endregion ApplyListFilterCommand
        #region ClearFilterCommand
        [NotMapped]
        public ICommand ClearFilterCommand
        {
            get
            {
                return _clearFilterCommand ?? (_clearFilterCommand =
                    new RelayCommand(() =>
                    {
                        AppliedFilter = "";
                        ApplyDistinctValuesFilter();
                    }));
            }
        }
        #endregion ClearFilterCommand
        #region HideCommand
        [NotMapped]
        public ICommand HideCommand
        {
            get
            {
                return _hideCommand ?? (_hideCommand =
                    new RelayCommand(() =>
                    {
                        IsVisible = false;
                    }));
            }
        }
        #endregion HideCommand
        #region RemoveCommand
        [NotMapped]
        public ICommand RemoveCommand
        {
            get
            {
                return _removeCommand ?? (_removeCommand =
                    new RelayCommand(() =>
                    {
                        Remove();
                    }));
            }
        }
        #endregion RemoveCommand
        #region ToggleListCheckAllCommand
        [NotMapped]
        public ICommand ToggleListCheckAllCommand
        {
            get
            {
                return _toggleListCheckAllCommand ?? (_toggleListCheckAllCommand =
                    new RelayCommand<Boolean>((isChecked) =>
                    {
                        if (SwagDataTable != null && SwagDataTable.DataTable != null)
                        {
                            foreach (KeyValuePair<Object, SwagDataCell> kvp in DistinctValuesView)
                            {
                                SwagDataCell cell = kvp.Value;
                                cell.IsChecked = isChecked;
                            }
                        }
                    }));
            }
        }
        #endregion ToggleListCheckAllCommand
        #region DistinctValuesView
        [NotMapped]
        public ICollectionView DistinctValuesView
        {
            get
            {
                if (_distinctValuesSource == null)
                {
                    _distinctValuesSource = new SwagColumnDistinctValues(this);
                    _distinctValues = new CollectionViewSource() { Source = _distinctValuesSource };
                    _distinctValues.SortDescriptions.Add(new SortDescription("Value.Value", ListSortDirection.Ascending));
                    ApplyDistinctValuesFilter();
                }

                return _distinctValues.View;
            }
        }
        #endregion DistinctValuesView
        #region ShowAllDistinctValues
        [NotMapped]
        public override Boolean ShowAllDistinctValues
        {
            get { return _showAllDistinctValues; }
            set
            {
                SetValue(ref _showAllDistinctValues, value);
                ApplyDistinctValuesFilter();
            }
        }
        #endregion ShowAllDistinctValues   
        #region ListCheckAll
        [NotMapped]
        public override Boolean ListCheckAll
        {
            get { return _listCheckAll; }
            set
            {
                SetValue(ref _listCheckAll, value);
                ToggleListCheckAllCommand.Execute(_listCheckAll);
            }
        }
        #endregion ListCheckAll
        #region AppliedFilter
        [NotMapped]
        public override String AppliedFilter
        {
            get { return _appliedFilter; }
            set
            {
                SetValue(ref _appliedFilter, value);
                OnPropertyChanged("HasAppliedFilter");
            }
        }
        #endregion AppliedFilter
        #endregion Properties

        #region Initialization
        public SwagDataColumnWPF()
        {
            PropertyChangedExtended += SwagDataColumn_PropertyChangedExtended;
        }

        private void SwagDataColumn_PropertyChangedExtended(object sender, PropertyChangedExtendedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "AppliedFilter":
                case "IsVisible":
                    SwagDataTable?.OnSwagItemChanged(this, e);
                    break;
            }
        }

        public SwagDataColumnWPF(DataColumn dc) : this()
        {
            PropertyCopy.Copy(dc, this);
        }

        public SwagDataColumnWPF(SwagDataColumn sdc) : this()
        {
            PropertyCopy.Copy(sdc, this);
        }
        #endregion Initialization

        #region Methods
        public void SetSequence(Int32 ordinal)
        {
            SwagDataTableWPF sdtWpf = (SwagDataTableWPF)SwagDataTable;
            sdtWpf.DataTable.Columns[ColumnName].SetOrdinal(ordinal);
            foreach (DataColumn dc in SwagDataTable.DataTable.Columns)
            {
                sdtWpf.Columns[dc.ColumnName].ColSeq = dc.Ordinal;
            }
            sdtWpf.ResetColumns();
            sdtWpf.InvalidateColumns();
            sdtWpf.Save();
        }

        public void Rename(String newColName)
        {
            SwagDataTableWPF sdtWpf = (SwagDataTableWPF)SwagDataTable;
            Int32 originalColSeq = ColSeq;
            //Update DataTable to have the new name
            sdtWpf.DataTable.Columns[ColumnName].ColumnName = newColName;
            sdtWpf.Columns.Remove(ColumnName);
            ColumnName = newColName;
            Binding = null;         //will need a new binding
            sdtWpf.Columns.Add(ColumnName, this);
            SetSequence(originalColSeq);        //When a column is added with 0 or lower sequence, it gets the value SwagDataTable.Columns.Count - 1
            sdtWpf.DelaySave = true;
            sdtWpf.ResetColumns();
            sdtWpf.InvalidateColumns();
            sdtWpf.InvalidateRows();
            sdtWpf.DelaySave = false;
            sdtWpf.Save();
        }

        public void Remove()
        {
            SwagDataTableWPF sdtWpf = (SwagDataTableWPF)SwagDataTable;
            sdtWpf.Columns.Remove(ColumnName);
            foreach (DataColumn dc in sdtWpf.DataTable.Columns)
            {
                sdtWpf.Columns[dc.ColumnName].ColSeq = dc.Ordinal;
            }
            sdtWpf.InvalidateColumns();
            sdtWpf.Save();
        }

        public void ApplyDistinctValuesFilter()
        {
            if (_distinctValues != null)
            {
                DistinctValuesView.Filter = item =>
                {
                    KeyValuePair<Object, SwagDataCell> kvp = (KeyValuePair<Object, SwagDataCell>)item;

                    if (String.IsNullOrEmpty(_listValuesFilter))
                    {
                        return kvp.Value.Count > 0 || _showAllDistinctValues;
                    }
                    else
                    {
                        return SearchHelper.Evaluate(kvp.Value.Value.ToString(), _listValuesFilter, false, _listValuesFilterMode, true)
                            && (kvp.Value.Count > 0 || _showAllDistinctValues);
                    }

                };
            }
        }

        #endregion Methods
    }
    #endregion SwagDataColumnWPF

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

    #region SwagDataTableWPF
    public class SwagDataTableWPF : SwagDataTable
    {
        #region Private Members
        SwagContext _context;
        Dictionary<DataRow, SwagDataRow> _dictChildren = new Dictionary<DataRow, SwagDataRow>();
        ICommand _filterCommand, _exportCommand, _importCommand, _applyColumnVisibilityCommand, _toggleColumnVisibilityCheckedAll, _applyColumnVisibilityFilterCommand,
            _toggleShowDebugCommand, _toggleShowColumnTotalsCommand, _resetColumnsCommand,
            _toggleColumnFiltersCheckedAll, _applyColumnFiltersFilterCommand, _clearColumnFiltersCommand;
        SwagSettingGroup _settings;
        SwagTabCollection _tabs;
        CollectionViewSource _childrenCollectionViewSource;
        CollectionViewSource _columnCollectionViewSource, _columnsVisibilityView, _columnsFilterView;
        SwagDataColumn _selectedColumn;
        SwagDataRowResult _selectedRow;
        #endregion Private Members

        #region Properties
        #region DataTable
        public override DataTable DataTable
        {
            get { return _dataTable; }
            set { SetDataTable(value); }
        }
        #endregion DataTable
        #region RowCount
        [NotMapped]
        public Int32 RowCount
        {
            get 
            {
                if (_dataTable == null)
                {
                    return 0;
                }
                return _dataTable.DefaultView.Count; 
            }
        }
        #endregion RowCount
        #region ColumnCount
        [NotMapped]
        public Int32 ColumnCount
        {
            get { return Columns.Count; }
        }
        #endregion ColumnCount
        #region ChildrenView
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView
        #region ColumnsView
        [NotMapped]
        [JsonIgnore]
        public ICollectionView ColumnsView
        {
            get { return _columnCollectionViewSource.View; }
        }
        #endregion ColumnsView
        #region ColumnsVisibilityView
        public ICollectionView ColumnsVisibilityView
        {
            get { return _columnsVisibilityView.View; }
        }
        #endregion ColumnsVisibilityView
        #region ColumnsFilterView
        public ICollectionView ColumnsFilterView
        {
            get { return _columnsFilterView.View; }
        }
        #endregion ColumnsSwagDataGridInstanceView
        #region ColumnVisibilityCheckAll
        [JsonIgnore]
        [NotMapped]
        public Boolean ColumnVisibilityCheckAll
        {
            get { return _columnVisibilityCheckAll; }
            set
            {
                SetValue(ref _columnVisibilityCheckAll, value);
                ToggleColumnVisibilityCheckedAll.Execute(_columnVisibilityCheckAll);
            }
        }
        #endregion ColumnVisibilityCheckAll
        #region ColumnFiltersCheckAll
        [JsonIgnore]
        [NotMapped]
        public Boolean ColumnFiltersCheckAll
        {
            get { return _columnFiltersCheckAll; }
            set
            {
                SetValue(ref _columnFiltersCheckAll, value);
                ToggleColumnFilterCheckedAll.Execute(_columnFiltersCheckAll);
            }
        }
        #endregion ColumnFiltersCheckAll
        #region FilterCommand
        public ICommand FilterCommand
        {
            get
            {
                return _filterCommand ?? (_filterCommand =
                    new RelayCommand(() =>
                    {
                        ICollectionView view = CollectionViewSource.GetDefaultView(DataTable.DefaultView);
                        if (view is BindingListCollectionView)      //Assuming you are DataView for now
                        {
                            BindingListCollectionView bindingView = (BindingListCollectionView)view;
                            //https://stackoverflow.com/questions/9385489/why-errors-when-filters-datatable-with-collectionview
                            bindingView.CancelEdit();

                            String combinedFilter = "";
                            foreach (KeyValuePair<string, SwagDataColumn> kvp in Columns)
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

                        _columnsFilterView.View.Refresh();
                    }));
            }
        }
        #endregion FilterCommand
        #region ExportCommand
        public ICommand ExportCommand
        {
            get
            {
                return _exportCommand ?? (_exportCommand =
                    new RelayCommand(() =>
                    {
                        IDataTableConverter converter = null;
                        string dialogFilter = "";
                        SwagTableExportType exportType = Settings["Export"]["Type"].GetValue<SwagTableExportType>();
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
                                sfd.FileName = Path.ChangeExtension(_dataTable.TableName, null);
                                sfd.Filter = "SQLite files (*.db;*.sqlite)|*.db;*.sqlite";
                                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    if (sfd.ShowDialog() ?? false)
                                    {
                                        DataSetSqliteFileConverter dsConverter = new DataSetSqliteFileConverter();
                                        DataSet ds = new DataSet();
                                        ds.Tables.Add(_dataTable.Copy());
                                        dsConverter.FromDataSet(null, ds, sfd.FileName);
                                    }
                                }));
                                return;
                            case SwagTableExportType.Sqlite_Command:
                                converter = new DataTableSqliteCommandConverter();
                                dialogFilter = "SQLite command files (*.cmd)|*.cmd";
                                break;
                        }

                        Object output = converter.FromDataTableToObject(new DataTableConvertParams(), _dataTable);

                        switch (Settings["Export"]["Destination"].GetValue<SwagTableDestinationType>())
                        {
                            case SwagTableDestinationType.Clipboard:
                                Clipboard.SetText(output.ToString());
                                break;
                            case SwagTableDestinationType.File:
                                SaveFileDialog sfd = new SaveFileDialog();
                                sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                                sfd.FileName = _dataTable.TableName;
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
                    }));
            }
        }
        #endregion ExportCommand
        #region ImportCommand
        public ICommand ImportCommand
        {
            get
            {
                return _importCommand ?? (_importCommand =
                    new RelayCommand(() =>
                    {
                        IDataTableConverter converter = null;
                        DataTableConvertParams cp = new DataTableConvertParams();

                        string dialogFilter = "";
                        String inputText = "";

                        switch (Settings["Import"]["Type"].GetValue<SwagTableImportType>())
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

                        switch (Settings["Import"]["Source"].GetValue<SwagTableSourceType>())
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
                        SetDataTable(dtInput);
                    }));
            }
        }
        #endregion ImportCommand
        #region ApplyColumnVisibilityFilterCommand
        public ICommand ApplyColumnVisibilityFilterCommand
        {
            get
            {
                return _applyColumnVisibilityFilterCommand ?? (_applyColumnVisibilityFilterCommand =
                    new RelayCommand(() =>
                    {
                        _columnsVisibilityView.View.Filter = (itm) =>
                        {
                            KeyValuePair<String, SwagDataColumn> kvp = (KeyValuePair<String, SwagDataColumn>)itm;
                            return SearchHelper.Evaluate(
                                kvp.Key,
                                _settings["ColumnEditor"]["Visibility"]["Search"]["Text"].GetValue<string>(),
                                false,
                                _settings["ColumnEditor"]["Visibility"]["Search"]["FilterMode"].GetValue<FilterMode>(),
                                false);
                        };
                    }));
            }
        }
        #endregion ApplyColumnVisibilityFilterCommand
        #region ToggleColumnVisibilityCheckedAll
        public ICommand ToggleColumnVisibilityCheckedAll
        {
            get
            {
                return _toggleColumnVisibilityCheckedAll ?? (_toggleColumnVisibilityCheckedAll =
                    new RelayCommand<Boolean>((checkAll) =>
                    {
                        foreach (KeyValuePair<String, SwagDataColumn> kvp in _columns)
                        {
                            SwagDataColumn col = kvp.Value;
                            col.IsCheckedVisibility = checkAll;
                        }
                    }));
            }
        }
        #endregion ToggleColumnVisibilityCheckedAll
        #region ApplyColumnVisibilityCommand
        public ICommand ApplyColumnVisibilityCommand
        {
            get
            {
                return _applyColumnVisibilityCommand ?? (_applyColumnVisibilityCommand =
                    new RelayCommand(() =>
                    {
                        foreach (KeyValuePair<String, SwagDataColumn> kvp in _columns)
                        {
                            SwagDataColumn col = kvp.Value;
                            col.IsVisible = col.IsCheckedVisibility;
                        }
                    }));
            }
        }
        #endregion ApplyColumnVisibilityCommand
        #region ApplyColumnFiltersFilterCommand
        public ICommand ApplyColumnFiltersFilterCommand
        {
            get
            {
                return _applyColumnFiltersFilterCommand ?? (_applyColumnFiltersFilterCommand =
                    new RelayCommand(() =>
                    {
                        _columnsFilterView.View.Filter = (itm) =>
                        {
                            KeyValuePair<String, SwagDataColumn> kvp = (KeyValuePair<String, SwagDataColumn>)itm;
                            return kvp.Value.HasAppliedFilter && (SearchHelper.Evaluate(
                                kvp.Key,
                                _settings["ColumnEditor"]["Filters"]["Search"]["Text"].GetValue<string>(),
                                false,
                                _settings["ColumnEditor"]["Filters"]["Search"]["FilterMode"].GetValue<FilterMode>(),
                                false) || SearchHelper.Evaluate(
                                kvp.Value.AppliedFilter,
                                _settings["ColumnEditor"]["Filters"]["Search"]["Text"].GetValue<string>(),
                                false,
                                _settings["ColumnEditor"]["Filters"]["Search"]["FilterMode"].GetValue<FilterMode>(),
                                false));
                        };
                    }));
            }
        }
        #endregion ApplyColumnFiltersFilterCommand
        #region ClearColumnFiltersCommand
        public ICommand ClearColumnFiltersCommand
        {
            get
            {
                return _clearColumnFiltersCommand ?? (_clearColumnFiltersCommand =
                    new RelayCommand(() =>
                    {
                        foreach (KeyValuePair<String, SwagDataColumn> kvp in _columns)
                        {
                            SwagDataColumn col = kvp.Value;
                            if (col.IsCheckedFilter)
                            {
                                col.AppliedFilter = "";
                            }
                        }

                        FilterCommand.Execute(null);
                    }));
            }
        }
        #endregion ClearColumnFiltersCommand
        #region ToggleColumnFilterCheckedAll
        public ICommand ToggleColumnFilterCheckedAll
        {
            get
            {
                return _toggleColumnFiltersCheckedAll ?? (_toggleColumnFiltersCheckedAll =
                    new RelayCommand<Boolean>((checkAll) =>
                    {
                        foreach (KeyValuePair<String, SwagDataColumn> kvp in _columns)
                        {
                            SwagDataColumn col = kvp.Value;
                            col.IsCheckedFilter = checkAll;
                        }
                    }));
            }
        }
        #endregion ToggleColumnFilterCheckedAll
        #region ResetColumnsCommand
        public ICommand ResetColumnsCommand
        {
            get
            {
                return _resetColumnsCommand ?? (_resetColumnsCommand =
                    new RelayCommand(() =>
                    {
                        ResetColumns();
                    }));
            }
        }
        #endregion ResetColumnsCommand
        #region ToggleShowDebugCommand
        public ICommand ToggleShowDebugCommand
        {
            get
            {
                return _toggleShowDebugCommand ?? (_toggleShowDebugCommand =
                    new RelayCommand(() =>
                    {
                        ShowDebug = !ShowDebug;
                    }));
            }
        }
        #endregion ToggleShowDebugCommand
        #region ToggleShowColumnTotalsCommand
        public ICommand ToggleShowColumnTotalsCommand
        {
            get
            {
                return _toggleShowColumnTotalsCommand ?? (_toggleShowColumnTotalsCommand =
                    new RelayCommand(() =>
                    {
                        ShowColumnTotals = !ShowColumnTotals;
                    }));
            }
        }
        #endregion ToggleShowColumnTotalsCommand
        #region Settings
        public SwagSettingGroup Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new SwagSettingGroup();
                    _settings["ColumnEditor"] = new SwagSettingGroup() { Icon = PackIconCustomKind.TableColumnEdit };
                    _settings["ColumnEditor"]["Visibility"] = new SwagSettingGroup() { Icon = PackIconCustomKind.Eye };
                    _settings["ColumnEditor"]["Visibility"]["Search"] = new SwagSettingGroup() { Icon = PackIconCustomKind.Search };
                    _settings["ColumnEditor"]["Visibility"]["Search"]["Text"] = new SwagSetting<String>() { Icon = PackIconCustomKind.KeyValue };
                    _settings["ColumnEditor"]["Visibility"]["Search"]["FilterMode"] = new SwagSetting<FilterMode>() { SettingType = SettingType.DropDown, Value = FilterMode.CONTAINS, Icon = PackIconCustomKind.Filter, ItemsSource = (FilterMode[])Enum.GetValues(typeof(FilterMode)) };
                    _settings["ColumnEditor"]["Filters"] = new SwagSettingGroup() { Icon = PackIconCustomKind.TableColumnFilter };
                    _settings["ColumnEditor"]["Filters"]["Search"] = new SwagSettingGroup() { Icon = PackIconCustomKind.Search };
                    _settings["ColumnEditor"]["Filters"]["Search"]["Text"] = new SwagSetting<String>() { Icon = PackIconCustomKind.KeyValue };
                    _settings["ColumnEditor"]["Filters"]["Search"]["FilterMode"] = new SwagSetting<FilterMode>() { SettingType = SettingType.DropDown, Value = FilterMode.CONTAINS, Icon = PackIconCustomKind.Filter, ItemsSource = (FilterMode[])Enum.GetValues(typeof(FilterMode)) };
                    _settings["Search"] = new SwagSettingGroup() { Icon = PackIconCustomKind.TableSearch };
                    _settings["Search"]["Text"] = new SwagSetting<String>() { Icon = PackIconCustomKind.KeyValue };
                    _settings["Search"]["FilterMode"] = new SwagSetting<FilterMode>() { SettingType = SettingType.DropDown, Value = FilterMode.CONTAINS, Icon = PackIconCustomKind.Filter, ItemsSource = (FilterMode[])Enum.GetValues(typeof(FilterMode)) };
                    _settings["Export"] = new SwagSettingGroup() { Icon = PackIconCustomKind.Export };
                    _settings["Export"]["Type"] = new SwagSetting<SwagTableExportType>() { SettingType = SettingType.DropDown, Value = SwagTableExportType.Csv, Icon = PackIconCustomKind.ExportType, ItemsSource = (SwagTableExportType[])Enum.GetValues(typeof(SwagTableExportType)) };
                    _settings["Export"]["Destination"] = new SwagSetting<SwagTableDestinationType>() { SettingType = SettingType.DropDown, Value = SwagTableDestinationType.Clipboard, Icon = PackIconCustomKind.Destination, ItemsSource = (SwagTableDestinationType[])Enum.GetValues(typeof(SwagTableDestinationType)) };
                    _settings["Import"] = new SwagSettingGroup() { Icon = PackIconCustomKind.Import };
                    _settings["Import"]["Type"] = new SwagSetting<SwagTableImportType>() { SettingType = SettingType.DropDown, Value = SwagTableImportType.Tsv, Icon = PackIconCustomKind.ExportType, ItemsSource = (SwagTableImportType[])Enum.GetValues(typeof(SwagTableImportType)) };
                    _settings["Import"]["Source"] = new SwagSetting<SwagTableSourceType>() { SettingType = SettingType.DropDown, Value = SwagTableSourceType.Clipboard, Icon = PackIconCustomKind.Destination, ItemsSource = (SwagTableSourceType[])Enum.GetValues(typeof(SwagTableSourceType)) };
                    InitSettings();
                }
                return _settings;
            }
            set
            {
                SetValue(ref _settings, value);
            }
        }
        #endregion Settings
        #region Tabs
        public SwagTabCollection Tabs
        {
            get
            {
                if (_tabs == null)
                {
                    _tabs = new SwagTabCollection();
                    _tabs["ColumnEditor"] = new SwagTabCollection() { Icon = PackIconCustomKind.TableColumnEdit, Display = "Column Editor" };
                    _tabs["Search"] = new SwagTabItem() { Icon = PackIconCustomKind.TableSearch };
                    _tabs["Export"] = new SwagTabItem() { Icon = PackIconCustomKind.TableExport };
                    _tabs["Import"] = new SwagTabItem() { Icon = PackIconCustomKind.TableImport };
                    _tabs["Settings"] = new SwagTabItem() { Icon = PackIconCustomKind.TableSettings };
                    _tabs["ColumnEditor"]["Visibility"] = new SwagTabItem() { Icon = PackIconCustomKind.TableColumnVisibility };
                    _tabs["ColumnEditor"]["Filters"] = new SwagTabItem() { Icon = PackIconCustomKind.TableColumnFilter };
                    _tabs["ColumnEditor"]["Add"] = new SwagTabItem() { Icon = PackIconCustomKind.TableColumnAdd };
                    _tabs["ColumnEditor"]["View"] = new SwagTabItem() { Icon = PackIconCustomKind.TableColumnView };
                    InitTabs();
                }
                return _tabs;
            }
            set
            {
                SetValue(ref _tabs, value);
            }
        }
        #endregion Tabs
        #region DelaySave
        [NotMapped]
        [JsonIgnore]
        public Boolean DelaySave { get; set; } = false;
        #endregion DelaySave
        #region ShowDebug
        [NotMapped]
        [JsonIgnore]
        public Boolean ShowDebug
        {
            get { return _showDebug; }
            set { SetValue(ref _showDebug, value); }
        }
        #endregion ShowDebug
        #region ShowColumnTotals
        [NotMapped]
        [JsonIgnore]
        public Boolean ShowColumnTotals
        {
            get { return _showColumnTotals; }
            set { SetValue(ref _showColumnTotals, value); }
        }
        #endregion ShowColumnTotals
        #region SelectedColumn
        [NotMapped]
        [JsonIgnore]
        public SwagDataColumn SelectedColumn
        {
            get { return _selectedColumn; }
            set { SetValue(ref _selectedColumn, value); }
        }
        #endregion SelectedColumn
        #region SelectedRow
        [NotMapped]
        [JsonIgnore]
        public SwagDataRowResult SelectedRow
        {
            get { return _selectedRow; }
            set { SetValue(ref _selectedRow, value); }
        }
        #endregion SelectedRow
        #endregion Properties

        #region Initialization
        public SwagDataTableWPF()
        {
            _columns = new SwagObservableOrderedDictionary<String, SwagDataColumn>();
            _children.CollectionChanged += _children_CollectionChanged;
            InitViews();
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagData newItem in e.NewItems)
                {
                    newItem.CanUndo = false;
                    newItem.Parent = this;
                    newItem.CanUndo = true;
                }
            }
        }

        public SwagDataTableWPF(SwagDataTable swagDataTable) : this()
        {
            PropertyCopy.Copy(swagDataTable, this);
            if (_columns == null)
            {
                _columns = new SwagObservableOrderedDictionary<String, SwagDataColumn>();
            }
            _children.CollectionChanged += _children_CollectionChanged;
        }

        public SwagDataTableWPF(DataTable dt) : this()
        {
            SetDataTable(dt);
        }

        public void SetDataTable(DataTable dt, Boolean silent = false)
        {
            if (dt == null)
            {
                return;
            }

            //This is not the best place for but cannot put this in the constructor because EF overwrites _columns
            //Also cannot place in setter because it is used to indicate the entity has changed
            ((SwagObservableOrderedDictionary<String, SwagDataColumn>)_columns).CollectionChanged += viewModel_Columns_CollectionChanged;

            SetValue(ref _dataTable, dt, "DataTable");

            if (!silent)
            {
                #region Clear Columns and Rows for the given context
                if (_context != null)
                {
                    SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                    foreach (SwagData swagData in Children)
                    {
                        work.Data.Delete(swagData);
                    }

                    work.DataTables.Update(this);
                    work.Complete();
                }
                #endregion Clear Columns and Rows for the given context

                #region Clear Columns and Rows for instance
                _columns.Clear();
                Children.Clear();
                #endregion Clear Columns and Rows for instance

                #region Add Columns and Rows for instance
                foreach (DataColumn dc in dt.Columns)
                {
                    SwagDataColumn sdc = new SwagDataColumnWPF() { ColumnName = dc.ColumnName, DataType = dc.DataType };
                    sdc.SwagDataTable = this;
                    sdc.DataTypeString = sdc.DataTypeString;
                    Children.Add(sdc);
                    _columns.Add(dc.ColumnName, sdc);
                }

                _dictChildren.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    SwagDataRow row = new SwagDataRow(dr);
                    row.Value = row.Value;
                    row.ValueTypeString = row.ValueTypeString;
                    Children.Add(row);
                    _dictChildren.Add(row.DataRow, row);
                }
                #endregion Add Columns and Rows for instance

                #region Save for the given context
                if (_context != null)
                {
                    SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                    work.DataTables.Update(this);
                    work.Complete();
                }
                #endregion Save for the given context
            }
            else
            {
                _dictChildren.Clear();
                foreach (SwagData swagData in Children)
                {
                    if (swagData is SwagDataRow dataRow)
                    {
                        _dictChildren.Add(dataRow.DataRow, dataRow);
                    }
                }
            }

            _dataTable.RowChanged += dataTable_RowChanged;
            _dataTable.DefaultView.ListChanged += dataView_ListChanged;
            ResetColumns();
            dataView_ListChanged(null, null);
        }

        public override void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedExtendedEventArgs e)
        {
            switch (e.Object)
            {
                case SwagDataColumn sdc:
                    e.Message = $"{this.Name}.{sdc.ColumnName}({e.OldValue}) => {e.NewValue}";
                    switch (e.PropertyName)
                    {
                        case "IsVisible":
                            _columnCollectionViewSource.View.Refresh();
                            break;
                        case "AppliedFilter":
                            FilterCommand.Execute(null);
                            break;
                    }
                    break;
                //case SwagDataRow sdr:
                //    switch (e.PropertyName)
                //    {
                //        case "Value":
                //            break;
                //    }
                //    break;
            }

            base.OnSwagItemChanged(swagItem, e);
        }
        #endregion Initialization

        #region Column Events
        private void viewModel_Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (KeyValuePair<String, SwagDataColumn> sdcKvp in e.NewItems)
                    {
                        if (!_dataTable.Columns.Contains(sdcKvp.Key))
                        {
                            _dataTable.Columns.Add(((SwagDataColumnWPF)sdcKvp.Value).DataColumn);
                            _dataTable.Columns[sdcKvp.Key].SetOrdinal(e.NewStartingIndex);
                            this.Children.Add(sdcKvp.Value);
                        }

                        if (sdcKvp.Value.ColSeq <= 0)
                        {
                            sdcKvp.Value.ColSeq = _columns.Count - 1;
                        }
                        sdcKvp.Value.Parent = this;
                        InvalidateRows();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (KeyValuePair<String, SwagDataColumn> sdcKvp in e.OldItems)
                    {
                        if (_dataTable.Columns.Contains(sdcKvp.Key))
                        {
                            _dataTable.Columns.Remove(sdcKvp.Key);
                            //This orphans the child
                            this.Children.Remove(sdcKvp.Value);
                            if (_context != null)
                            {
                                SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                                work.Data.Delete(sdcKvp.Value);
                                work.Complete();
                            }
                        }
                    }
                    InvalidateRows();
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }
            InvalidateColumns();
            OnPropertyChanged("ColumnCount");
        }
        #endregion Column Events

        #region RowEvents
        private void dataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!DelaySave)
            {
                SwagDataRow row = _dictChildren[e.Row];
                row.ObjValue = row.ObjValue;

                if (_context != null)
                {
                    SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                    work.Data.Update(row);
                    work.Complete();
                }
            }
        }

        private void dataView_ListChanged(object sender, ListChangedEventArgs e)
        {
            foreach (KeyValuePair<String, SwagDataColumn> kvp in _columns)
            {
                if (_dataTable.Columns.Contains(kvp.Key))
                {
                    SwagDataColumn swagDataColumn = kvp.Value;
                    Decimal colTotal = 0.0m;
                    if (swagDataColumn.DataType.IsNumericType())
                    {
                        foreach (DataRowView drv in _dataTable.DefaultView)
                        {
                            Object objVal = drv[swagDataColumn.ColumnName];
                            if (objVal != null && objVal != DBNull.Value)
                            {
                                Decimal val = Decimal.Parse(objVal.ToString());
                                colTotal += val;
                            }
                        }
                        swagDataColumn.Total = colTotal;
                    }
                }
            }
            OnPropertyChanged("RowCount");
        }
        #endregion RowEvents

        #region Context Methods
        public void SetContext(SwagContext context)
        {
            _context = context;
        }

        public void Save()
        {
            if (!DelaySave && _context != null)
            {
                SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                work.DataTables.Update(this);
                work.Complete();
            }
        }

        public void InvalidateColumns()
        {
            DelaySave = true;
            foreach (SwagData swagData in Children)
            {
                switch (swagData)
                {
                    case SwagDataColumn swagDataColumn:
                        swagDataColumn.CanUndo = false;
                        swagDataColumn.DataTypeString = swagDataColumn.DataTypeString;
                        swagDataColumn.Value = swagDataColumn.Value;
                        swagDataColumn.CanUndo = true;
                        break;
                }
            }
            DelaySave = false;
        }

        public void InvalidateRows()
        {
            DelaySave = true;
            foreach (SwagData swagData in Children)
            {
                switch (swagData)
                {
                    case SwagDataRow swagDataRow:
                        swagDataRow.CanUndo = false;
                        swagDataRow.Value = swagDataRow.Value;
                        swagDataRow.CanUndo = true;
                        break;
                }
            }
            DelaySave = false;
        }

        #endregion Context Methods

        #region Methods
        public void InitSettings()
        {
            Settings.SwagItemChanged += _settings_SwagItemChanged;
        }

        private void _settings_SwagItemChanged(object sender, SwagItemChangedEventArgs e)
        {
            switch (e.PropertyChangedArgs.PropertyName)
            {
                case "Value":
                case "IsExpanded":
                    if (_context != null)
                    {
                        SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                        this.Settings = Settings;
                        work.DataTables.Update(this);
                        work.Complete();
                    }
                    break;
            }
        }

        public void InitTabs()
        {
            if (_tabs != null)
            {
                SwagItemPreOrderIterator<SwagTabItem> iterator = _tabs.CreateIterator();
                for (SwagTabItem tabItem = iterator.First(); !iterator.IsDone; tabItem = iterator.Next())
                {
                    tabItem.ViewModel = this;
                }
                _tabs.SwagItemChanged += _tabs_SwagItemChanged;
                _tabs.PropertyChangedExtended += _tabs_PropertyChangedExtended;
            }
        }

        private void _tabs_SwagItemChanged(object sender, SwagItemChangedEventArgs e)
        {
            _tabs_PropertyChangedExtended(sender, e.PropertyChangedArgs);
        }

        private void _tabs_PropertyChangedExtended(object sender, PropertyChangedExtendedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedIndex":
                    if (_context != null)
                    {
                        SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                        this.Tabs = Tabs;
                        work.DataTables.Update(this);
                        work.Complete();
                    }
                    break;
            }
        }

        public void ResetColumns()
        {
            DelaySave = true;
            _columnCollectionViewSource.Source = _columnsVisibilityView.Source = _columnsFilterView.Source = _columns;
            _columnCollectionViewSource.View.Refresh();
            _columnsVisibilityView.View.Refresh();
            _columnsFilterView.View.Refresh();
            OnPropertyChanged("ColumnsView");
            OnPropertyChanged("ColumnCount");
            DelaySave = false;
        }

        public void InitViews()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _columnCollectionViewSource = new CollectionViewSource() { Source = _columns };
            _columnCollectionViewSource.SortDescriptions.Add(new SortDescription("Value.ColSeq", ListSortDirection.Ascending));
            _columnCollectionViewSource.View.Filter = (itm) =>
            {
                KeyValuePair<String, SwagDataColumn> kvp = (KeyValuePair<String, SwagDataColumn>)itm;
                return kvp.Value.IsVisible;
            };
            _columnsVisibilityView = new CollectionViewSource() { Source = _columns };
            _columnsFilterView = new CollectionViewSource() { Source = _columns };
            _columnsFilterView.View.Filter = (itm) =>
            {
                KeyValuePair<String, SwagDataColumn> kvp = (KeyValuePair<String, SwagDataColumn>)itm;
                return kvp.Value.HasAppliedFilter;
            };
        }

        public override DataSet GetDataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = _dataTable.Copy();
            ds.Tables.Add(dt);
            return ds;
        }
        #endregion Methods
    }
    #endregion SwagDataTableWPF

    #region SwagDataSet
    public class SwagDataSetWPF : SwagDataSet
    {
        #region Private Members
        SwagSettingGroup _settings;
        SwagTabCollection _tabs;
        ICommand _filterTabsCommand, _addDataSetCommand, _addDataTableCommand;
        SwagData _selectedChild;
        CollectionViewSource _childrenCollectionViewSource;
        #endregion Private Members

        #region Properties
        #region Settings
        public SwagSettingGroup Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new SwagSettingGroup();
                    _settings["Tabs"] = new SwagSettingGroup() { Icon = PackIconCustomKind.TableSearch };
                    _settings["Tabs"]["Search"] = new SwagSettingGroup() { Icon = PackIconCustomKind.Search };
                    _settings["Tabs"]["Search"]["Text"] = new SwagSettingString() { Icon = PackIconCustomKind.KeyValue };
                    _settings["Tabs"]["Search"]["FilterMode"] = new SwagSetting<FilterMode>() { SettingType = SettingType.DropDown, Value = FilterMode.CONTAINS, Icon = PackIconCustomKind.Filter, ItemsSource = (FilterMode[])Enum.GetValues(typeof(FilterMode)) };
                    _settings["Search"] = new SwagSettingGroup() { Icon = PackIconCustomKind.GlobalSearch };
                    _settings["Search"]["Text"] = new SwagSetting<String>() { Icon = PackIconCustomKind.KeyValue };
                    _settings["Search"]["FilterMode"] = new SwagSetting<FilterMode>() { SettingType = SettingType.DropDown, Value = FilterMode.CONTAINS, Icon = PackIconCustomKind.Filter, ItemsSource = (FilterMode[])Enum.GetValues(typeof(FilterMode)) };
                }
                return _settings;
            }
            set
            {
                SetValue(ref _settings, value);
            }
        }
        #endregion Settings
        #region Tabs
        public SwagTabCollection Tabs
        {
            get
            {
                if (_tabs == null)
                {
                    _tabs = new SwagTabCollection();
                    _tabs["Tabs"] = new SwagTabItem() { Icon = PackIconCustomKind.TableSearch, ViewModel = this };
                    _tabs["Search"] = new SwagTabItem() { Icon = PackIconCustomKind.GlobalSearch, ViewModel = this };
                    _tabs["Settings"] = new SwagTabItem() { Icon = PackIconCustomKind.Settings, ViewModel = this };
                }
                return _tabs;
            }
            set
            {
                SetValue(ref _tabs, value);
                _tabs["Tabs"].ViewModel = this;
                _tabs["Search"].ViewModel = this;
                _tabs["Settings"].ViewModel = this;
            }
        }
        #endregion Tabs
        #region SelectedChild
        [NotMapped]
        [JsonIgnore]
        public SwagData SelectedChild
        {
            get { return _selectedChild; }
            set { SetValue(ref _selectedChild, value); }
        }
        #endregion SelectedChild
        #region ChildrenView
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView
        #region FilterTabsCommand
        public ICommand FilterTabsCommand
        {
            get
            {
                return _filterTabsCommand ?? (_filterTabsCommand =
                    new RelayCommand(() =>
                    {
                        String filter = _settings["Tabs"]["Search"]["Text"].GetValue<String>();
                        FilterMode filterMode = _settings["Tabs"]["Search"]["FilterMode"].GetValue<FilterMode>();
                        ChildrenView.Filter = (item) =>
                        {
                            SwagData swagData = (SwagData)item;
                            Boolean itemMatch = SearchHelper.Evaluate(swagData.Display, filter, false, filterMode, false);
                            Boolean childDataSetMatch = false;

                            if (swagData is SwagDataSetWPF)
                            {
                                SwagDataSetWPF childDataSet = (SwagDataSetWPF)swagData;
                                childDataSet.Settings["Tabs"]["Search"]["Text"].SetValue(filter);
                                childDataSet.Settings["Tabs"]["Search"]["FilterMode"].SetValue(filterMode);
                                childDataSet.FilterTabsCommand.Execute(null);
                                childDataSetMatch = !childDataSet.ChildrenView.IsEmpty;
                            }

                            return itemMatch || childDataSetMatch;
                        };
                    }));
            }
        }
        #endregion FilterTabsCommand
        #region AddDataSetCommand
        public ICommand AddDataSetCommand
        {
            get
            {
                return _addDataSetCommand ?? (_addDataSetCommand =
                    new RelayCommand(() =>
                    {
                        SwagDataSetWPF newDataSet = new SwagDataSetWPF();
                        newDataSet.Display = $"Set {this.Children.Count + 1}";
                        Children.Add(newDataSet);
                    }));
            }
        }
        #endregion AddDataSetCommand
        #region AddDataTableCommand
        public ICommand AddDataTableCommand
        {
            get
            {
                return _addDataTableCommand ?? (_addDataTableCommand =
                    new RelayCommand(() =>
                    {
                        SwagDataTableWPF newDataTable = new SwagDataTableWPF();
                        newDataTable.Display = $"Table {this.Children.Count + 1}";
                        Children.Add(newDataTable);
                    }));
            }
        }
        #endregion AddDataTableCommand
        #endregion Properties

        #region Initialization
        public SwagDataSetWPF()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
        }

        public SwagDataSetWPF(DataSet dataSet) : this()
        {
            foreach (DataTable dt in dataSet.Tables)
            {
                Children.Add(new SwagDataTableWPF(dt) { Display = dt.TableName });
            }
        }
        #endregion Initialization

        #region Methods
        public void LoadFiles(IEnumerable<String> files, IEnumerable<KeyValuePairViewModel<String, ParseViewModel>> parseMappers)
        {
            foreach (String file in files)
            {
                SwagLogger.LogStart(this, "Load {file}", file);
                SwagData child = SwagDataHelper.FromFile(file, parseMappers);
                SwagLogger.LogEnd(this, "Load {file}", file);

                if (child != null)
                {
                    Children.Add(child);
                }
                else
                {
                    SwagLogger.Log("Load {file} did not yield data (unsupported extenstion).", file);
                }
            }
        }

        public override DataSet GetDataSet()
        {
            DataSet ds = new DataSet(Display);
            foreach (SwagData swagData in ChildrenView)
            {
                switch (swagData)
                {
                    case SwagDataTable swagDataTable:
                        foreach (DataTable dt in swagDataTable.GetDataSet().Tables)
                        {
                            DataTable dtCopy = dt.Copy();
                            ds.Tables.Add(dtCopy);
                        }
                        break;
                    case SwagDataSet swagDataSet:
                        foreach (DataTable dt in swagDataSet.GetDataSet().Tables)
                        {
                            DataTable dtCopy = dt.Copy();
                            dtCopy.TableName = $"{swagDataSet.Display}.{dt.TableName}";
                            ds.Tables.Add(dtCopy);
                        }
                        break;
                }
            }
            return ds;
        }
        #endregion Methods
    }
    #endregion SwagDataSet

    #region SwagDataHelper
    public static class SwagDataHelper
    {
        public static SwagData FromFile(String file, IEnumerable<KeyValuePairViewModel<String, ParseViewModel>> parseMappers)
        {
            String filename = Path.GetFileName(file);
            String ext = Path.GetExtension(file).TrimStart('.');

            KeyValuePairViewModel<String, ParseViewModel> parseMapper = parseMappers.FirstOrDefault(pm => pm.Key != null && pm.Key.ToLower() == ext.ToLower());
            if (parseMapper != null)
            {
                switch (parseMapper.Value.ParseStrategy)
                {
                    case ParseStrategy.Csv:
                    case ParseStrategy.Dbf:
                        DataTableConvertContext dataTableConvertContext = new DataTableConvertContext();
                        switch (parseMapper.Value.ParseStrategy)
                        {
                            case ParseStrategy.Csv:
                                dataTableConvertContext.Converter = new DataTableCsvFileConverter();
                                break;
                            case ParseStrategy.Dbf:
                                dataTableConvertContext.Converter = new DataTableDbfFileConverter();
                                break;
                        }
                        dataTableConvertContext.Params = new DataTableConvertParams();
                        PropertyCopy.Copy(parseMapper.Value, dataTableConvertContext.Params);
                        DataTableConverterHelper.ConverterFileContexts[ext] = dataTableConvertContext;
                        break;
                    case ParseStrategy.Xml:
                    case ParseStrategy.Json:
                        DataSetConvertContext dataSetConvertContext = new DataSetConvertContext();
                        switch (parseMapper.Value.ParseStrategy)
                        {
                            case ParseStrategy.Xml:
                                dataSetConvertContext.Converter = new DataSetXmlFileConverter();
                                break;
                            case ParseStrategy.Json:
                                dataSetConvertContext.Converter = new DataSetJsonFileConverter();
                                break;
                        }
                        dataSetConvertContext.Params = new DataSetConvertParams();
                        PropertyCopy.Copy(parseMapper.Value, dataSetConvertContext.Params);
                        DataSetConverterHelper.ConverterFileContexts[ext] = dataSetConvertContext;
                        break;
                }
            }

            DataSetConvertContext dataSetContext = DataSetConverterHelper.ConverterFileContexts[ext];
            if (dataSetContext != null)
            {
                return new SwagDataSetWPF(dataSetContext.ToDataSet(file)) { Display = filename };
            }

            DataTableConvertContext dataTableContext = DataTableConverterHelper.ConverterFileContexts[ext];
            if (dataTableContext != null)
            {
                return new SwagDataTableWPF(dataTableContext.ToDataTable(file)) { Display = filename };
            }

            return null;
        }
    }
    #endregion SwagDataHelper
}
