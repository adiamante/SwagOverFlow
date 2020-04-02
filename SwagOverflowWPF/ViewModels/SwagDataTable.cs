using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverflowWPF.Collections;
using SwagOverflowWPF.Commands;
using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Iterator;
using SwagOverflowWPF.Repository;
using SwagOverflowWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;

namespace SwagOverflowWPF.ViewModels
{
    #region SwagDataRow
    public class SwagDataRow : SwagItem<JObject>
    {
        DataRow _dataRow;

        #region DataRow
        [NotMapped]
        public DataRow DataRow
        {
            get { return _dataRow; }
            set
            {
                if (_dataRow != null)
                {
                    _value = this.ToJObject();
                }
                SetValue(ref _dataRow, value);
            }
        }
        #endregion DataRow

        #region Value
        public override object Value
        {
            get
            {
                if (_dataRow != null)
                {
                    _value = this.ToJObject();
                }
                return _value;
            }
            set { SetValue(ref _value, value); }
        }
        #endregion Value

        public SwagDataRow()
        {

        }

        public SwagDataRow(DataRow dataRow)
        {
            _dataRow = dataRow;
            if (_dataRow != null)
            {
                _value = this.ToJObject();
            }
        }

        public SwagDataRow(SwagDataRow row) : base()
        {
            PropertyCopy.Copy(row, this);
        }

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
    }
    #endregion SwagDataRow

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
        public FilterMode SearchFilterMode
        {
            get { return _searchFilterMode; }
            set { SetValue(ref _searchFilterMode, value); }
        }
        #endregion SearchFilterMode
        #region ListValuesFilterMode
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

    public class SwagDataTable  : SwagGroup<SwagDataRow>
    {
        #region Private Members
        DataTable _dataTable;
        SwagContext _context;
        Dictionary<DataRow, SwagDataRow> _dictChildren = new Dictionary<DataRow, SwagDataRow>();
        ConcurrentObservableOrderedDictionary<String, SwagDataColumn> _columns = new ConcurrentObservableOrderedDictionary<String, SwagDataColumn>();
        ICommand _filterCommand;
        #endregion Private Members

        #region Properties
        #region DataTable
        public DataTable DataTable
        {
            get { return _dataTable; }
            set { SetDataTable(value); }
        }
        #endregion DataTable

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
            SetValue(ref _dataTable, dt, "DataTable");

            if (!silent)
            {
                #region Clear Columns and Rows for the given context
                if (_context != null)
                {
                    SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                    foreach (SwagDataRow row in RootGeneric.Children)
                    {
                        work.DataRows.Delete(row);
                    }

                    work.DataTables.Update(this);
                    work.Complete();
                }
                #endregion Clear Columns and Rows for the given context

                #region Clear Columns and Rows for instance
                Columns.Clear();
                RootGeneric.Children.Clear();
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
                    RootGeneric.Children.Add(row);
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
                foreach (SwagDataRow row in RootGeneric.Children)
                {
                    _dictChildren.Add(row.DataRow, row);
                }
            }

            _dataTable.RowChanged += dataTable_RowChanged;
            _dataTable.Columns.CollectionChanged += dataTable_Columns_CollectionChanged;
            _columns.CollectionChanged += viewModel_Columns_CollectionChanged;
        }
        #endregion Initialization

        #region Column Events
        private void dataTable_Columns_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
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
        }

        private void viewModel_Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
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
        }
        #endregion Column Events

        #region RowEvents
        private void dataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (_context != null)
            {
                SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                SwagDataRow row = _dictChildren[e.Row];
                row.Value = row.Value;
                work.DataRows.Update(row);
                work.Complete();
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
            if (_context != null)
            {
                SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                this.Columns = Columns;
                foreach (SwagDataRow dataRow in this.RootGeneric.Children)
                {
                    dataRow.Value = dataRow.Value;
                }

                work.DataTables.Update(this);
                work.Complete();
            }
        }

        public void Save()
        {
            if (_context != null)
            {
                _context.SaveChanges();
            }
        }
        #endregion Context Methods
    }
}
