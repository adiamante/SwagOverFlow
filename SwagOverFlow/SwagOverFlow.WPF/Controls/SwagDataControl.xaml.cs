using Microsoft.Win32;
//using Microsoft.WindowsAPICodePack.Shell;
using SwagOverFlow.Utils;
using SwagOverFlow.WPF.UI;
using SwagOverFlow.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SwagOverFlow.ViewModels;
using System.ComponentModel;
using SwagOverFlow.Data.Converters;
using SwagOverFlow.Logger;
using SwagOverFlow.Commands;
using SwagOverFlow.Iterator;
using MahApps.Metro.IconPacks;
using System.Windows.Input;
using System.Threading.Tasks;
using SwagOverFlow.WPF.Commands;
using Newtonsoft.Json.Linq;
using SwagOverFlow.WPF.Services;

namespace SwagOverFlow.WPF.Controls
{
    /// <summary>
    /// Interaction logic for SwagDataControl.xaml
    /// </summary>
    public partial class SwagDataControl : SwagControlBase
    {
        #region Properties
        #region SwagDataSet
        public static DependencyProperty SwagDataSetProperty =
                DependencyProperty.Register(
                    "SwagDataSet",
                    typeof(SwagDataSet),
                    typeof(SwagDataControl),
                    new FrameworkPropertyMetadata(null, SwagDataSetPropertyChanged));

        private static void SwagDataSetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwagDataControl swagDataControl = (SwagDataControl)d;
            if (swagDataControl.SwagDataSet != null)
            {
                SwagDataSet swagDataSet = swagDataControl.SwagDataSet;
                InitSwagDataSet(swagDataSet);
            }
        }

        public static void InitSwagDataSet(SwagDataSet swagDataSet)
        {
            #region FilterTabsCommand
            SwagItemPreOrderIterator<SwagData> itrSwagData = swagDataSet.CreateIterator();
            for (SwagData swagData = itrSwagData.First(); !itrSwagData.IsDone; swagData = itrSwagData.Next())
            {
                if (swagData is SwagDataSet sds)
                {
                    sds.FilterTabsCommand =
                    new RelayCommand(() =>
                    {
                        String filter = sds.Settings["Tabs"]["Search"]["Text"].GetValue<String>();
                        FilterMode filterMode = sds.Settings["Tabs"]["Search"]["FilterMode"].GetValue<FilterMode>();
                        UIHelper.GetCollectionView(sds.Children).Filter = (item) =>
                        {
                            SwagData swagData = (SwagData)item;
                            Boolean itemMatch = SearchHelper.Evaluate(swagData.Display, filter, false, filterMode, false);
                            Boolean childDataSetMatch = false;

                            if (swagData is SwagDataSet subDataSet)
                            {
                                subDataSet.Settings["Tabs"]["Search"]["Text"].SetValue(filter);
                                subDataSet.Settings["Tabs"]["Search"]["FilterMode"].SetValue(filterMode);
                                subDataSet.FilterTabsCommand.Execute(null);
                                ICollectionView subchildren = UIHelper.GetCollectionView(subDataSet.Children);
                                childDataSetMatch = !subchildren.IsEmpty;
                            }

                            return itemMatch || childDataSetMatch;
                        };
                    });
                }
            }
            #endregion FilterTabsCommand

            #region InitSettings
            swagDataSet.Settings.TryAddChildSetting("Tabs", new SwagSettingGroup() { Icon = PackIconMaterialKind.TableSearch });
            swagDataSet.Settings["Tabs"].TryAddChildSetting("Search", new SwagSettingGroup() { Icon = PackIconMaterialKind.Magnify });
            swagDataSet.Settings["Tabs"]["Search"].TryAddChildSetting("Text", new SwagSettingString() { Icon = PackIconBoxIconsKind.RegularText });
            swagDataSet.Settings["Tabs"]["Search"].TryAddChildSetting("FilterMode", new SwagSetting<FilterMode>() { SettingType = SettingType.DropDown, Value = FilterMode.CONTAINS, Icon = PackIconUniconsKind.Filter, ItemsSource = (FilterMode[])Enum.GetValues(typeof(FilterMode)) });
            swagDataSet.Settings.TryAddChildSetting("Search", new SwagSettingGroup() { Icon = PackIconMaterialKind.TextSearch });
            swagDataSet.Settings["Search"].TryAddChildSetting("Text", new SwagSettingString { Icon = PackIconBoxIconsKind.RegularText });
            swagDataSet.Settings["Search"].TryAddChildSetting("FilterMode", new SwagSetting<FilterMode>() { SettingType = SettingType.DropDown, Value = FilterMode.CONTAINS, Icon = PackIconUniconsKind.Filter, ItemsSource = (FilterMode[])Enum.GetValues(typeof(FilterMode)) });
            #endregion InitSettings

            #region InitTabs
            SwagTabGroup tabs = new SwagTabGroup();
            tabs["Tabs"] = new SwagTabItem() { Icon = PackIconMaterialKind.TableSearch };
            tabs["Parse"] = new SwagTabGroup() { Icon = PackIconZondiconsKind.DocumentAdd };
            tabs["Parse"]["Paste"] = new SwagTabGroup() { Icon = PackIconZondiconsKind.Paste };
            tabs["Parse"]["Paste"]["TSV"] = new SwagTabItem() { Icon = PackIconMaterialKind.AlphaTBoxOutline };
            tabs["Parse"]["Paste"]["JSON"] = new SwagTabItem() { Icon = PackIconMaterialKind.CodeJson };
            tabs["Parse"]["Paste"]["XML"] = new SwagTabItem() { Icon = PackIconMaterialKind.Xml };
            tabs["Search"] = new SwagTabItem() { Icon = PackIconMaterialKind.TextSearch };
            tabs["Session"] = new SwagTabItem() { Icon = PackIconMaterialKind.WindowRestore };
            tabs["Test"] = new SwagTabItem() { Icon = PackIconOcticonsKind.Beaker };
            tabs["Settings"] = new SwagTabItem() { Icon = PackIconMaterialKind.Cog };
            swagDataSet.Tabs = tabs;
            SwagItemPreOrderIterator<SwagTabItem> itrTabs = tabs.CreateIterator();
            for (SwagTabItem tabItem = itrTabs.First(); !itrTabs.IsDone; tabItem = itrTabs.Next())
            {
                tabItem.ViewModel = swagDataSet;
            }
            #endregion InitTabs
        }

