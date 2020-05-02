using Microsoft.WindowsAPICodePack.Shell;
using SwagOverFlow.Utils;
using SwagOverflowWPF.UI;
using SwagOverflowWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SwagOverflowWPF.Controls
{
    /// <summary>
    /// Interaction logic for SwagDataControl.xaml
    /// </summary>
    public partial class SwagDataControl : SwagControlBase
    {
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

        public SwagDataControl()
        {
            InitializeComponent();
        }

        private void ControlInstance_Loaded(object sender, RoutedEventArgs e)
        {
            //SwagDataSet set = new SwagDataSet();
            //SwagDataTable t1 = new SwagDataTable() { Display = "T1X" };
            //t1.InitSettings();
            //t1.InitTabs();
            //SwagDataTable t2 = new SwagDataTable() { Display = "T2" };
            //t2.InitSettings();
            //t2.InitTabs();
            //set.Children.Add(t1);
            //set.Children.Add(t2);


            //SwagDataSet s1 = new SwagDataSet() { Display = "S1" };
            //SwagDataTable s1t1 = new SwagDataTable() { Display = "S1T1" };
            //s1t1.InitSettings();
            //s1t1.InitTabs();
            //SwagDataTable s1t2 = new SwagDataTable() { Display = "S1T2" };
            //s1t2.InitSettings();
            //s1t2.InitTabs();
            //s1.Children.Add(s1t1);
            //s1.Children.Add(s1t2);

            //set.Children.Add(s1);

            //SwagDataSet = set;
            //SwagDataSet = new SwagDataSet();
        }

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
                SwagDataSet.LoadFiles(allFiles);
            }
        }

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
    }
}
