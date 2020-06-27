using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.Core
{
    public abstract class Build : SwagItem<GroupBuild, Build>
    {
        #region Private Members
        String _name, _description;
        Boolean _isEnabled = true;
        List<Schema> _requiredData;
        #endregion Private Members

        #region Properties
        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name
        #region Description
        public String Description
        {
            get { return _description; }
            set { SetValue(ref _description, value); }
        }
        #endregion Description
        #region IsEnabled
        public Boolean IsEnabled
        {
            get { return _isEnabled; }
            set { SetValue(ref _isEnabled, value); }
        }
        #endregion IsEnabled
        #region RequiredData
        public List<Schema> RequiredData
        {
            get { return _requiredData; }
            set { SetValue(ref _requiredData, value); }
        }
        #endregion RequiredData
        #region Type
        public abstract Type Type { get; }
        #endregion Type
        #endregion Properties

        #region Initialization
        public Build()
        {

        }
        #endregion Initialization
    }
}
