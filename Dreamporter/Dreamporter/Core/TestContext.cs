using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.Core
{
    public class TestContext : SwagItemBase
    {
        string _initialDB;

        #region InitialDB
        public String InitialDB
        {
            get { return _initialDB; }
            set { SetValue(ref _initialDB, value); }
        }
        #endregion InitialDB
    }
}
