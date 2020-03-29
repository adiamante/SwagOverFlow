using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverflowWPF.Collections;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Iterator;
using SwagOverflowWPF.Repository;
using SwagOverflowWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

    public class SwagDataColumn : ViewModelBaseExtended
    {
        #region Private Members
        Boolean _readOnly = false, _isSelected, _isChecked_Visiblity, _isChecked_filter, _ignoreAppliedFilters = false;
        Decimal _total = 0.0m;
        String _columnName, _expression, _dataTemplate;
        Binding _binding = null;
        String _jsonBinding;
        #endregion Private Members

        #region Properties
        #region ReadOnly
        public Boolean ReadOnly
        {
            get { return _readOnly; }
            set { SetValue(ref _readOnly, value); }
        }

        [JsonIgnore]
        public Boolean IsReadOnly
        {
            get { return _readOnly; }
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

                if (!String.IsNullOrEmpty(Expression))
                {
                    dgc.IsReadOnly = true;
                }
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
    }

    public class SwagDataTable  : SwagGroup<SwagDataRow>
    {
        #region Private Members
        DataTable _dataTable;
        SwagContext _context;
        Dictionary<DataRow, SwagDataRow> _dictChildren = new Dictionary<DataRow, SwagDataRow>();
        ConcurrentObservableOrderedDictionary<String, SwagDataColumn> _columns = new ConcurrentObservableOrderedDictionary<String, SwagDataColumn>();
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
