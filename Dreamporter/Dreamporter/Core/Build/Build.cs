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
        Int32? _integrationId;
        String _name, _description;
        Boolean _isEnabled = true;
        Integration _integration;
        BooleanContainerExpression _condition = new BooleanContainerExpression();
        ObservableCollection<Schema> _requiredData = new ObservableCollection<Schema>();
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
                //Passing parameters directly is not thread safe (Should run sequentially or create new instance)
                rp.Build = Path;
                RunHandler(context, rp);
            }
        }
        #endregion Methods
    }
}
