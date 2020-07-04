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
        Int32 _integrationId, _buildId, _sequence;
        String _name;
        GroupBuild _build = new GroupBuild();
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
        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name
        #region Sequence
        public Int32 Sequence
        {
            get { return _sequence; }
            set { SetValue(ref _sequence, value); }
        }
        #endregion Sequence
        #region BuildId
        public Int32 BuildId
        {
            get { return _buildId; }
            set { SetValue(ref _buildId, value); }
        }
        #endregion BuildId
        #region CoreBuild
        public GroupBuild Build
        {
            get { return _build; }
            set { SetValue(ref _build, value); }
        }
        #endregion CoreBuild
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
