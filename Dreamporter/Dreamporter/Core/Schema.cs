using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;

namespace Dreamporter.Core
{
    #region SchemaColumnDataType
    public enum SchemaColumnDataType
    {
        String,
        DateTime,
        Integer,
        Real
    }
    #endregion SchemaColumnDataType

    public abstract class SchemaBase : ViewModelBaseExtended
    {
        Boolean _isExpanded = true, _isSelected;
        String _name;

        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name

        #region IsExpanded
        public Boolean IsExpanded
        {
            get { return _isExpanded; }
            set { SetValue(ref _isExpanded, value); }
        }
        #endregion IsExpanded

        #region IsSelected
        public Boolean IsSelected
        {
            get { return _isSelected; }
            set { SetValue(ref _isSelected, value); }
        }
        #endregion IsSelected
    }

    public class Schema : SchemaBase
    {
        ObservableCollection<SchemaTable> _tables = new ObservableCollection<SchemaTable>();

        public Schema()
        {
            _tables.CollectionChanged += _tables_CollectionChanged;
        }

        private void _tables_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SchemaTable table in e.NewItems)
                {
                    table.Schema = this;
                }
            }
        }

        #region Tables
        public ObservableCollection<SchemaTable> Tables
        {
            get { return _tables; }
            set { SetValue(ref _tables, value); }
        }
        #endregion Tables

        #region GetDataSet
        public DataSet GetDataSet()
        {
            DataSet ds = new DataSet(Name);

            foreach (SchemaTable table in Tables)
            {
                DataTable dtbl = new DataTable($"{Name}{(string.IsNullOrEmpty(Name) ? "" : ".")}{table.Name}");
                foreach (SchemaColumn column in table.Columns)
                {
                    DataColumn dc = new DataColumn(column.Name);
                    switch (column.DataType)
                    {
                        default:
                        case SchemaColumnDataType.String:
                            dc.DataType = typeof(String);
                            break;
                        case SchemaColumnDataType.DateTime:
                            dc.DataType = typeof(DateTime);
                            break;
                        case SchemaColumnDataType.Integer:
                            dc.DataType = typeof(Int32);
                            break;
                        case SchemaColumnDataType.Real:
                            dc.DataType = typeof(Double);
                            break;
                    }
                    dtbl.Columns.Add(dc);
                }
                ds.Tables.Add(dtbl);
            }

            return ds;
        }
        #endregion GetDataSet

        public void Init()
        {
            foreach (SchemaTable table in Tables)
            {
                table.Schema = this;
                table.Init();
            }
        }
    }

    public class SchemaTable : SchemaBase
    {
        ObservableCollection<SchemaColumn> _columns = new ObservableCollection<SchemaColumn>();

        public Schema Schema { get; set; }
        #region Columns
        public ObservableCollection<SchemaColumn> Columns
        {
            get { return _columns; }
            set { SetValue(ref _columns, value); }
        }
        #endregion Columns

        public SchemaTable()
        {
            _columns.CollectionChanged += _columns_CollectionChanged;
        }

        private void _columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SchemaColumn column in e.NewItems)
                {
                    column.Table = this;
                }
            }
        }

        public void Init()
        {
            foreach (SchemaColumn column in Columns)
            {
                column.Table = this;
            }
        }
    }

    public class SchemaColumn : SchemaBase
    {
        SchemaColumnDataType _dataType;

        public SchemaTable Table { get; set; }
        #region DataType
        [JsonConverter(typeof(StringEnumConverter))]
        public SchemaColumnDataType DataType
        {
            get { return _dataType; }
            set { SetValue(ref _dataType, value); }
        }
        #endregion DataType
    }
}
