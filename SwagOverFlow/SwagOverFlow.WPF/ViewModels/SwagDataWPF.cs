using Microsoft.Win32;
using Newtonsoft.Json;
using SwagOverFlow.Iterator;
using SwagOverFlow.ViewModels;
using SwagOverFlow.Data.Converters;
using SwagOverFlow.Logger;
using SwagOverFlow.Utils;
using SwagOverFlow.WPF.Commands;
using SwagOverFlow.WPF.Collections;
using SwagOverFlow.WPF.UI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using SwagOverFlow.Collections;
using SwagOverFlow.Data.Persistence;
using SwagOverFlow.Commands;
using SwagOverFlow.WPF.Controls;

namespace SwagOverFlow.WPF.ViewModels
{
    #region SwagDataSet
    public class SwagDataSetWPF : SwagDataSet
    {
        #region Private Members
        SwagSettingGroup _settings;
        SwagTabGroup _tabs;
        ICommand _filterTabsCommand, _addDataSetCommand, _addDataTableCommand;
        SwagData _selectedChild;
        CollectionViewSource _childrenCollectionViewSource;
        #endregion Private Members

        #region Properties
        #region Settings
        public SwagSettingGroup Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new SwagSettingGroup();
                    _settings["Tabs"] = new SwagSettingGroup() { Icon = PackIconCustomKind.TableSearch };
                    _settings["Tabs"]["Search"] = new SwagSettingGroup() { Icon = PackIconCustomKind.Search };
                    _settings["Tabs"]["Search"]["Text"] = new SwagSettingString() { Icon = PackIconCustomKind.KeyValue };
                    _settings["Tabs"]["Search"]["FilterMode"] = new SwagSetting<FilterMode>() { SettingType = SettingType.DropDown, Value = FilterMode.CONTAINS, Icon = PackIconCustomKind.Filter, ItemsSource = (FilterMode[])Enum.GetValues(typeof(FilterMode)) };
                    _settings["Search"] = new SwagSettingGroup() { Icon = PackIconCustomKind.GlobalSearch };
                    _settings["Search"]["Text"] = new SwagSetting<String>() { Icon = PackIconCustomKind.KeyValue };
                    _settings["Search"]["FilterMode"] = new SwagSetting<FilterMode>() { SettingType = SettingType.DropDown, Value = FilterMode.CONTAINS, Icon = PackIconCustomKind.Filter, ItemsSource = (FilterMode[])Enum.GetValues(typeof(FilterMode)) };
                }
                return _settings;
            }
            set
            {
                SetValue(ref _settings, value);
            }
        }
        #endregion Settings
        #region Tabs
        public SwagTabGroup Tabs
        {
            get
            {
                if (_tabs == null)
                {
                    _tabs = new SwagTabGroup();
                    _tabs["Tabs"] = new SwagTabItem() { Icon = PackIconCustomKind.TableSearch, ViewModel = this };
                    _tabs["Search"] = new SwagTabItem() { Icon = PackIconCustomKind.GlobalSearch, ViewModel = this };
                    _tabs["Settings"] = new SwagTabItem() { Icon = PackIconCustomKind.Settings, ViewModel = this };
                }
                return _tabs;
            }
            set
            {
                SetValue(ref _tabs, value);
                _tabs["Tabs"].ViewModel = this;
                _tabs["Search"].ViewModel = this;
                _tabs["Settings"].ViewModel = this;
            }
        }
        #endregion Tabs
        #region SelectedChild
        [NotMapped]
        [JsonIgnore]
        public SwagData SelectedChild
        {
            get { return _selectedChild; }
            set { SetValue(ref _selectedChild, value); }
        }
        #endregion SelectedChild
        #region ChildrenView
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView
        #region FilterTabsCommand
        public ICommand FilterTabsCommand
        {
            get
            {
                return _filterTabsCommand ?? (_filterTabsCommand =
                    new RelayCommand(() =>
                    {
                        String filter = _settings["Tabs"]["Search"]["Text"].GetValue<String>();
                        FilterMode filterMode = _settings["Tabs"]["Search"]["FilterMode"].GetValue<FilterMode>();
                        ChildrenView.Filter = (item) =>
                        {
                            SwagData swagData = (SwagData)item;
                            Boolean itemMatch = SearchHelper.Evaluate(swagData.Display, filter, false, filterMode, false);
                            Boolean childDataSetMatch = false;

                            if (swagData is SwagDataSetWPF)
                            {
                                SwagDataSetWPF childDataSet = (SwagDataSetWPF)swagData;
                                childDataSet.Settings["Tabs"]["Search"]["Text"].SetValue(filter);
                                childDataSet.Settings["Tabs"]["Search"]["FilterMode"].SetValue(filterMode);
                                childDataSet.FilterTabsCommand.Execute(null);
                                childDataSetMatch = !childDataSet.ChildrenView.IsEmpty;
                            }

                            return itemMatch || childDataSetMatch;
                        };
                    }));
            }
        }
        #endregion FilterTabsCommand
        #region AddDataSetCommand
        public ICommand AddDataSetCommand
        {
            get
            {
                return _addDataSetCommand ?? (_addDataSetCommand =
                    new RelayCommand(() =>
                    {
                        SwagDataSetWPF newDataSet = new SwagDataSetWPF();
                        newDataSet.Display = $"Set {this.Children.Count + 1}";
                        Children.Add(newDataSet);
                    }));
            }
        }
        #endregion AddDataSetCommand
        #region AddDataTableCommand
        public ICommand AddDataTableCommand
        {
            get
            {
                return _addDataTableCommand ?? (_addDataTableCommand =
                    new RelayCommand(() =>
                    {
                        SwagDataTable newDataTable = new SwagDataTable();
                        newDataTable.Display = $"Table {this.Children.Count + 1}";
                        Children.Add(newDataTable);
                    }));
            }
        }
        #endregion AddDataTableCommand
        #endregion Properties

        #region Initialization
        public SwagDataSetWPF()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
        }

        public SwagDataSetWPF(DataSet dataSet) : this()
        {
            foreach (DataTable dt in dataSet.Tables)
            {
                Children.Add(new SwagDataTable(dt) { Display = dt.TableName });
            }
        }
        #endregion Initialization

        #region Methods
        public void LoadFiles(IEnumerable<String> files, IEnumerable<KeyValuePairViewModel<String, ParseViewModel>> parseMappers)
        {
            foreach (String file in files)
            {
                SwagLogger.LogStart(this, "Load {file}", file);
                SwagData child = SwagDataHelper.FromFile(file, parseMappers);
                SwagLogger.LogEnd(this, "Load {file}", file);

                if (child != null)
                {
                    Children.Add(child);
                }
                else
                {
                    SwagLogger.Log("Load {file} did not yield data (unsupported extenstion).", file);
                }
            }
        }

        public override DataSet GetDataSet()
        {
            DataSet ds = new DataSet(Display);
            foreach (SwagData swagData in ChildrenView)
            {
                switch (swagData)
                {
                    case SwagDataTable swagDataTable:
                        foreach (DataTable dt in swagDataTable.GetDataSet().Tables)
                        {
                            DataTable dtCopy = dt.Copy();
                            ds.Tables.Add(dtCopy);
                        }
                        break;
                    case SwagDataSet swagDataSet:
                        foreach (DataTable dt in swagDataSet.GetDataSet().Tables)
                        {
                            DataTable dtCopy = dt.Copy();
                            dtCopy.TableName = $"{swagDataSet.Display}.{dt.TableName}";
                            ds.Tables.Add(dtCopy);
                        }
                        break;
                }
            }
            return ds;
        }
        #endregion Methods
    }
    #endregion SwagDataSet

    #region SwagDataHelper
    public static class SwagDataHelper
    {
        public static SwagData FromFile(String file, IEnumerable<KeyValuePairViewModel<String, ParseViewModel>> parseMappers)
        {
            String filename = Path.GetFileName(file);
            String ext = Path.GetExtension(file).TrimStart('.');

            KeyValuePairViewModel<String, ParseViewModel> parseMapper = parseMappers.FirstOrDefault(pm => pm.Key != null && pm.Key.ToLower() == ext.ToLower());
            if (parseMapper != null)
            {
                switch (parseMapper.Value.ParseStrategy)
                {
                    case ParseStrategy.Csv:
                    case ParseStrategy.Dbf:
                        DataTableConvertContext dataTableConvertContext = new DataTableConvertContext();
                        switch (parseMapper.Value.ParseStrategy)
                        {
                            case ParseStrategy.Csv:
                                dataTableConvertContext.Converter = new DataTableCsvFileConverter();
                                break;
                            case ParseStrategy.Dbf:
                                dataTableConvertContext.Converter = new DataTableDbfFileConverter();
                                break;
                        }
                        dataTableConvertContext.Params = new DataTableConvertParams();
                        PropertyCopy.Copy(parseMapper.Value, dataTableConvertContext.Params);
                        DataTableConverterHelper.ConverterFileContexts[ext] = dataTableConvertContext;
                        break;
                    case ParseStrategy.Xml:
                    case ParseStrategy.Json:
                        DataSetConvertContext dataSetConvertContext = new DataSetConvertContext();
                        switch (parseMapper.Value.ParseStrategy)
                        {
                            case ParseStrategy.Xml:
                                dataSetConvertContext.Converter = new DataSetXmlFileConverter();
                                break;
                            case ParseStrategy.Json:
                                dataSetConvertContext.Converter = new DataSetJsonFileConverter();
                                break;
                        }
                        dataSetConvertContext.Params = new DataSetConvertParams();
                        PropertyCopy.Copy(parseMapper.Value, dataSetConvertContext.Params);
                        DataSetConverterHelper.ConverterFileContexts[ext] = dataSetConvertContext;
                        break;
                }
            }

            DataSetConvertContext dataSetContext = DataSetConverterHelper.ConverterFileContexts[ext];
            if (dataSetContext != null)
            {
                return new SwagDataSetWPF(dataSetContext.ToDataSet(file)) { Display = filename };
            }

            DataTableConvertContext dataTableContext = DataTableConverterHelper.ConverterFileContexts[ext];
            if (dataTableContext != null)
            {
                return new SwagDataTable(dataTableContext.ToDataTable(file)) { Display = filename };
            }

            return null;
        }
    }
    #endregion SwagDataHelper

}