        public SwagDataSet SwagDataSet
        {
            get { return (SwagDataSet)GetValue(SwagDataSetProperty); }
            set
            {
                SetValue(SwagDataSetProperty, value);
                OnPropertyChanged();
                InitDataSet();
            }
        }
        #endregion SwagDataSet
        #region DataTemplates
        public static readonly DependencyProperty DataTemplatesProperty =
            DependencyProperty.Register("DataTemplates", typeof(SwagTemplateCollection), typeof(SwagDataControl),
            new FrameworkPropertyMetadata(new SwagTemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public SwagTemplateCollection TabItemTemplates
        {
            get { return (SwagTemplateCollection)GetValue(DataTemplatesProperty); }
            set { SetValue(DataTemplatesProperty, value); }
        }
        #endregion DataTemplates
        #region ParseMapper
        public List<KeyValuePairViewModel<String, ParseViewModel>> ParseMapper
        {
            get
            {
                return SwagWindow.GlobalSettings["SwagData"]["ParseMapper"].GetValue<List<KeyValuePairViewModel<String, ParseViewModel>>>();
            }
        }
        #endregion ParseMapper
        #region SessionEnabled
        public Boolean SessionEnabled
        {
            get { return SwagWindow.GlobalSettings["SwagData"]["Session"]["Enabled"].GetValue<Boolean>(); }
            set { SwagWindow.GlobalSettings["SwagData"]["Session"]["Enabled"].SetValue(value); }
        }
        #endregion SessionEnabled
        #region SessionName
        public String SessionName
        {
            get { return SwagWindow.GlobalSettings["SwagData"]["Session"]["Name"].GetValue<String>(); }
            set { SwagWindow.GlobalSettings["SwagData"]["Session"]["Name"].SetValue(value); }
        }
        #endregion SessionName
        #endregion Properties

        #region Initialization
        public SwagDataControl()
        {
            InitializeComponent();
        }

        private void ControlInstance_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
            InitDataSet();
        }

        public void Initialize()
        {
            #region Prevents Designer Error
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }
            #endregion Prevents Designer Error

            SwagWindow.GlobalSettings.TryAddChildSetting("SwagData", new SwagSettingGroup() { Icon = PackIconMaterialKind.TableMultiple });
            SwagSetting<List<KeyValuePairViewModel<string, ParseViewModel>>> ssParseMapper =
            new SwagSetting<List<KeyValuePairViewModel<string, ParseViewModel>>>()
            {
                Icon = PackIconModernKind.ArrowLeftRight,
                Value = new List<KeyValuePairViewModel<string, ParseViewModel>>()
            };
            SwagWindow.GlobalSettings["SwagData"].TryAddChildSetting("ParseMapper", ssParseMapper);
            SwagWindow.GlobalSettings["SwagData"].TryAddChildSetting("Session", new SwagSettingGroup() { Icon = PackIconMaterialKind.WindowRestore });
            SwagWindow.GlobalSettings["SwagData"]["Session"].TryAddChildSetting("Enabled", new SwagSettingBoolean { Icon = PackIconMaterialKind.AlphaEBoxOutline });
            SwagWindow.GlobalSettings["SwagData"]["Session"].TryAddChildSetting("Name", new SwagSettingString { Icon = PackIconMaterialKind.AlphaNBoxOutline });
        }

