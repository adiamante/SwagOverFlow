using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using SwagOverFlow.Iterator;
using SwagOverFlow.Collections;
using System.ComponentModel;
using System.Windows.Input;
using SwagOverFlow.Commands;
using System.Collections;

namespace SwagOverFlow.ViewModels
{
    #region SwagData
    public class SwagData : SwagValueItem<SwagDataGroup, SwagData, JObject>
    {
        #region Private/Protected Members
        protected SwagDataResult _swagDataResult;
        #endregion Private/Protected Members

        #region Properties
        #region SwagDataResult
        [NotMapped]
        [JsonIgnore]
        public SwagDataResult SwagDataResult
        {
            get { return _swagDataResult; }
            set { SetValue(ref _swagDataResult, value); }
        }
        #endregion SwagDataResult
        #endregion Properties

        #region Methods
        public virtual SwagDataResult Search(String searchValue, FilterMode filterMode, Func<SwagDataColumn, SwagDataRow, String, FilterMode, bool> searchFunc)
        {
            return null;
        }

        public virtual DataSet GetDataSet()
        {
            return null;
        }
        #endregion Methods
    }
    #endregion SwagData

    #region SwagDataGroup
    public class SwagDataGroup : SwagData, ISwagParent<SwagData>
    {
        #region Private/Protected Members
        protected ObservableCollection<SwagData> _children = new ObservableCollection<SwagData>();
        #endregion Private/Protected Members

