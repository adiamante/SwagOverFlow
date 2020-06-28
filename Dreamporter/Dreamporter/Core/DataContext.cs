using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.Core
{
    public abstract class DataContext : ViewModelBaseExtended
    {
        String _name;

        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name
        #region Type
        public abstract Type Type { get; }
        #endregion Type
    }

    public class SqlConnectionDataContext : DataContext
    {
        String _connectionString;

        #region Type
        public override Type Type { get { return typeof(SqlConnectionDataContext); } }
        #endregion Type
        #region ConnectionString
        public String ConnectionString
        {
            get { return _connectionString; }
            set { SetValue(ref _connectionString, value); }
        }
        #endregion ConnectionString
    }
}
