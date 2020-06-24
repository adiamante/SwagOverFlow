using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;

namespace Dreamporter.Builds
{
    public enum SchemaColumnDataType
    {
        String
    }

    public abstract class SchemaBase : ViewModelBaseExtended
    {
        Boolean _isExpanded = true;
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
    }

    public class Schema : SchemaBase
    {
        List<SchemaTable> _tables = new List<SchemaTable>();

        #region Tables
        public List<SchemaTable> Tables
        {
            get { return _tables; }
            set { SetValue(ref _tables, value); }
        }
        #endregion Tables
    }

    public class SchemaTable : SchemaBase
    {
        List<SchemaColumn> _columns = new List<SchemaColumn>();

        #region Columns
        public List<SchemaColumn> Columns
        {
            get { return _columns; }
            set { SetValue(ref _columns, value); }
        }
        #endregion Columns
    }

    public class SchemaColumn : SchemaBase
    {
        SchemaColumnDataType _dataType;

        #region DataType
        public SchemaColumnDataType DataType
        {
            get { return _dataType; }
            set { SetValue(ref _dataType, value); }
        }
        #endregion DataType
    }
}
