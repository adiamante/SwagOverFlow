using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Data;
using SwagOverflowWPF.Collections;
using SwagOverflowWPF.Commands;
using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Iterator;
using SwagOverflowWPF.Repository;
using SwagOverflowWPF.UI;
using SwagOverflowWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;

namespace SwagOverflowWPF.ViewModels
{
    #region SwagData
    public class SwagData : SwagItem<SwagDataGroup, SwagData>
    {
    }
    #endregion SwagData

    #region SwagDataGroup
    public class SwagDataGroup : SwagData, ISwagParent<SwagDataGroup, SwagData>
    {
        #region Private/Protected Members
        protected Boolean _listening = true;
        CollectionViewSource _childrenCollectionViewSource;
        protected ObservableCollection<SwagData> _children = new ObservableCollection<SwagData>();
        #endregion Private/Protected Members

        #region Events
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public virtual void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedExtendedEventArgs e)
        {
            if (Listening)
            {
                SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs() { SwagItem = swagItem, PropertyChangedArgs = e });
                Parent?.OnSwagItemChanged(swagItem, e);
            }
        }
        #endregion Events

        #region Properties
        #region Listening
        public virtual Boolean Listening
        {
            get { return _listening; }
            set { SetValue(ref _listening, value); }
        }
        #endregion Listening
        #region Children
        public ObservableCollection<SwagData> Children
        {
            get { return _children; }
            set { SetValue(ref _children, value); }
        }
        #endregion Children
        #region ChildrenView
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView
        #endregion Properties

        #region Initialization
        public SwagDataGroup()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagData newItem in e.NewItems)
                {
                    newItem.Parent = this;
                    if (newItem.Sequence <= 0)
                    {
                        newItem.Sequence = this.Children.Count;
                    }
                }
            }
        }
        #endregion Initialization
    }
    #endregion SwagDataGroup

    #region SwagData<T>
    public class SwagData<T> : SwagData
    {
        #region Private/Protected Members
        protected T _value;
        #endregion Private/Protected Members

        #region Properties
        #region Value
        public virtual T Value
        {
            get
            {
                if (ValueType != null && _objValue != null && ValueType != _objValue.GetType())
                {
                    if (ValueType == typeof(Boolean))
                    {
                        _objValue = Boolean.Parse(_objValue.ToString());
                    }
                    else if (ValueType == typeof(String))
                    {
                        _objValue = _objValue.ToString();
                    }
                    else
                    {
                        _objValue = JsonConvert.DeserializeObject(_objValue.ToString(), ValueType);
                    }
                }

                if (_objValue != null && _value == null)
                {
                    _value = (T)_objValue;
                }

                return _value;
            }
            set
            {
                _objValue = value;
                OnPropertyChanged("ObjValue");      //Normal (if this was extended SwagCommandManager detects double)
                SetValue(ref _value, value);        //Extended
            }
        }
        #endregion Value
        #region ValueType
        public override Type ValueType { get { return typeof(T); } set { } }
        #endregion ValueType
        #region ValueTypeString
        public override String ValueTypeString
        {
            get { return JsonHelper.ToJsonString(typeof(T)); }
            set { SetValue(ref _valueTypeString, value); }
        }
        #endregion ValueTypeString
        #endregion Properties

        #region Initialization
        public SwagData() : base()
        {
        }
        #endregion Initialization
    }
    #endregion SwagData<T>

    #region SwagDataRow
    public class SwagDataRow : SwagData<JObject>
    {
        #region Private/Protected Members
        DataRow _dataRow;
        JObject _previousValue;
        #endregion Private/Protected Members

        #region Properties
        #region DataRow
        public DataRow DataRow
        {
            get { return _dataRow; }
            set
            {
                SetValue(ref _dataRow, value);
                if (_dataRow != null)
                {
                    _previousValue = _value;
                    _value = this.ToJObject();
                }
            }
        }
        #endregion DataRow
        #region Value
        public override JObject Value
        {
            get
            {
                if (_dataRow != null)
                {
                    _previousValue = _value;
                    _value = this.ToJObject();
                    _objValue = _value;
                }
                return _value;
            }
            set
            {
                SetValue(ref _previousValue, value, false);
                _objValue = _value;

                if (Parent != null && Parent.Listening)
                {
                    Parent.Listening = false;

                    SwagDataTable sdtParent = (SwagDataTable)Parent;
                    foreach (KeyValuePair<String, JToken> kvp in value)
                    {
                        if (kvp.Value is JValue)
                        {
                            JValue jVal = (JValue)kvp.Value;
                            if (_dataRow[kvp.Key] != jVal.Value && !sdtParent.Columns[kvp.Key].ReadOnly)
                            {
                                _dataRow[kvp.Key] = jVal.Value;
                            }
                        }
                    }
                    Parent.Listening = true;
                }
            }
        }
        #endregion Value
        #endregion Properties

        #region Initialization
        public SwagDataRow()
        {

        }

        public SwagDataRow(DataRow dataRow)
        {
            _dataRow = dataRow;
            if (_dataRow != null)
            {
                _previousValue = _value;
                _value = this.ToJObject();
            }
        }

        public SwagDataRow(SwagDataRow row) : base()
        {
            PropertyCopy.Copy(row, this);
        }
        #endregion Initialization

        #region Methods
        public JObject ToJObject()
        {
            if (_dataRow == null)
            {
                return new JObject();
            }
            else
            {
                JObject jObject = new JObject();
                foreach (DataColumn dc in _dataRow.Table.Columns)
                {
                    jObject[dc.ColumnName] = JToken.FromObject(_dataRow[dc.ColumnName]);
                }
                return jObject;
            }
        }

        public void RowOnPropertyChangedExtended(object oldValue, object newValue, string propertyName = null)
        {
            base.OnPropertyChangedExtended(oldValue, newValue, propertyName);
        }
        #endregion Methods
    }
    #endregion SwagDataRow

    #region SwagColumnDistinctValues
    public class SwagColumnDistinctValues : ConcurrentObservableDictionary<Object, SwagDataCell>
    {
        #region Private Members
        SwagDataColumn _column;
        #endregion Private Members

        #region Initialization
        public SwagColumnDistinctValues(SwagDataColumn column)
        {
            _column = column;
            _column.SwagDataTable.DataTable.DefaultView.ListChanged += DefaultView_ListChanged;
            Init();
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

    #region SwagDataCell
    public class SwagDataCell : ViewModelBase
    {
        #region Private Members
        Object _value;
        Boolean _isChecked = false;
        Int32 _count = 0;
        SwagDataColumn _column;
        #endregion Private Members

        #region Properties
        #region Column
        public SwagDataColumn Column
        {
            get { return _column; }
            set { SetValue(ref _column, value); }
        }
        #endregion Column
        #region Value
        public Object Value
        {
            get { return _value; }
            set { SetValue(ref _value, value); }
        }
        #endregion Value
        #region IsChecked
        public Boolean IsChecked
        {
            get { return _isChecked; }
            set { SetValue(ref _isChecked, value); }
        }
        #endregion IsChecked
        #region Count
        public Int32 Count
        {
            get { return _count; }
            set
            {
                SetValue(ref _count, value);
                OnPropertyChanged("IsVisible");
            }
        }
        #endregion Count
        #endregion Properties
    }
    #endregion SwagDataCell

    #region SwagDataColumn
    public class SwagDataColumn : ViewModelBaseExtended
    {
        #region Private Members
        Boolean _readOnly = false, _isSelected,
            _isColumnFilterOpen = false, _showAllDistinctValues = false, _listCheckAll = false,
            _isChecked_Visiblity, _isChecked_filter, _ignoreAppliedFilters = false;
        Decimal _total = 0.0m;
        String _columnName, _expression, _dataTemplate, _searchFilter, _listValuesFilter, _appliedFilter;
        Binding _binding = null;
        String _jsonBinding;
        ICommand _applySearchFilterCommand, _applyListFilterCommand, _applyListValuesFilterCommand, _toggleListCheckAllCommand, _clearFilterCommand;
        SwagColumnDistinctValues _distinctValuesSource;
        CollectionViewSource _distinctValues;
        FilterMode _searchFilterMode, _listValuesFilterMode;
        #endregion Private Members

        #region Properties
        #region ReadOnly
        public Boolean ReadOnly
        {
            get
            {
                if (!String.IsNullOrEmpty(Expression))
                {
                    _readOnly = true;
                }
                return _readOnly;
            }
            set { SetValue(ref _readOnly, value); }
        }

        [JsonIgnore]
        public Boolean IsReadOnly
        {
            get
            {
                if (!String.IsNullOrEmpty(Expression))
                {
                    _readOnly = true;
                }
                return _readOnly;
            }
            set { SetValue(ref _readOnly, value); }
        }
        #endregion ReadOnly
        #region IsSelected
        public Boolean IsSelected
        {
            get { return _isSelected; }
            set { SetValue(ref _isSelected, value); }
        }
        #endregion IsSelected
        #region IsColumnFilterOpen
        [JsonIgnore]
        public Boolean IsColumnFilterOpen
        {
            get { return _isColumnFilterOpen; }
            set { SetValue(ref _isColumnFilterOpen, value); }
        }
        #endregion IsColumnFilterOpen   
        #region ListCheckAll
        [JsonIgnore]
        public Boolean ListCheckAll
        {
            get { return _listCheckAll; }
            set
            {
                SetValue(ref _listCheckAll, value);
                ToggleListCheckAllCommand.Execute(_listCheckAll);
            }
        }
        #endregion ListCheckAll
        #region SearchFilter
        [JsonIgnore]
        public String SearchFilter
        {
            get { return _searchFilter; }
            set { SetValue(ref _searchFilter, value); }
        }
        #endregion SearchFilter
        #region SearchFilterMode
        [JsonIgnore]
        public FilterMode SearchFilterMode
        {
            get { return _searchFilterMode; }
            set { SetValue(ref _searchFilterMode, value); }
        }
        #endregion SearchFilterMode
        #region ListValuesFilterMode
        [JsonIgnore]
        public FilterMode ListValuesFilterMode
        {
            get { return _listValuesFilterMode; }
            set { SetValue(ref _listValuesFilterMode, value); }
        }
        #endregion ListValuesFilterMode
        #region ListValuesFilter
        public String ListValuesFilter
        {
            get { return _listValuesFilter; }
            set { SetValue(ref _listValuesFilter, value); }
        }
        #endregion ListValuesFilter
        #region AppliedFilter
        public String AppliedFilter
        {
            get { return _appliedFilter; }
            set
            {
                SetValue(ref _appliedFilter, value);
                OnPropertyChanged("HasAppliedFilter");
            }
        }
        #endregion AppliedFilter
        #region HasAppliedFilter
        public Boolean HasAppliedFilter
        {
            get { return !String.IsNullOrEmpty(_appliedFilter); }
        }
        #endregion HasAppliedFilter
        #region ColumnName
        public String ColumnName
        {
            get { return _columnName; }
            set { SetValue(ref _columnName, value); }
        }

        [JsonIgnore]
        public String Header
        {
            get { return _columnName; }
            set { SetValue(ref _columnName, value); }
        }
        #endregion ColumnName
        #region Expression
        public String Expression
        {
            get { return _expression; }
            set { SetValue(ref _expression, value); }
        }
        #endregion Expression
        #region DataTemplate
        public String DataTemplate
        {
            get { return _dataTemplate; }
            set { SetValue(ref _dataTemplate, value); }
        }
        #endregion DataTemplate
        #region Binding
        public Binding Binding
        {
            get
            {
                if (_binding == null)
                {
                    _binding = new Binding(_columnName);
                }
                return _binding;
            }
            set { SetValue(ref _binding, value); }
        }
        #endregion Binding
        #region DataGridColumn
        [JsonIgnore]
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
        [JsonIgnore]
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
        #region SwagDataTable
        [JsonIgnore]
        public SwagDataTable SwagDataTable { get; set; }
        #endregion SwagDataTable
        #region ApplySearchFilterCommand
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
                        SwagDataTable.FilterCommand.Execute(null);
                        ApplyDistinctValuesFilter();
                    }));
            }
        }
        #endregion ApplySearchFilterCommand
        #region ApplyListValuesFilterCommand
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
                        SwagDataTable.FilterCommand.Execute(null);
                        ApplyDistinctValuesFilter();
                    }));
            }
        }
        #endregion ApplyListFilterCommand
        #region ClearFilterCommand
        public ICommand ClearFilterCommand
        {
            get
            {
                return _clearFilterCommand ?? (_clearFilterCommand =
                    new RelayCommand(() =>
                    {
                        AppliedFilter = "";
                        SwagDataTable.FilterCommand.Execute(null);
                        ApplyDistinctValuesFilter();
                    }));
            }
        }
        #endregion ClearFilterCommand
        #region ToggleListCheckAllCommand
        public ICommand ToggleListCheckAllCommand
        {
            get
            {
                return _toggleListCheckAllCommand ?? (_toggleListCheckAllCommand =
                    new RelayCommand<Boolean>((isChecked) =>
                    {
                        foreach (KeyValuePair<Object, SwagDataCell> kvp in DistinctValuesView)
                        {
                            SwagDataCell cell = kvp.Value;
                            cell.IsChecked = isChecked;
                        }
                    }));
            }
        }
        #endregion ToggleListCheckAllCommand
        #region DistinctValuesView
        [JsonIgnore]
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
        [JsonIgnore]
        public Boolean ShowAllDistinctValues
        {
            get { return _showAllDistinctValues; }
            set
            {
                SetValue(ref _showAllDistinctValues, value);
                ApplyDistinctValuesFilter();
            }
        }
        #endregion ShowAllDistinctValues   
        #endregion Properties

        #region Initialization
        public SwagDataColumn()
        {

        }

        public SwagDataColumn(DataColumn dc)
        {
            PropertyCopy.Copy(dc, this);
        }
        #endregion Initialization

        #region Methods
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
                        return SearchHelper.PassCriteria(kvp.Value.Value.ToString(), _listValuesFilter, false, _listValuesFilterMode, true)
                            && (kvp.Value.Count > 0 || _showAllDistinctValues);
                    }

                };
            }
        }
        #endregion Methods
    }
    #endregion SwagDataColumn

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

    #region SwagDataTable
    public class SwagDataTable : SwagDataGroup
    {
        #region Private Members
        String _name, _message;
        DataTable _dataTable;
        SwagContext _context;
        Dictionary<DataRow, SwagDataRow> _dictChildren = new Dictionary<DataRow, SwagDataRow>();
        ConcurrentObservableOrderedDictionary<String, SwagDataColumn> _columns = new ConcurrentObservableOrderedDictionary<String, SwagDataColumn>();
        ICommand _filterCommand, _exportCommand;
        SwagSettingGroup _settings;
        SwagTabCollection _tabs;
        #endregion Private Members

        #region Properties
        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name
        #region Message
        [JsonIgnore]
        [NotMapped]
        public String Message
        {
            get { return _message; }
            set { SetValue(ref _message, value); }
        }
        #endregion Message
        #region DataTable
        public DataTable DataTable
        {
            get { return _dataTable; }
            set { SetDataTable(value); }
        }
        #endregion DataTable
        #region HasChildren
        public Boolean HasChildren
        {
            get { return _children.Count > 0; }
        }
        #endregion HasChildren
        #region Columns
        public ConcurrentObservableOrderedDictionary<String, SwagDataColumn> Columns
        {
            get { return _columns; }
            set { SetValue(ref _columns, value); }
        }
        #endregion Columns
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
                        switch (Settings["Export"]["Type"].GetValue<SwagTableExportType>())
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
                            case SwagTableExportType.Sqlite_Command:
                                converter = new DataTableSqliteCommandConverter();
                                dialogFilter = "SQLite files (*.sqlite)|*.sqlite";
                                break;
                        }

                        Object output = converter.FromDataTableToObject(new DataTableConvertParameters(), _dataTable);

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
        #region Settings
        public SwagSettingGroup Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new SwagSettingGroup();
                    _settings["Export"] = new SwagSettingGroup() { SettingType = SettingType.SettingGroup, Icon = PackIconCustomKind.Export };
                    _settings["Export"]["Type"] = new SwagSetting<SwagTableExportType>() { SettingType = SettingType.DropDown, Value = SwagTableExportType.Csv, Icon = PackIconCustomKind.ExportType, ItemsSource = (SwagTableExportType[])Enum.GetValues(typeof(SwagTableExportType)) };
                    _settings["Export"]["Destination"] = new SwagSetting<SwagTableDestinationType>() { SettingType = SettingType.DropDown, Value = SwagTableDestinationType.Clipboard, Icon = PackIconCustomKind.Destination, ItemsSource = (SwagTableDestinationType[])Enum.GetValues(typeof(SwagTableDestinationType)) };
                }
                return _settings;
            }
            set
            {
                SetValue(ref _settings, value);
                InitSettings();
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
        #region Listening
        public override Boolean Listening
        {
            get { return _listening; }
            set
            {
                if (_listening != value)
                {
                    SetValue(ref _listening, value);
                    if (_listening && _dataTable != null)
                    {
                        _dataTable.RowChanged += dataTable_RowChanged;
                    }
                    else if (_dataTable != null)
                    {
                        _dataTable.RowChanged -= dataTable_RowChanged;
                    }
                }
            }
        }
        #endregion Listening
        #endregion Properties

        #region Initialization
        public SwagDataTable()
        {
        }

        public SwagDataTable(DataTable dt)
        {
            SetDataTable(dt);
        }

        public void SetDataTable(DataTable dt, Boolean silent = false)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();

            Listening = false;
            SetValue(ref _dataTable, dt, "DataTable");

            if (!silent)
            {
                #region Clear Columns and Rows for the given context
                if (_context != null)
                {
                    SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                    foreach (SwagDataRow row in Children)
                    {
                        work.DataRows.Delete(row);
                    }

                    work.DataTables.Update(this);
                    work.Complete();
                }
                #endregion Clear Columns and Rows for the given context

                #region Clear Columns and Rows for instance
                Columns.Clear();
                Children.Clear();
                #endregion Clear Columns and Rows for instance

                #region Add Columns and Rows for instance
                foreach (DataColumn dc in dt.Columns)
                {
                    SwagDataColumn sdc = new SwagDataColumn() { ColumnName = dc.ColumnName };
                    sdc.SwagDataTable = this;
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
                foreach (SwagDataRow row in Children)
                {
                    _dictChildren.Add(row.DataRow, row);
                }
            }

            Listening = true;
            //_dataTable.RowChanged += dataTable_RowChanged;
            _dataTable.Columns.CollectionChanged += dataTable_Columns_CollectionChanged;
            _columns.CollectionChanged += viewModel_Columns_CollectionChanged;

            //stopwatch.Stop();
            //Message = $"Table Load [{Name}][{stopwatch.Elapsed.ToString("g")}]";
        }

        #endregion Initialization

        #region Column Events
        private void dataTable_Columns_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Listening = false;
            DataColumn dc = (DataColumn)e.Element;
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    if (!Columns.ContainsKey(dc.ColumnName))
                    {
                        SwagDataColumn sdc = new SwagDataColumn(dc);
                        sdc.SwagDataTable = this;
                        Columns.Add(dc.ColumnName, sdc);
                    }
                    break;
                case CollectionChangeAction.Refresh:
                    //Ex. usage of SetOrdinal;
                    Columns.Move(dc.Ordinal, dc.ColumnName);
                    break;
                case CollectionChangeAction.Remove:
                    if (Columns.ContainsKey(dc.ColumnName))
                    {
                        Columns.Remove(dc.ColumnName);
                    }
                    break;
            }
            Listening = true;
        }

        private void viewModel_Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Listening = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (KeyValuePair<String, SwagDataColumn> sdcKvp in e.NewItems)
                    {
                        if (!_dataTable.Columns.Contains(sdcKvp.Key))
                        {
                            _dataTable.Columns.Add(sdcKvp.Value.DataColumn);
                            _dataTable.Columns[sdcKvp.Key].SetOrdinal(e.NewStartingIndex);
                        }
                        sdcKvp.Value.SwagDataTable = this;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (KeyValuePair<String, SwagDataColumn> sdcKvp in e.OldItems)
                    {
                        if (_dataTable.Columns.Contains(sdcKvp.Key))
                        {
                            _dataTable.Columns.Remove(sdcKvp.Key);
                        }
                    }
                    break;
            }
            TrySaveColumns();
            Listening = true;

        }
        #endregion Column Events

        #region RowEvents
        private void dataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            SwagDataRow row = _dictChildren[e.Row];
            row.Value = row.Value;

            if (_context != null)
            {
                SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                Listening = false;
                row.ObjValue = row.ObjValue;
                work.DataRows.Update(row);
                work.Complete();
                Listening = true;
            }
        }
        #endregion RowEvents

        #region Context Methods
        public void SetContext(SwagContext context)
        {
            _context = context;
        }

        public void TrySaveColumns()
        {
            Listening = false;
            if (_context != null)
            {
                SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                this.Columns = Columns;
                foreach (SwagDataRow dataRow in Children)
                {
                    dataRow.Value = dataRow.Value;
                }

                work.DataTables.Update(this);
                work.Complete();
            }
            Listening = true;
        }

        public void Save()
        {
            if (_context != null)
            {
                _context.SaveChanges();
            }
        }
        #endregion Context Methods

        #region Iterator
        public SwagItemPreOrderIterator<SwagDataGroup, SwagData> CreateIterator()
        {
            return new SwagItemPreOrderIterator<SwagDataGroup, SwagData>(this);
        }
        #endregion Iterator

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
                SwagItemPreOrderIterator<SwagTabCollection, SwagTabItem> iterator = _tabs.CreateIterator();
                for (SwagTabItem tabItem = iterator.First(); !iterator.IsDone; tabItem = iterator.Next())
                {
                    tabItem.ViewModel = this;
                }
                _tabs.SwagItemChanged += _tabs_SwagItemChanged;
                _tabs.PropertyChangedExtended += _tabs_PropertyChangedExtended;
                //_tabs.IsInitialized = true;
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
        #endregion Methods
    }
    #endregion SwagDataTable

    #region SwagDataSet
    public class SwagDataSet : SwagDataGroup
    {
        #region Private Members
        SwagSettingGroup _settings;
        SwagTabCollection _tabs;
        ICommand _filterTabsCommand;
        #endregion Private Members

        #region Settings
        public SwagSettingGroup Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new SwagSettingGroup();
                    _settings["Search"] = new SwagSettingGroup() { Icon = PackIconCustomKind.Search };
                    _settings["Search"]["Tabs"] = new SwagSettingGroup() { Icon = PackIconCustomKind.TableSearch };
                    _settings["Search"]["Tabs"]["Text"] = new SwagSettingString() { };
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
                    _tabs["SearchTabs"] = new SwagTabItem() { Icon = PackIconCustomKind.TableSearch, ViewModel = this };
                    _tabs["Settings"] = new SwagTabItem() { Icon = PackIconCustomKind.Settings, ViewModel = this };
                }
                return _tabs;
            }
            set
            {
                SetValue(ref _tabs, value);
                _tabs["SearchTabs"].ViewModel = this;
                _tabs["Settings"].ViewModel = this;
            }
        }
        #endregion Tabs
        #region FilterTabsCommand
        public ICommand FilterTabsCommand
        {
            get
            {
                return _filterTabsCommand ?? (_filterTabsCommand =
                    new RelayCommand(() =>
                    {
                        String filter = _settings["Search"]["Tabs"]["Text"].GetValue<String>();
                        ChildrenView.Filter = (item) =>
                        {
                            SwagData swagData = (SwagData)item;
                            Boolean itemMatch = SearchHelper.PassCriteria(swagData.Display, filter, false, FilterMode.CONTAINS, false);
                            Boolean childDataSetMatch = false;

                            if (swagData is SwagDataSet)
                            {
                                SwagDataSet childDataSet = (SwagDataSet)swagData;
                                childDataSet.Settings["Search"]["Tabs"]["Text"].SetValue(filter);
                                childDataSet.FilterTabsCommand.Execute(null);
                                childDataSetMatch = !childDataSet.ChildrenView.IsEmpty;
                            }

                            return itemMatch || childDataSetMatch;
                        };
                    }));
            }
        }
        #endregion FilterTabsCommand

        public void LoadFiles(IEnumerable<String> files)
        {
            foreach (String file in files)
            {
                Children.Add(SwagDataHelper.FromFile(file));
            }
        }
    }
    #endregion SwagDataSet

    #region SwagDataHelper
    public static class SwagDataHelper
    {
        public static SwagData FromFile(String file, Dictionary<string, string> extensionMappings = null)
        {
            String filename = Path.GetFileName(file);
            String ext = Path.GetExtension(file);
            var dataTableContext = DataTableConverterHelper.ConverterContexts[ext];
            if (dataTableContext != null)
            {
                return new SwagDataTable(dataTableContext.ToDataTable(file)) { Display = filename };
            }

            return null;
        }
    }
    #endregion SwagDataHelper

}
