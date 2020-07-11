using Dreamporter.Core;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dreamporter.Core
{
    public abstract class Build : SwagItem<GroupBuild, Build>
    {
        #region Private Members
        Int32 _buildId;
        Int32? _integrationId;
        String _name, _description;
        Boolean _isEnabled = true;
        BooleanContainerExpression _condition = new BooleanContainerExpression();
        Integration _integration;
        #endregion Private Members

        #region Properties
        #region BuildId
        public Int32 BuildId
        {
            get { return _buildId; }
            set { SetValue(ref _buildId, value); }
        }
        #endregion BuildId
        #region IntegrationId
        public Int32? IntegrationId
        {
            get { return _integrationId; }
            set { SetValue(ref _integrationId, value); }
        }
        #endregion IntegrationId
        #region Integration
        [NotMapped]     //Opt out of one to zero to one convention with Integration
        public Integration Integration
        {
            get { return _integration; }
            set { SetValue(ref _integration, value); }
        }
        #endregion Integration
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
        #region Condition
        public BooleanContainerExpression Condition
        {
            get { return _condition; }
            set { SetValue(ref _condition, value); }
        }
        #endregion Condition   
        #region IsEnabled
        public Boolean IsEnabled
        {
            get { return _isEnabled; }
            set { SetValue(ref _isEnabled, value); }
        }
        #endregion IsEnabled
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
