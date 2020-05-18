using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using SwagOverFlow.Data;
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
                    typeof(SwagDataSetWPF),
                    typeof(SwagDataControl));

        public SwagDataSetWPF SwagDataSet
        {
            get { return (SwagDataSetWPF)GetValue(SwagDataSetProperty); }
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
        public SwagItemGroupWPF<KeyValuePairViewModel<String, ParseViewModel>> ParseMapper
        {
            get
            {
                return SwagWindow.GlobalSettings["SwagData"]["ParseMapper"].GetValue<SwagItemGroupWPF<KeyValuePairViewModel<String, ParseViewModel>>>();
            }
        }
        #endregion ParseMapper
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

            if (!((SwagSettingGroup)SwagWindow.GlobalSettings).ContainsKey("SwagData"))
            {
                SwagSettingWPFGroup swagDataSetting = new SwagSettingWPFGroup() { Icon = PackIconCustomKind.Dataset };
                SwagWindow.GlobalSettings["SwagData"] = swagDataSetting;
                swagDataSetting.IconString = swagDataSetting.IconString;
                swagDataSetting.IconTypeString = swagDataSetting.IconTypeString;
                ((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
            }

            if (!((SwagSettingGroup)SwagWindow.GlobalSettings["SwagData"]).ContainsKey("ParseMapper"))
            {
                SwagSetting<SwagItemGroupWPF<KeyValuePairViewModel<String, ParseViewModel>>> ssParseMapper =
                    new SwagSetting<SwagItemGroupWPF<KeyValuePairViewModel<string, ParseViewModel>>>()
                    {
                        Icon = PackIconCustomKind.ArrowMultipleSweepRight,
                        Value = new SwagItemGroupWPF<KeyValuePairViewModel<string, ParseViewModel>>()
                    };

                ssParseMapper.IconString = ssParseMapper.IconString;
                ssParseMapper.IconTypeString = ssParseMapper.IconTypeString;
                ssParseMapper.ValueTypeString = ssParseMapper.ValueTypeString;
                ssParseMapper.ObjValue = ssParseMapper.ObjValue;
                SwagWindow.GlobalSettings["SwagData"]["ParseMapper"] = ssParseMapper;
                ((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
            }

            ParseMapper.SwagItemChanged += (s, e) =>
            {
               SwagWindow.GlobalSettings.OnSwagItemChanged(SwagWindow.GlobalSettings["SwagData"]["ParseMapper"], e.PropertyChangedArgs);
            };
        }

        public void InitDataSet()
        {
            if (SwagDataSet != null)
            {
                SwagDataSet.Children.CollectionChanged += Children_CollectionChanged;
            }
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagData swagData in e.NewItems)
                {
                    switch (swagData)
                    {
                        case SwagDataSet swagDataSet:
                            swagDataSet.Children.CollectionChanged += Children_CollectionChanged;
                            break;
                        case SwagDataTable swagDataTable:
                            SwagWindow.CommandManager.Attach(swagDataTable);
                            break;
                    }
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
                String[] formats = e.Data.GetFormats();

                foreach (var format in formats)
                {
                    // Shell items are passed using the "Shell IDList Array" format. 
                    if (format == "Shell IDList Array")
                    {
                        // Retrieve the ShellObjects from the data object
                        ShellObjectCollection shellObjects = ShellObjectCollection.FromDataObject(
                            (System.Runtime.InteropServices.ComTypes.IDataObject)e.Data);

                        foreach (ShellObject shellObject in shellObjects)
                        {
                            allFiles.Add(shellObject.ParsingName);
                        }
                    }
                }
            }

            if (allFiles.Count > 0)
            {
                List<KeyValuePairViewModel<String, ParseViewModel>> parseMappers = ParseMapper.Children.Select(pm => pm.Value).ToList();
                SwagDataSet.LoadFiles(allFiles, parseMappers);
            }
        }
        #endregion Drop

        #region Search
        private void Search_OnSearch(object sender, RoutedEventArgs e)
        {
            SearchTextBox searchTextBox = (SearchTextBox)sender;
            SwagData swagData = (SwagData)((SwagTabItem)searchTextBox.DataContext).ViewModel;
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
                    View((SwagDataSetWPF)setResult.SwagData);
                    break;
                case SwagDataTableResultGroup tableResult:
                    View((SwagDataTableWPF)tableResult.SwagData);
                    break;
                case SwagDataColumnResultGroup columnResult:
                    View((SwagDataColumn)columnResult.SwagData);
                    break;
                case SwagDataRowResult rowResult:
                    View(rowResult);
                    break;
            }
        }

        private void View(SwagDataSetWPF swagDataSet)
        {
            if (swagDataSet.Parent != null && swagDataSet.Parent is SwagDataSetWPF)
            {
                SwagDataSetWPF parent = (SwagDataSetWPF)swagDataSet.Parent;
                parent.SelectedChild = swagDataSet;
                View(parent);
            }
        }

        private void View(SwagDataTableWPF swagDataTable)
        {
            if (swagDataTable.Parent != null && swagDataTable.Parent is SwagDataSetWPF)
            {
                SwagDataSetWPF parent = (SwagDataSetWPF)swagDataTable.Parent;
                parent.SelectedChild = swagDataTable;
                View(parent);
            }
        }

        private void View(SwagDataColumn swagDataColumn)
        {
            if (swagDataColumn.Parent != null && swagDataColumn.Parent is SwagDataTableWPF)
            {
                SwagDataTableWPF parent = (SwagDataTableWPF)swagDataColumn.Parent;
                parent.SelectedColumn = swagDataColumn;
                View(parent);
            }
        }

        private void View(SwagDataRowResult rowResult)
        {
            SwagDataColumn swagDataColumn = (SwagDataColumn)rowResult.Parent.SwagData;
            ((SwagDataTableWPF)swagDataColumn.SwagDataTable).SelectedRow = rowResult;
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
            if (swagData.Parent != null && swagData.Parent is SwagDataSetWPF)
            {
                SwagDataSetWPF swagDataSet = (SwagDataSetWPF)swagData.Parent;
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
            DataSet ds = swagDataSet.GetDataSet();
            ParseStrategy parseStrategy = (ParseStrategy)fe.Tag;
            
            switch (parseStrategy)
            {
                case ParseStrategy.Sqlite:
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    sfd.FileName = Path.ChangeExtension(swagDataSet.Display, null);
                    sfd.Filter = "SQLite files (*.db;*.sqlite)|*.db;*.sqlite";
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (sfd.ShowDialog() ?? false)
                        {
                            DataSetSqliteFileConverter dsConverter = new DataSetSqliteFileConverter();
                            dsConverter.FromDataSet(null, ds, sfd.FileName);
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

    }
}
