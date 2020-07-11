﻿using Dreamporter.Core;
using Newtonsoft.Json;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        GroupBuild _build = new GroupBuild() { Sequence = 0 };      //Iterator is dependent on Sequence
        Build _selectedBuild;
        GroupInstruction _instructionTemplates = new GroupInstruction();
        SwagOptionGroup _defaultOptions = new SwagOptionGroup();
        OptionsNode _selectedOptions;
        OptionsNode _optionsTree = new OptionsNode();
        ObservableCollection<Schema> _initialSchemas = new ObservableCollection<Schema>();
        ObservableCollection<DataContext> _dataContexts = new ObservableCollection<DataContext>();
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
        #region Build
        [NotMapped] //Opt out of one to zero to one convention with Group Build
        public GroupBuild Build
        {
            get { return _build; }
            set { SetValue(ref _build, value); }
        }
        #endregion Build
        #region SelectedBuild
        [JsonIgnore]
        [NotMapped]
        public Build SelectedBuild
        {
            get { return _selectedBuild; }
            set { SetValue(ref _selectedBuild, value); }
        }
        #endregion SelectedBuild
        #region InstructionTemplates
        public GroupInstruction InstructionTemplates
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
        #region OptionsTree
        public OptionsNode OptionsTree
        {
            get { return _optionsTree; }
            set { SetValue(ref _optionsTree, value); }
        }
        #endregion OptionsTree
        #region SelectedOptions
        [JsonIgnore]
        [NotMapped]
        public OptionsNode SelectedOptions
        {
            get { return _selectedOptions; }
            set { SetValue(ref _selectedOptions, value); }
        }
        #endregion SelectedOptions
        #region InitialSchemas
        public ObservableCollection<Schema> InitialSchemas
        {
            get { return _initialSchemas; }
            set { SetValue(ref _initialSchemas, value); }
        }
        #endregion InitialSchemas
        #region DataContexts
        public ObservableCollection<DataContext> DataContexts
        {
            get { return _dataContexts; }
            set { SetValue(ref _dataContexts, value); }
        }
        #endregion DataContexts
        #endregion Properties
    }
}
