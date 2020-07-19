using Dreamporter.Core;
using Newtonsoft.Json;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dreamporter.Core
{
    public abstract class Build : SwagItem<GroupBuild, Build>
    {
        #region Private Members
        Int32 _buildId;
        String _name, _description;
        Boolean _isEnabled = true;
        BooleanContainerExpression _condition = new BooleanContainerExpression();
        ObservableCollection<Schema> _requiredData = new ObservableCollection<Schema>();
        Integration _integration;
        Integration _testIntegration;
        Int32? _integrationId;
        Int32? _testIntegrationId;
        Int32 _tabIndex = 0;            //For the UI
        #endregion Private Members

        #region Properties
        #region BuildId
        public Int32 BuildId
        {
            get { return _buildId; }
            set { SetValue(ref _buildId, value); }
        }
        #endregion BuildId
        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name
        #region TabIndex
        [JsonIgnore]
        [NotMapped]
        public Int32 TabIndex
        {
            get { return _tabIndex; }
            set { SetValue(ref _tabIndex, value); }
        }
        #endregion TabIndex
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
        #region Type
        public abstract Type Type { get; }
        #endregion Type
        #region Path
        [JsonIgnore]
        public String Path
        {
            get
            {
                String path = "";
                Build temp = this;
                while (temp != null)
                {
                    path = $"{temp.Name}/{path}";
                    temp = temp.Parent;
                }
                path = path.Trim('/');
                return path;
            }
        }
        #endregion Path
        #region Condition
        public BooleanContainerExpression Condition
        {
            get { return _condition; }
            set { SetValue(ref _condition, value); }
        }
        #endregion Condition  
        #region RequiredData
        public ObservableCollection<Schema> RequiredData
        {
            get { return _requiredData; }
            set { SetValue(ref _requiredData, value); }
        }
        #endregion RequiredData

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
        #region TestIntegrationId
        public Int32? TestIntegrationId
        {
            get { return _testIntegrationId; }
            set { SetValue(ref _testIntegrationId, value); }
        }
        #endregion TestIntegrationId
        #region TestIntegration
        [NotMapped]     //Opt out of one to zero to one convention with Integration
        public Integration TestIntegration
        {
            get { return _testIntegration; }
            set { SetValue(ref _testIntegration, value); }
        }
        #endregion TestIntegration
        #endregion Properties

        #region Initialization
        public Build()
        {

        }
        #endregion Initialization

        #region Methods
        abstract public void RunHandler(RunContext context, RunParams rp);

        public void Run(RunContext context, RunParams rp)
        {
            if (IsEnabled && Condition.Evaluate(rp.Params))
            {
                IsSelected = true;
                IsExpanded = true;
                //Passing parameters directly is not thread safe (Should run sequentially or create new instance)
                rp.Build = Path;
                RunHandler(context, rp);
                IsExpanded = false;
            }
        }
        #endregion Methods
    }
}
