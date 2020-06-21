using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.Builds
{
    public abstract class DataStore : ViewModelBaseExtended
    {
        String _name;

        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name
    }

    public class SqlConnectionStore : DataStore
    {
        String _connectionString;

        #region ConnectionString
        public String ConnectionString
        {
            get { return _connectionString; }
            set { SetValue(ref _connectionString, value); }
        }
        #endregion ConnectionString
    }
}
