using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.ViewModels;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using SwagOverFlow.Iterator;

namespace SwagOverFlow.ViewModels
{
    #region SwagData
    public class SwagData : SwagItem<SwagDataGroup, SwagData, JObject>
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

    #region SwagDataColumn
    public class SwagDataColumn : SwagData
    {
        #region Private/Protected Members
        protected Boolean _readOnly = false, _isSelected, _isVisible = true,
            _isColumnFilterOpen = false, _showAllDistinctValues = false, _listCheckAll = false,
            _isCheckedVisiblity, _isCheckedFilter;
        protected Decimal _total = 0.0m;
        protected Int32 _colSeq;
        protected String _expression, _dataTemplate, _searchFilter, _listValuesFilter, _appliedFilter;
        protected FilterMode _searchFilterMode, _listValuesFilterMode;
        protected Type _dataType;
        protected String _dataTypeString;
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
            set { SetValue(ref _listCheckAll, value); }
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
            set { SetValue(ref _showAllDistinctValues, value); }
        }
        #endregion ShowAllDistinctValues   
        #region ColSeq
        public Int32 ColSeq
        {
            get { return _colSeq; }
            set { SetValue(ref _colSeq, value); }
        }
        #endregion ColSeq
        #endregion Properties

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
        #endregion Methods
    }
    #endregion SwagDataColumn

    #region SwagDataTable
    public class SwagDataTable : SwagDataGroup
    {
        #region Private/Protected Members
        protected String _name, _message;
        protected DataTable _dataTable;
        protected Boolean _showDebug = false, _showColumnTotals = false, _columnVisibilityCheckAll = false, _columnFiltersCheckAll = false;
        protected IDictionary<String, SwagDataColumn> _columns;
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
        #region DataTable
        public virtual DataTable DataTable
        {
            get { return _dataTable; }
            set { SetValue(ref _dataTable, value); }
        }
        #endregion DataTable
        #region Columns
        //Leaving to the derived class to fill and manage for the time being
        [NotMapped]
        public IDictionary<String, SwagDataColumn> Columns
        {
            get { return _columns; }
            set { SetValue(ref _columns, value); }
        }
        #endregion Columns
        #endregion Properties

        #region Methods
        public SwagItemPreOrderIterator<SwagData> CreateIterator()
        {
            return new SwagItemPreOrderIterator<SwagData>(this);
        }

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
        #endregion Methods
    }
    #endregion SwagDataTable

    #region SwagDataSet
    public class SwagDataSet : SwagDataGroup
    {
        #region Methods
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