        #region Events
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public virtual void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedExtendedEventArgs e)
        {
            SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs() { SwagItem = swagItem, PropertyChangedArgs = e, Message = e.Message });
            Parent?.OnSwagItemChanged(swagItem, e);
        }
        #endregion Events

        #region Properties
        #region Children
        public ObservableCollection<SwagData> Children
        {
            get { return _children; }
            set { SetValue(ref _children, value); }
        }
        #endregion Children
        #region HasChildren
        [NotMapped]
        public Boolean HasChildren
        {
            get { return _children.Count > 0; }
        }
        #endregion HasChildren
        #endregion Properties

        #region Initialization
        public SwagDataGroup()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagData newItem in e.NewItems)
                {
                    newItem.CanUndo = false;
                    newItem.Parent = this;
                    if (newItem.Sequence < 0)
                    {
                        newItem.Sequence = this.Children.Count - 1;
                    }
                    newItem.CanUndo = true;
                }
            }
        }
        #endregion Initialization

        #region Iterator
        public SwagItemPreOrderIterator<SwagData> CreateIterator()
        {
            return new SwagItemPreOrderIterator<SwagData>(this);
        }
        #endregion Iterator
    }
    #endregion SwagDataGroup

    #region SwagDataRow
    public class SwagDataRow : SwagData
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
        #region ObjValue
        public override object ObjValue
        {
            get
            {
                var _currentValue = Value;
                if (_currentValue == null)
                {
                    return _objValue;
                }
                return _currentValue;
            }
            set
            {
                _value = (JObject)value;
                _objValue = value;
                OnPropertyChangedExtended(_previousValue, _value, "Value");
            }
        }
        #endregion ObjValue
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
                JObject dif = JsonHelper.FindDiff(_value, value);
                SetValue(ref _previousValue, value, false);
                _objValue = _value;
                
                if (dif.Count > 0 && _dataRow != null)
                {
                    foreach (KeyValuePair<String, JToken> kvp in dif)
                    {
                        String colName = kvp.Key;
                        _dataRow[colName] = ((JValue)value[colName]).Value;
                    }
                }
            }
        }
        #endregion Value
        #region PreviousValue
        [NotMapped]
        [JsonIgnore]
        public JObject PreviousValue
        {
            get { return _previousValue; }
            //set { SetValue(ref _previousValue, value); }
        }
        #endregion PreviousValue
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

    #region SwagDataColumn
    public class SwagDataColumn : SwagData
    {
        #region Private/Protected Members
        protected Boolean _readOnly = false, _isSelected, _isVisible = true,
            _isColumnFilterOpen = false, _showAllDistinctValues = false, _listCheckAll = false,
            _isCheckedVisiblity, _isCheckedFilter;
        protected Decimal _total = 0.0m;
        protected Int32 _colSeq = -1;
        protected String _expression, _dataTemplate, _searchFilter, _listValuesFilter, _appliedFilter;
        protected FilterMode _searchFilterMode, _listValuesFilterMode;
        protected Type _dataType;
        protected String _dataTypeString;
        SwagColumnDistinctValues _distinctValues;
        ICommand _applySearchFilterCommand, _applyListFilterCommand, _applyListValuesFilterCommand, _toggleListCheckAllCommand, _clearFilterCommand,
            _hideCommand, _removeCommand;
        #endregion Private/Protected Members

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
        #region IsCheckedVisibility
        [NotMapped]
        public Boolean IsCheckedVisibility
        {
            get { return _isCheckedVisiblity; }
            set { SetValue(ref _isCheckedVisiblity, value); }
        }
        #endregion IsCheckedVisibility
        #region IsCheckedFilter
        [NotMapped]
        public Boolean IsCheckedFilter
        {
            get { return _isCheckedFilter; }
            set { SetValue(ref _isCheckedFilter, value); }
        }
        #endregion IsCheckedFilter
        #region IsVisible
        public Boolean IsVisible
        {
            get { return _isVisible; }
            set { SetValue(ref _isVisible, value); }
        }
        #endregion IsVisible
        #region IsColumnFilterOpen
        [NotMapped]
        public Boolean IsColumnFilterOpen
        {
            get { return _isColumnFilterOpen; }
            set { SetValue(ref _isColumnFilterOpen, value); }
        }
        #endregion IsColumnFilterOpen   
        #region ListCheckAll
        [NotMapped]
        public virtual Boolean ListCheckAll
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
        [NotMapped]
        public String SearchFilter
        {
            get { return _searchFilter; }
            set { SetValue(ref _searchFilter, value); }
        }
        #endregion SearchFilter
        #region SearchFilterMode
        [NotMapped]
        public FilterMode SearchFilterMode
        {
            get { return _searchFilterMode; }
            set { SetValue(ref _searchFilterMode, value); }
        }
        #endregion SearchFilterMode
        #region ListValuesFilterMode
        [NotMapped]
        public FilterMode ListValuesFilterMode
        {
            get { return _listValuesFilterMode; }
            set { SetValue(ref _listValuesFilterMode, value); }
        }
        #endregion ListValuesFilterMode
        #region ListValuesFilter
        [NotMapped]
        public String ListValuesFilter
        {
            get { return _listValuesFilter; }
            set { SetValue(ref _listValuesFilter, value); }
        }
        #endregion ListValuesFilter
        #region AppliedFilter
        public virtual String AppliedFilter
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
        [NotMapped]
        public Boolean HasAppliedFilter
        {
            get { return !String.IsNullOrEmpty(_appliedFilter); }
        }
        #endregion HasAppliedFilter
        #region ColumnName
        public String ColumnName
        {
            get { return _display; }
            set { SetValue(ref _display, value); }
        }

        [JsonIgnore]
        public String Header
        {
            get { return _display; }
            set { SetValue(ref _display, value); }
        }
        #endregion ColumnName
        #region Expression
        public String Expression
        {
            get { return _expression; }
            set { SetValue(ref _expression, value); }
        }
        #endregion Expression
        #region SwagDataTable
        [JsonIgnore]
        [NotMapped]
        public SwagDataTable SwagDataTable
        {
            get { return (SwagDataTable)_parent; }
            set
            {
                SetValue(ref _parent, value);
            }
        }
        #endregion SwagDataTable
        #region DataType
        [NotMapped]
        public virtual Type DataType
        {
            get
            {
                if (_dataType == null && !String.IsNullOrEmpty(_dataTypeString))
                {
                    _dataType = JsonConvert.DeserializeObject<Type>(_dataTypeString);
                }
                return _dataType;
            }
            set { SetValue(ref _dataType, value); }
        }
        #endregion DataType
        #region DataTypeString
        public virtual String DataTypeString
        {
            get 
            {
                if (DataType != null)
                {
                    _dataTypeString = JsonHelper.ToJsonString(DataType);
                }
                return _dataTypeString; 
            }
            set { SetValue(ref _dataTypeString, value); }
        }
        #endregion DataTypeString
        #region Total
        [NotMapped]
        public Decimal Total
        {
            get { return _total; }
            set { SetValue(ref _total, value); }
        }
        #endregion Total
        #region ShowAllDistinctValues
        [NotMapped]
        public virtual Boolean ShowAllDistinctValues
        {
            get { return _showAllDistinctValues; }
            set 
            { 
                SetValue(ref _showAllDistinctValues, value);
                OnApplyDistinctValuesFilter();
            }
        }
        #endregion ShowAllDistinctValues   
        #region ColSeq
        public Int32 ColSeq
        {
            get { return _colSeq; }
            set { SetValue(ref _colSeq, value); }
        }
        #endregion ColSeq
        #region DataTemplate
        public String DataTemplate
        {
            get { return _dataTemplate; }
            set { SetValue(ref _dataTemplate, value); }
        }
        #endregion DataTemplate
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
                        OnApplyDistinctValuesFilter();
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
                        OnApplyDistinctValuesFilter();
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

                        foreach (KeyValuePair<Object, SwagDataCell> kvp in _distinctValues)
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
                        OnApplyDistinctValuesFilter();
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
                        OnApplyDistinctValuesFilter();
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
                            foreach (KeyValuePair<Object, SwagDataCell> kvp in DistinctValues)
                            {
                                SwagDataCell cell = kvp.Value;
                                cell.IsChecked = isChecked;
                            }
                        }
                    }));
            }
        }
        #endregion ToggleListCheckAllCommand  
        #region DistinctValues
        [NotMapped]
        public SwagColumnDistinctValues DistinctValues
        {
            get
            {
                if (_distinctValues == null)
                {
                    _distinctValues = new SwagColumnDistinctValues(this);
                    OnApplyDistinctValuesFilter();
                }

                return _distinctValues;
            }
        }
        #endregion DistinctValues
        #region HasApplyDistinctValuesFilterHandler
        public Boolean HasApplyDistinctValuesFilterHandler
        {
            get
            {
                EventHandler<EventArgs> handler = ApplyDistinctValuesFilter;
                return handler != null;
            }
        }
        #endregion HasApplyDistinctValuesFilterHandler
        #endregion Properties

        #region Events
        public event EventHandler<EventArgs> ApplyDistinctValuesFilter;
        #endregion Events

        #region Initialization
        public SwagDataColumn()
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
        #endregion Initialization

        #region Methods
        public override SwagDataResult Search(String searchValue, FilterMode filterMode, Func<SwagDataColumn, SwagDataRow, String, FilterMode, bool> searchFunc)
        {
            List<SwagDataRowResult> lstCellResults = new List<SwagDataRowResult>();
            foreach (SwagData swagData in SwagDataTable.Children)
            {
                if (swagData is SwagDataRow swagDataRow)
                {
                    if (searchFunc(this, swagDataRow, searchValue, filterMode))
                    {
                        String resultValue = swagDataRow.DataRow[ColumnName].ToString(); ;
                        SwagDataRowResult cellResult = new SwagDataRowResult() { SwagData = swagDataRow, Display = resultValue };
                        lstCellResults.Add(cellResult);
                    }
                }
            }

            if (lstCellResults.Count > 0)
            {
                SwagDataColumnResultGroup columnResultGroup = new SwagDataColumnResultGroup() { SwagData = this, Display = ColumnName };
                columnResultGroup.Children = new ObservableCollection<SwagDataResult>(lstCellResults);
                return columnResultGroup;
            }

            return null;
        }

        public void SetSequence(Int32 ordinal)
        {
            SwagDataTable sdt = SwagDataTable;
            sdt.DataTable.Columns[ColumnName].SetOrdinal(ordinal);
            foreach (DataColumn dc in SwagDataTable.DataTable.Columns)
            {
                sdt.Columns[dc.ColumnName].ColSeq = dc.Ordinal;
            }
            sdt.ResetColumnsCommand.Execute(null);
            sdt.InvalidateColumns();
        }

        public void Rename(String newColName)
        {
            SwagDataTable sdt = SwagDataTable;
            Int32 originalColSeq = ColSeq;
            //Update DataTable to have the new name
            sdt.DataTable.Columns[ColumnName].ColumnName = newColName;
            sdt.Columns.Remove(ColumnName);
            ColumnName = newColName;
            sdt.Columns.Add(ColumnName, this);
            SetSequence(originalColSeq);        //When a column is added with 0 or lower sequence, it gets the value SwagDataTable.Columns.Count - 1
            sdt.DelaySave = true;
            sdt.InvalidateColumns();
            sdt.InvalidateRows();
            sdt.DelaySave = false;
        }

        public void Remove()
        {
            SwagDataTable sdt = SwagDataTable;
            sdt.Columns.Remove(ColumnName);
            foreach (DataColumn dc in sdt.DataTable.Columns)
            {
                sdt.Columns[dc.ColumnName].ColSeq = dc.Ordinal;
            }
            sdt.InvalidateColumns();
        }

        public void OnApplyDistinctValuesFilter()
        {
            EventHandler<EventArgs> handler = ApplyDistinctValuesFilter;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        public DataColumn DataColumn()
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
        #endregion Methods
    }
    #endregion SwagDataColumn

    #region SwagDataTable
    public class SwagDataTable : SwagDataGroup
    {
        #region Private/Protected Members
        String _name, _message;
        DataTable _dataTable;
        Boolean _isInitialized = false, _showDebug = false, _showColumnTotals = false, _columnVisibilityCheckAll = false, _columnFiltersCheckAll = false;
        IDictionary<String, SwagDataColumn> _columns;
        Dictionary<DataRow, SwagDataRow> _dictRows = new Dictionary<DataRow, SwagDataRow>();
        ICommand _filterCommand, _exportCommand, _importCommand, _applyColumnVisibilityCommand, _toggleColumnVisibilityCheckedAll, _applyColumnVisibilityFilterCommand,
            _toggleShowDebugCommand, _toggleShowColumnTotalsCommand, _resetColumnsCommand,
            _toggleColumnFiltersCheckedAll, _applyColumnFiltersFilterCommand, _clearColumnFiltersCommand;
        SwagSettingGroup _settings = new SwagSettingGroup();
        SwagTabGroup _tabs = new SwagTabGroup();
        SwagDataColumn _selectedColumn;
        SwagDataRowResult _selectedRow;
        INotifyCollectionChanged _columnsView, _columnsVisibilityView, _columnsFilterView;
        #endregion Private/Protected Members

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
        #region IsInitialized
        public Boolean IsInitialized
        {
            get { return _isInitialized; }
            set { SetValue(ref _isInitialized, value); }
        }
        #endregion IsInitialized
        #region DataTable
        [JsonIgnore]
        [NotMapped]
        public DataTable DataTable
        {
            get { return _dataTable; }
            set { SetValue(ref _dataTable, value); }
        }
        #endregion DataTable
        #region Columns
        [JsonIgnore]
        [NotMapped]
        public IDictionary<String, SwagDataColumn> Columns
        {
            get { return _columns; }
            set { SetValue(ref _columns, value); }
        }
        #endregion Columns
        #region RowCount
        [JsonIgnore]
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
        [JsonIgnore]
        [NotMapped]
        public Int32 ColumnCount
        {
            get { return Columns.Count; }
        }
        #endregion ColumnCount
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
        [JsonIgnore]
        [NotMapped]
        public ICommand FilterCommand
        {
            get { return _filterCommand; }
            set { SetValue(ref _filterCommand, value); }
        }
        #endregion FilterCommand
        #region ExportCommand
        [JsonIgnore]
        [NotMapped]
        public ICommand ExportCommand
        {
            get { return _exportCommand; }
            set { SetValue(ref _exportCommand, value); }
        }
        #endregion ExportCommand
        #region ImportCommand
        [JsonIgnore]
        [NotMapped]
        public ICommand ImportCommand
        {
            get { return _importCommand; }
            set { SetValue(ref _importCommand, value); }
        }
        #endregion ImportCommand
        #region ApplyColumnVisibilityFilterCommand
        [JsonIgnore]
        [NotMapped]
        public ICommand ApplyColumnVisibilityFilterCommand
        {
            get { return _applyColumnVisibilityFilterCommand; }
            set { SetValue(ref _applyColumnVisibilityFilterCommand, value); }
        }
        #endregion ApplyColumnVisibilityFilterCommand
        #region ToggleColumnVisibilityCheckedAll
        [JsonIgnore]
        [NotMapped]
        public ICommand ToggleColumnVisibilityCheckedAll
        {
            get
            {
                return _toggleColumnVisibilityCheckedAll ?? (_toggleColumnVisibilityCheckedAll =
                    new RelayCommand<Boolean>((checkAll) =>
                    {
                        if (ColumnsVisibilityView is IEnumerable columnsVisibilityView)
                        {
                            foreach (KeyValuePair<String, SwagDataColumn> kvp in columnsVisibilityView)
                            {
                                SwagDataColumn col = kvp.Value;
                                col.IsCheckedVisibility = checkAll;
                            }
                        }
                    }));
            }
        }
        #endregion ToggleColumnVisibilityCheckedAll
        #region ApplyColumnVisibilityCommand
        [JsonIgnore]
        [NotMapped]
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
                        ResetColumnsCommand.Execute(null);
                    }));
            }
        }
        #endregion ApplyColumnVisibilityCommand
        #region ApplyColumnFiltersFilterCommand
        [JsonIgnore]
        [NotMapped]
        public ICommand ApplyColumnFiltersFilterCommand
        {
            get { return _applyColumnFiltersFilterCommand; }
            set { SetValue(ref _applyColumnFiltersFilterCommand, value); }
        }
        #endregion ApplyColumnFiltersFilterCommand
        #region ClearColumnFiltersCommand
        [JsonIgnore]
        [NotMapped]
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
        [JsonIgnore]
        [NotMapped]
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
        [JsonIgnore]
        [NotMapped]
        public ICommand ResetColumnsCommand
        {
            get { return _resetColumnsCommand; }
            set { SetValue(ref _resetColumnsCommand, value); }
        }
        #endregion ResetColumnsCommand
        #region ToggleShowDebugCommand
        [JsonIgnore]
        [NotMapped]
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
        [JsonIgnore]
        [NotMapped]
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
        [JsonIgnore]
        [NotMapped]
        public SwagSettingGroup Settings
        {
            get { return _settings; }
            set { SetValue(ref _settings, value); }
        }
        #endregion Settings
        #region Tabs
        [NotMapped]
        [JsonIgnore]
        public SwagTabGroup Tabs
        {
            get { return _tabs; }
            set { SetValue(ref _tabs, value); }
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
        #region DictRows
        [NotMapped]
        [JsonIgnore]
        public Dictionary<DataRow, SwagDataRow> DictRows
        {
            get { return _dictRows; }
            set { SetValue(ref _dictRows, value); }
        }
        #endregion DictRows
        #region ColumnsView
        [NotMapped]
        [JsonIgnore]
        public INotifyCollectionChanged ColumnsView
        {
            get { return _columnsView; }
            set { SetValue(ref _columnsView, value); }
        }
        #endregion ColumnsVisibilityView
        #region ColumnsVisibilityView
        [NotMapped]
        [JsonIgnore]
        public INotifyCollectionChanged ColumnsVisibilityView
        {
            get { return _columnsVisibilityView; }
            set { SetValue(ref _columnsVisibilityView, value); }
        }
        #endregion ColumnsVisibilityView
        #region ColumnsFilterView
        [NotMapped]
        [JsonIgnore]
        public INotifyCollectionChanged ColumnsFilterView
        {
            get { return _columnsFilterView; }
            set { SetValue(ref _columnsFilterView, value); }
        }
        #endregion ColumnsFilterView
        #endregion Properties

        #region Initialization
        public SwagDataTable()
        {
            _columns = new SwagObservableOrderedDictionary<String, SwagDataColumn>();
            _children.CollectionChanged += _children_CollectionChanged;
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

        public SwagDataTable(DataTable dt) : this()
        {
            DataTable = dt;
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
                            ResetColumnsCommand.Execute(null);
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

        public void InitColumns()
        {
            ((SwagObservableOrderedDictionary<String, SwagDataColumn>)_columns).CollectionChanged += viewModel_Columns_CollectionChanged;
        }

        public void InitDataTable()
        {
            _dataTable.DefaultView.ListChanged += dataView_ListChanged;
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
                            _dataTable.Columns.Add(sdcKvp.Value.DataColumn());
                            _dataTable.Columns[sdcKvp.Key].SetOrdinal(e.NewStartingIndex);
                            this.Children.Add(sdcKvp.Value);
                        }

                        if (sdcKvp.Value.ColSeq < 0)
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
            //FIX_THIS
            //if (!DelaySave)
            //{
            //    SwagDataRow row = _dictChildren[e.Row];
            //    row.ObjValue = row.ObjValue;

            //    if (_context != null)
            //    {
            //        SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
            //        work.Data.Update(row);
            //        work.Complete();
            //    }
            //}
        }

        public void dataView_ListChanged(object sender, ListChangedEventArgs e)
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
        //public void SetContext(SwagContext context)
        //{
        //    _context = context;
        //}

        //public void Save()
        //{
        //    if (!DelaySave && _context != null)
        //    {
        //        SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
        //        work.DataTables.Update(this);
        //        work.Complete();
        //    }
        //}

        //public void InvalidateColumns()
        //{
        //    DelaySave = true;
        //    foreach (SwagData swagData in Children)
        //    {
        //        switch (swagData)
        //        {
        //            case SwagDataColumn swagDataColumn:
        //                swagDataColumn.CanUndo = false;
        //                swagDataColumn.DataTypeString = swagDataColumn.DataTypeString;
        //                swagDataColumn.Value = swagDataColumn.Value;
        //                swagDataColumn.CanUndo = true;
        //                break;
        //        }
        //    }
        //    DelaySave = false;
        //}

        //public void InvalidateRows()
        //{
        //    DelaySave = true;
        //    foreach (SwagData swagData in Children)
        //    {
        //        switch (swagData)
        //        {
        //            case SwagDataRow swagDataRow:
        //                swagDataRow.CanUndo = false;
        //                swagDataRow.Value = swagDataRow.Value;
        //                swagDataRow.CanUndo = true;
        //                break;
        //        }
        //    }
        //    DelaySave = false;
        //}
        #endregion Context Methods

        #region Methods

        public override SwagDataResult Search(String searchValue, FilterMode filterMode, Func<SwagDataColumn, SwagDataRow, String, FilterMode, bool> searchFunc)
        {
            List<SwagDataColumnResultGroup> lstColumnResults = new List<SwagDataColumnResultGroup>();

            foreach (KeyValuePair<String, SwagDataColumn> kvpCol in Columns)
            {
                SwagDataColumnResultGroup columnResults = kvpCol.Value.Search(searchValue, filterMode, searchFunc) as SwagDataColumnResultGroup;
                if (columnResults != null)
                {
                    lstColumnResults.Add(columnResults);
                }
            }

            if (lstColumnResults.Count > 0)
            {
                SwagDataTableResultGroup swagDataTableResultGroup = new SwagDataTableResultGroup() { SwagData = this, Display = this.Display };
                swagDataTableResultGroup.Children = new ObservableCollection<SwagDataResult>(lstColumnResults);
                return swagDataTableResultGroup;
            }

            return null;
        }

        public override DataSet GetDataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = _dataTable.Copy();
            ds.Tables.Add(dt);
            return ds;
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

        public void OnPropertyChangedPublic(String propertyName)
        {
            OnPropertyChanged(propertyName);
        }
        #endregion Methods

        #region Scopes
        public class FreezeList : IDisposable
        {
            SwagDataTable _swagDataTable;
            public FreezeList(SwagDataTable swagDataTable)
            {
                swagDataTable.DataTable.DefaultView.ListChanged -= swagDataTable.dataView_ListChanged;
                _swagDataTable = swagDataTable;
            }

            ~FreezeList() => Dispose();

            public void Dispose()
            {
                _swagDataTable.DataTable.DefaultView.ListChanged += _swagDataTable.dataView_ListChanged;
                _swagDataTable.dataView_ListChanged(null, new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }
        #endregion Scopes
    }
    #endregion SwagDataTable

    #region SwagDataSet
    public class SwagDataSet : SwagDataGroup
    {
        #region Private Members
        SwagSettingGroup _settings = new SwagSettingGroup();
        SwagTabGroup _tabs = new SwagTabGroup();
        ICommand _filterTabsCommand, _addDataSetCommand, _addDataTableCommand;
        SwagData _selectedChild;
        #endregion Private Members

        #region Properties
        #region Settings
        [JsonIgnore]
        [NotMapped]
        public SwagSettingGroup Settings
        {
            get { return _settings; }
            set { SetValue(ref _settings, value); }
        }
        #endregion Settings
        #region Tabs
        [NotMapped]
        [JsonIgnore]
        public SwagTabGroup Tabs
        {
            get { return _tabs; }
            set { SetValue(ref _tabs, value); }
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
        #region FilterTabsCommand
        [NotMapped]
        [JsonIgnore]
        public ICommand FilterTabsCommand
        {
            get { return _filterTabsCommand; }
            set { SetValue(ref _filterTabsCommand, value); }
        }
        #endregion FilterTabsCommand
        #region AddDataSetCommand
        [NotMapped]
        [JsonIgnore]
        public ICommand AddDataSetCommand
        {
            get
            {
                return _addDataSetCommand ?? (_addDataSetCommand =
                    new RelayCommand<DataSet>((ds) =>
                    {
                        SwagDataSet newDataSet = new SwagDataSet(ds);
                        newDataSet.Display = $"Set {this.Children.Count + 1}";
                        Children.Add(newDataSet);
                    }));
            }
        }
        #endregion AddDataSetCommand
        #region AddDataTableCommand
        [NotMapped]
        [JsonIgnore]
        public ICommand AddDataTableCommand
        {
            get
            {
                return _addDataTableCommand ?? (_addDataTableCommand =
                    new RelayCommand<DataTable>((dtbl) =>
                    {
                        String defaultTableName = $"Table {this.Children.Count + 1}";
                        if (dtbl == null)
                        {
                            dtbl = new DataTable(defaultTableName);
                        }
                        else if (dtbl.TableName == "")
                        {
                            dtbl.TableName = defaultTableName;
                        }
                        SwagDataTable newDataTable = new SwagDataTable(dtbl);
                        newDataTable.Display = dtbl.TableName;
                        Children.Add(newDataTable);
                        SelectedChild = newDataTable;
                    }));
            }
        }
        #endregion AddDataTableCommand
        #endregion Properties

        #region Initialization
        public SwagDataSet()
        {
            
        }

        public SwagDataSet(DataSet dataSet) : this()
        {
            if (dataSet != null)
            {
                foreach (DataTable dt in dataSet.Tables)
                {
                    Children.Add(new SwagDataTable(dt) { Display = dt.TableName });
                }
            }
        }
        #endregion Initialization

        #region Methods
        public override DataSet GetDataSet()
        {
            DataSet ds = new DataSet(Display);
            foreach (SwagData swagData in Children)
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

        public override SwagDataResult Search(String searchValue, FilterMode filterMode, Func<SwagDataColumn, SwagDataRow, String, FilterMode, bool> searchFunc)
        {
            List<SwagDataResult> lstSwagDataResults = new List<SwagDataResult>();

            foreach (SwagData swagData in Children)
            {
                SwagDataResult swagDataResult = swagData.Search(searchValue, filterMode, searchFunc);
                if (swagDataResult != null)
                {
                    lstSwagDataResults.Add(swagDataResult);
                }
            }

            if (lstSwagDataResults.Count > 0)
            {
                SwagDataSetResultGroup swagDataSetResultGroup = new SwagDataSetResultGroup() { SwagData = this, Display = this.Display };
                swagDataSetResultGroup.Children = new ObservableCollection<SwagDataResult>(lstSwagDataResults);
                return swagDataSetResultGroup;
            }

            return null;
        }
        #endregion Methods
    }
    #endregion SwagDataSet

    #region SwagDataResult
    public class SwagDataResult : SwagItem<SwagDataResultGroup, SwagDataResult>
    {
        public SwagData SwagData { get; set; }
    }
    #endregion SwagDataResult

    #region SwagDataResultGroup
    public class SwagDataResultGroup : SwagDataResult, ISwagParent<SwagDataResult>
    {
        #region Private/Protected Members
        protected ObservableCollection<SwagDataResult> _children = new ObservableCollection<SwagDataResult>();
        #endregion Private/Protected Members

        #region Events
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;
        public void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedExtendedEventArgs e)
        {
            //Nothing to do here
        }
        #endregion Events

        #region Properties
        #region ChildrenView
        //Might consider moving this out
        public ObservableCollection<SwagDataResult> ChildrenView
        {
            get { return _children; }
            set { SetValue(ref _children, value); }
        }
        #endregion ChildrenView
        #region Children
        public ObservableCollection<SwagDataResult> Children
        {
            get { return _children; }
            set
            {
                SetValue(ref _children, value);
                _children.CollectionChanged += _children_CollectionChanged;
                foreach (SwagDataResult swagDataResult in _children)
                {
                    swagDataResult.Parent = this;
                }
                OnPropertyChanged("ChildrenView");
            }
        }
        #endregion Children
        #endregion Properties

        #region Initialization
        public SwagDataResultGroup()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagDataResult newItem in e.NewItems)
                {
                    newItem.Parent = this;
                    if (newItem.Sequence < 0)
                    {
                        newItem.Sequence = this.Children.Count - 1;
                    }
                }
            }
        }
        #endregion Initialization
    }
    #endregion SwagDataResultGroup

    #region SwagDataRowResult
    public class SwagDataRowResult : SwagDataResult
    {
    }
    #endregion SwagDataRowResult

    #region SwagDataColumnResultGroup
    public class SwagDataColumnResultGroup : SwagDataResultGroup
    {
    }
    #endregion SwagDataColumnResultGroup

    #region SwagDataTableResultGroup
    public class SwagDataTableResultGroup : SwagDataResultGroup
    {
    }
    #endregion SwagDataTableResultGroup

    #region SwagDataSetResultGroup
    public class SwagDataSetResultGroup : SwagDataResultGroup
    {
    }
    #endregion SwagDataSetResultGroup

}