        public void InitDataSet()
        {
            if (SwagDataSet != null)
            {
                SwagDataSet.SwagItemChanged += SwagDataSet_SwagItemChanged;
            }
        }

        private void SwagDataSet_SwagItemChanged(object sender, SwagItemChangedEventArgs e)
        {
            if (!SwagWindow.CommandManager.IsFrozen)
            {
                Boolean canUndo = true;
                String message = e.Message;

                switch (e.PropertyChangedArgs)
                {
                    case PropertyChangedExtendedEventArgs exArgs:
                        switch (exArgs.Object)
                        {
                            #region SwagDataTable
                            case SwagDataTable swagDataTable:
                                switch (exArgs.PropertyName)
                                {
                                    case "Parent":
                                    case "ColumnsVisibilityView":
                                    case "ColumnsFilterView":
                                    case "FilterCommand":
                                    case "ImportCommand":
                                    case "ExportCommand":
                                    case "ApplyColumnVisibilityFilterCommand":
                                    case "ApplyColumnFiltersFilterCommand":
                                    case "ResetColumnsCommand":
                                    case "Tabs":
                                    case "Settings":
                                    case "IsInitialized":
                                        canUndo = false;
                                        break;
                                    case "Sequence":
                                        if ((Int32)exArgs.OldValue == -1)
                                        {
                                            canUndo = false;
                                        }
                                        break;
                                }
                                break;
                            #endregion SwagDataTable
                            #region SwagDataColumn
                            case SwagDataColumn swagDataColumn:
                                switch (exArgs.PropertyName)
                                {
                                    case "Parent":
                                    case "SwagDataTable":
                                        canUndo = false;
                                        break;
                                    case "ColSeq":
                                    case "Sequence":
                                        if ((Int32)exArgs.OldValue == -1)
                                        {
                                            canUndo = false;
                                        }
                                        break;
                                    case "IsColumnFilterOpen":    //Popup StaysOpen="False" leads to wierd interactions
                                        canUndo = false;
                                        break;
                                }
                                break;
                            #endregion SwagDataColumn
                            #region SwagDataRow
                            case SwagDataRow swagDataRow:
                                switch (e.PropertyChangedArgs.PropertyName)
                                {
                                    case "Parent":
                                        canUndo = false;
                                        break;
                                    case "ColSeq":
                                    case "Sequence":
                                        if ((Int32)exArgs.OldValue == -1)
                                        {
                                            canUndo = false;
                                        }
                                        break;
                                    case "Value":
                                        JObject dif = JsonHelper.FindDiff((JObject)exArgs.NewValue, (JObject)exArgs.OldValue);
                                        if (dif.Count > 0)
                                        {

                                            message = swagDataRow.Path;
                                            foreach (KeyValuePair<String, JToken> kvp in dif)
                                            {
                                                JObject difLine = (JObject)kvp.Value;
                                                message += $"\n\t[{kvp.Key}] {difLine["-"].ToString()} => {difLine["+"].ToString()}";
                                            }
                                        }
                                        else
                                        {
                                            canUndo = false;
                                        }
                                        break;
                                    case "DataRow":
                                        if (exArgs.OldValue == null)
                                        {
                                            canUndo = false;
                                        }
                                        break;
                                }
                                break;
                                #endregion SwagDataRow
                        }

                        #region SwagPropertyChangedCommand
                        if (canUndo)
                        {
                            SwagPropertyChangedCommand cmd = new SwagPropertyChangedCommand(
                                exArgs.PropertyName,
                                exArgs.Object,
                                exArgs.OldValue,
                                exArgs.NewValue);
                            cmd.Display = message;

                            SwagWindow.CommandManager.AddCommand(cmd);
                        }
                        #endregion SwagPropertyChangedCommand
                        break;
                    case CollectionPropertyChangedEventArgs colArgs:
                        if (colArgs.NewItems != null)
                        {
                            foreach (SwagData swagData in colArgs.NewItems)
                            {
                                if (swagData is SwagDataSet subSet)
                                {
                                    InitSwagDataSet(subSet);
                                }
                            }
                        }
                        break;
                }
            }
        }
        #endregion Initialization

