using Dreamporter.Core;
using Newtonsoft.Json;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;

namespace Dreamporter.Core
{
    public class Integration : ViewModelBaseExtended
    {
        #region Private Members
        Int32 _integrationId, _coreBuildId, _profileBuildId;
        CoreGroupBuild _coreBuild = new CoreGroupBuild();
        ProfileGroupBuild _profileBuild = new ProfileGroupBuild();
        List<Instruction> _instructionTemplates = new List<Instruction>();
        SwagOptionGroup _defaultOptions = new SwagOptionGroup();
        SwagOptionGroup _selectedOptions = new SwagOptionGroup();
        List<SwagOptionGroup> _optionsSet = new List<SwagOptionGroup>();
        List<Schema> _initialSchemas = new List<Schema>();
        List<DataContext> _dataContexts = new List<DataContext>();
        #endregion Private Members

        #region Properties
        #region IntegrationId
        public Int32 IntegrationId
        {
            get { return _integrationId; }
            set { SetValue(ref _integrationId, value); }
        }
        #endregion IntegrationId
        #region CoreBuildId
        public Int32 CoreBuildId
        {
            get { return _coreBuildId; }
            set { SetValue(ref _coreBuildId, value); }
        }
        #endregion CoreBuildId
        #region CoreBuild
        public CoreGroupBuild CoreBuild
        {
            get { return _coreBuild; }
            set { SetValue(ref _coreBuild, value); }
        }
        #endregion CoreBuild
        #region ProfileBuildId
        public Int32 ProfileBuildId
        {
            get { return _profileBuildId; }
            set { SetValue(ref _profileBuildId, value); }
        }
        #endregion ProfileBuildId
        #region ProfileBuild
        public ProfileGroupBuild ProfileBuild
        {
            get { return _profileBuild; }
            set { SetValue(ref _profileBuild, value); }
        }
        #endregion ProfileBuild
        #region InstructionTemplates
        public List<Instruction> InstructionTemplates
        {
            get { return _instructionTemplates; }
            set { SetValue(ref _instructionTemplates, value); }
        }
        #endregion InstructionTemplates
        #region DefaultOptions
        public SwagOptionGroup DefaultOptions
        {
            get { return _defaultOptions; }
            set { SetValue(ref _defaultOptions, value); }
        }
        #endregion DefaultOptions
        #region SelectedOptions
        [NotMapped]
        [JsonIgnore]
        public SwagOptionGroup SelectedOptions
        {
            get { return _selectedOptions; }
            set { SetValue(ref _selectedOptions, value); }
        }
        #endregion SelectedOptions
        #region OptionsSet
        public List<SwagOptionGroup> OptionsSet
        {
            get { return _optionsSet; }
            set { SetValue(ref _optionsSet, value); }
        }
        #endregion OptionsSet
        #region InitialSchemas
        public List<Schema> InitialSchemas
        {
            get { return _initialSchemas; }
            set { SetValue(ref _initialSchemas, value); }
        }
        #endregion InitialSchemas
        #region DataContexts
        public List<DataContext> DataContexts
        {
            get { return _dataContexts; }
            set { SetValue(ref _dataContexts, value); }
        }
        #endregion DataContexts
        #endregion Properties
    }
}
