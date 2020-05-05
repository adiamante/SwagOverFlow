using Microsoft.WindowsAPICodePack.Shell;
using SwagOverFlow.Utils;
using SwagOverflowWPF.UI;
using SwagOverflowWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SwagOverflowWPF.Controls
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
                    typeof(SwagDataControl));

        public SwagDataSet SwagDataSet
        {
            get { return (SwagDataSet)GetValue(SwagDataSetProperty); }
            set
            {
                SetValue(SwagDataSetProperty, value);
                OnPropertyChanged();
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
        public SwagItemGroup<KeyValuePairViewModel<String, ParseViewModel>> ParseMapper
        {
            get
            {
                return SwagWindow.GlobalSettings["SwagData"]["ParseMapper"].GetValue<SwagItemGroup<KeyValuePairViewModel<String, ParseViewModel>>>();
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
        }

        public void Initialize()
        {
            if (!((SwagSettingGroup)SwagWindow.GlobalSettings).ContainsKey("SwagData"))
            {
                SwagSettingGroup swagDataSetting = new SwagSettingGroup() { Icon = PackIconCustomKind.Dataset };
                SwagWindow.GlobalSettings["SwagData"] = swagDataSetting;
                swagDataSetting.IconString = swagDataSetting.IconString;
                swagDataSetting.IconTypeString = swagDataSetting.IconTypeString;
                ((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
            }

            if (!((SwagSettingGroup)SwagWindow.GlobalSettings["SwagData"]).ContainsKey("ParseMapper"))
            {
                SwagSetting<SwagItemGroup<KeyValuePairViewModel<String, ParseViewModel>>> ssParseMapper =
                    new SwagSetting<SwagItemGroup<KeyValuePairViewModel<string, ParseViewModel>>>()
                    {
                        Icon = PackIconCustomKind.ArrowMultipleSweepRight,
                        Value = new SwagItemGroup<KeyValuePairViewModel<string, ParseViewModel>>()
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
        #endregion SwagDataHeader ContextMenu
    }
}