        #region Drop
        private void SwagData_Drop(object sender, DragEventArgs e)
        {
            List<String> allFiles = new List<string>();

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                allFiles = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
            }
            else
            {
                //https://searchcode.com/codesearch/view/28333948/
                //Does not seem to work in .NET CORE; Gonna need to research if we want dropping inner zip files
                //String[] formats = e.Data.GetFormats();

                //foreach (var format in formats)
                //{
                //    // Shell items are passed using the "Shell IDList Array" format. 
                //    if (format == "Shell IDList Array")
                //    {
                //        // Retrieve the ShellObjects from the data object
                //        ShellObjectCollection shellObjects = ShellObjectCollection.FromDataObject(
                //            (System.Runtime.InteropServices.ComTypes.IDataObject)e.Data);

                //        foreach (ShellObject shellObject in shellObjects)
                //        {
                //            allFiles.Add(shellObject.ParsingName);
                //        }
                //    }
                //}
            }

            if (allFiles.Count > 0)
            {
                SwagDataSet.LoadFiles(allFiles, ParseMapper);
            }
        }
        #endregion Drop

        #region Search
        private async void Search_OnSearch(object sender, RoutedEventArgs e)
        {
            SearchTextBox searchTextBox = (SearchTextBox)sender;
            SwagData swagData = (SwagData)((SwagTabItem)searchTextBox.DataContext).ViewModel;
            SwagItemPreOrderIterator<SwagData> itrSwagData = SwagDataSet.CreateIterator();

            SwagWindow.GlobalIsBusy = true;
            await Task.Run(() =>
            {
                for (SwagData sd = itrSwagData.First(); !itrSwagData.IsDone; sd = itrSwagData.Next())
                {
                    if (sd is SwagDataTable sdg && !sdg.IsInitialized)
                    {
                        SwagDataGrid.InitSwagDataTable(sdg);
                    }
                }
            });
            SwagWindow.GlobalIsBusy = false;

            SwagDataResult swagDataResult = SwagDataSet.Search(searchTextBox.Text, searchTextBox.FilterMode,
                (sdc, sdr, searchValue, filterMode) =>
                {
                    String compareTarget = sdr.DataRow[sdc.ColumnName].ToString();
                    String compareValue = searchValue;
                    return SearchHelper.Evaluate(compareTarget, compareValue, false, filterMode, false);
                });

            swagData.SwagDataResult = swagDataResult;
        }

        private void Search_ResultGo_Click(object sender, RoutedEventArgs e)
        {
            SwagDataResult swagDataResult = (SwagDataResult)((MenuItem)sender).DataContext;
            SwagDataResult currentResult = swagDataResult;

            switch (currentResult)
            {
                case SwagDataSetResultGroup setResult:
                    View((SwagDataSet)setResult.SwagData);
                    break;
                case SwagDataTableResultGroup tableResult:
                    View((SwagDataTable)tableResult.SwagData);
                    break;
                case SwagDataColumnResultGroup columnResult:
                    View((SwagDataColumn)columnResult.SwagData);
                    break;
                case SwagDataRowResult rowResult:
                    View(rowResult);
                    break;
            }
        }

        private void View(SwagDataSet swagDataSet)
        {
            if (swagDataSet.Parent != null && swagDataSet.Parent is SwagDataSet)
            {
                SwagDataSet parent = (SwagDataSet)swagDataSet.Parent;
                parent.SelectedChild = swagDataSet;
                View(parent);
            }
        }

        private void View(SwagDataTable swagDataTable)
        {
            if (swagDataTable.Parent != null && swagDataTable.Parent is SwagDataSet)
            {
                SwagDataSet parent = (SwagDataSet)swagDataTable.Parent;
                parent.SelectedChild = swagDataTable;
                View(parent);
            }
        }

        private void View(SwagDataColumn swagDataColumn)
        {
            if (swagDataColumn.Parent != null && swagDataColumn.Parent is SwagDataTable)
            {
                SwagDataTable parent = (SwagDataTable)swagDataColumn.Parent;
                parent.SelectedColumn = swagDataColumn;
                View(parent);
            }
        }

        private void View(SwagDataRowResult rowResult)
        {
            SwagDataColumn swagDataColumn = (SwagDataColumn)rowResult.Parent.SwagData;
            swagDataColumn.SwagDataTable.SelectedRow = rowResult;
            View(swagDataColumn);
        }
        #endregion Search

        #region SwagDataHeader ContextMenu

        private void SwagDataHeader_Close(object sender, RoutedEventArgs e)
        {
            SwagData swagData = (SwagData)((FrameworkElement)sender).DataContext;
            swagData.Parent.Children.Remove(swagData);
        }

        private void SwagDataHeader_CloseSiblings(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to close all siblings?", "Warning", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                SwagData swagData = (SwagData)((FrameworkElement)sender).DataContext;
                if (swagData.Parent != null)
                {
                    swagData.Parent.Children.Clear();
                    swagData.Parent.Children.Add(swagData);
                }
            }
        }

        private void SwagDataHeader_CloseAll(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to close all in set?", "Warning", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                SwagData swagData = (SwagData)((FrameworkElement)sender).DataContext;
                if (swagData.Parent != null)
                {
                    swagData.Parent.Children.Clear();
                }
            }
        }

        private void SwagDataHeader_ContextMenuOpened(object sender, RoutedEventArgs e)
        {
            SwagData swagData = (SwagData)((FrameworkElement)sender).DataContext;
            if (swagData.Parent != null && swagData.Parent is SwagDataSet)
            {
                SwagDataSet swagDataSet = (SwagDataSet)swagData.Parent;
                if (swagDataSet.SelectedChild != null)
                {
                    swagDataSet.SelectedChild.IsSelected = false;
                }
                swagDataSet.SelectedChild = swagData;
                swagData.IsSelected = true;
            }
        }

        private void SwagDataSetHeader_Export(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender;
            SwagDataSet swagDataSet = (SwagDataSet)fe.DataContext;
            ParseStrategy parseStrategy = (ParseStrategy)fe.Tag;
            
            switch (parseStrategy)
            {
                case ParseStrategy.Sqlite:
                    SaveFileDialog sfdSqlite = new SaveFileDialog();
                    sfdSqlite.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    sfdSqlite.FileName = Path.ChangeExtension(swagDataSet.Display, null);
                    sfdSqlite.Filter = "SQLite files (*.db;*.sqlite)|*.db;*.sqlite";
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (sfdSqlite.ShowDialog() ?? false)
                        {
                            DataSetSqliteFileConverter dsConverter = new DataSetSqliteFileConverter();
                            dsConverter.FromDataSet(null, swagDataSet.GetDataSet(), sfdSqlite.FileName);
                        }
                    }));
                    break;
                case ParseStrategy.Csv:
                    System.Windows.Forms.FolderBrowserDialog fbdCsv = new System.Windows.Forms.FolderBrowserDialog();
                    fbdCsv.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (fbdCsv.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            DataSetCsvFileConverter dsConverter = new DataSetCsvFileConverter();

                            SwagItemPreOrderIterator<SwagData> itrSwagData = swagDataSet.CreateIterator();
                            swagDataSet.Display = swagDataSet.Display ?? "DataSet";
                            for (SwagData sd = itrSwagData.First(); !itrSwagData.IsDone; sd = itrSwagData.Next())
                            {
                                if (sd is SwagDataSet sds)
                                {
                                    String destFolder = "";

                                    //If exporting root and root has blank display, use DataSet
                                    if (swagDataSet == SwagDataSet && string.IsNullOrEmpty(swagDataSet.Display))
                                    {
                                        destFolder = Path.Combine(fbdCsv.SelectedPath, "DataSet", sds.Path);
                                    }
                                    else
                                    {
                                        destFolder = Path.Combine(fbdCsv.SelectedPath, sds.Path);
                                    }

                                    //limit to depth of 1 when exporting DataSet
                                    dsConverter.FromDataSet(null, sds.GetDataSet(1), destFolder);
                                }
                            }
                        }
                    }));
                    break;
            }
        }

        private void Search_ResultGo_Opened(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)sender;
            SwagDataResult swagResult = (SwagDataResult)fe.DataContext;
            swagResult.IsSelected = true;
        }

        #endregion SwagDataHeader ContextMenu

        #region Parse
        #region Paste
        private void Parse_Paste_TSV_txtPreviewExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                String text = Clipboard.GetText();
                DataTableConvertParams cp = new DataTableConvertParams() { FieldDelim = '\t' };
                IDataTableConverter converter = new DataTableCsvStringConverter();
                DataTable dt = converter.ToDataTable(cp, text);
                SwagDataSet.AddDataTableCommand.Execute(dt);
                e.Handled = true;
            }
        }

        private void Parse_Paste_JSON_txtPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                String text = Clipboard.GetText();
                DataSetConvertParams cp = new DataSetConvertParams();
                IDataSetConverter converter = new DataSetJsonStringConverter();
                DataSet ds = converter.ToDataSet(cp, text);
                SwagDataSet.AddDataSetCommand.Execute(ds);
                e.Handled = true;
            }
        }

        private void Parse_Paste_XML_txtPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                String text = Clipboard.GetText();
                DataSetConvertParams cp = new DataSetConvertParams();
                IDataSetConverter converter = new DataSetXmlStringConverter();
                DataSet ds = converter.ToDataSet(cp, text);
                SwagDataSet.AddDataSetCommand.Execute(ds);
                e.Handled = true;
            }
        }
        #endregion Paste
        #endregion Parse

        #region Test
        private void Test_ThrowError_Click(object sender, RoutedEventArgs e)
        {
            throw new Exception("Test Exception", new Exception("Inner Exception"));
        }
        #endregion Test

        #region Events
        private async void Session_Save(object sender, RoutedEventArgs e)
        {
            SwagDataSet.Display = SessionName;
            SwagItemPreOrderIterator<SwagData> itrSwagData = SwagDataSet.CreateIterator();
            SwagWindow.GlobalIsBusy = true;

            await Task.Run(() =>
            {
                for (SwagData sd = itrSwagData.First(); !itrSwagData.IsDone; sd = itrSwagData.Next())
                {
                    if (sd is SwagDataTable sdt && !sdt.IsInitialized)
                    {
                        SwagDataGrid.InitSwagDataTable(sdt);
                    }
                }
            });

            SwagDataSet sdsSession = SwagWPFContainer.SwagDataService.SwagDataSets.Where(sds => sds.Display == SessionName).FirstOrDefault();
            if (sdsSession == SwagDataSet)
            {
                SwagWPFContainer.SwagDataService.SwagDataSets.Update(SwagDataSet);
            }
            else
            {
                SwagWPFContainer.SwagDataService.SwagDataSets.Add(SwagDataSet);
            }
            SwagWindow.GlobalIsBusy = false;
            SwagWPFContainer.SwagDataService.Save();
        }

        private async void Session_Load(object sender, RoutedEventArgs e)
        {
            SwagDataSet sdsSession = null;

            SwagWindow.GlobalIsBusy = true;
            await Task.Run(() =>
            {
                sdsSession = SwagWPFContainer.SwagDataService.SwagDataSets.Where(sds => sds.Display == SessionName).FirstOrDefault();
            });

            if (sdsSession != null)
            {
                SwagWPFContainer.SwagDataService.Init(sdsSession);
                SwagDataSet = sdsSession;
                SwagItemPreOrderIterator<SwagData> itrSwagData = SwagDataSet.CreateIterator();
                for (SwagData sd = itrSwagData.First(); !itrSwagData.IsDone; sd = itrSwagData.Next())
                {
                    if (sd is SwagDataTable sdt)
                    {
                        if (!sdt.IsInitialized)
                        {
                            SwagDataGrid.InitSwagDataTable(sdt);
                        }
                        sdt.FilterCommand.Execute(null);
                    }
                }
            }
            SwagWindow.GlobalIsBusy = false;
        }
        #endregion Events
    }

    #region SwagDataHelper
    public static class SwagDataHelper
    {
        public static void LoadFiles(this SwagDataSet swagDataSet, IEnumerable<String> files, IEnumerable<KeyValuePairViewModel<String, ParseViewModel>> parseMappers)
        {
            foreach (String file in files)
            {
                SwagLogger.LogStart(swagDataSet, "Load {file}", file);
                SwagData child = SwagDataHelper.FromFile(file, parseMappers);
                SwagLogger.LogEnd(swagDataSet, "Load {file}", file);

                if (child != null)
                {
                    swagDataSet.Children.Add(child);
                }
                else
                {
                    SwagLogger.Log("Load {file} did not yield data (unsupported extenstion).", file);
                }
            }
        }

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
                return new SwagDataSet(dataSetContext.ToDataSet(file)) { Display = filename };
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
