using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;

namespace Dreamporter.Builds
{
    public enum SchemaColumnDataType
    {
        String
    }

    public class Schema : ViewModelBaseExtended
    {
        String _name;
        List<SchemaTable> _tables = new List<SchemaTable>();

        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name

        #region Tables
        public List<SchemaTable> Tables
        {
            get { return _tables; }
            set { SetValue(ref _tables, value); }
        }
        #endregion Tables
    }

    public class SchemaTable : ViewModelBaseExtended
    {
        String _name;
        List<SchemaColumns> _columns = new List<SchemaColumns>();

        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name

        #region Columns
        public List<SchemaColumns> Columns
        {
            get { return _columns; }
            set { SetValue(ref _columns, value); }
        }
        #endregion Columns
    }

    public class SchemaColumns : ViewModelBaseExtended
    {
        String _name;
        SchemaColumnDataType _dataType;

        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name

        #region DataType
        public SchemaColumnDataType DataType
        {
            get { return _dataType; }
            set { SetValue(ref _dataType, value); }
        }
        #endregion DataType
    }
}
