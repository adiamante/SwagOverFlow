using SwagOverFlow.WPF.Services;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.Data;
using System;
using System.Reflection;
using System.Windows;
using System.Data;
using System.Windows.Data;
using Microsoft.Extensions.DependencyInjection;
using SwagOverFlow.WPF.ViewModels;
using System.Text;
using System.Collections;
using SwagOverFlow.Data;
using Microsoft.Extensions.Logging;
using SwagOverFlow.ViewModels;

namespace TestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SwagWindow
    {
        SwagDataTableWPF _sdtSource = null, _sdtDest = null;
        String descriptionField = "FullDescription";
        IEnumerable _comboboxSource = null;
        String _combobxText = "";

        #region Source
        public SwagDataTableWPF Source
        {
            get { return _sdtSource; }
            set { SetValue(ref _sdtSource, value); }
        }
        #endregion Source

        #region Dest
        public SwagDataTableWPF Dest
        {
            get { return _sdtDest; }
            set { SetValue(ref _sdtDest, value); }
        }
        #endregion Dest

        #region ComboBoxText
        public String ComboBoxText
        {
            get { return _combobxText; }
            set { SetValue(ref _combobxText, value); }
        }
        #endregion ComboBoxText

        #region ComboBoxSource
        public IEnumerable ComboBoxSource
        {
            get { return _comboboxSource; }
            set { SetValue(ref _comboboxSource, value); }
        }
        #endregion ComboBoxSource

        public MainWindow()
        {
            InitializeComponent();

            String sourceTableGroupName = $"{Assembly.GetEntryAssembly().GetName().Name}_Table_Source";
            Source = SwagWPFServices.DataTableService.GetDataTableByName(sourceTableGroupName);
            String destTableGroupName = $"{Assembly.GetEntryAssembly().GetName().Name}_Table_Dest";
            //Dest = SwagWPFServices.DataTableService.GetDataTableByName(destTableGroupName);

            BindSourceGrid();

            //InitCombobox();
            EventManager.RegisterClassHandler(typeof(SwagComboBox), SwagComboBox.ValueChangedEvent, new RoutedEventHandler(SwagComboBox_ValueChanged));
        }

        private void InitCombobox()
        {
            DataTableCsvFileConverter csvFileToDataTable = new DataTableCsvFileConverter();
            DataTable dt = csvFileToDataTable.ToDataTable(new DataTableConvertParams(), @"C:\Users\Desktop\Desktop\445.csv");
            ComboBoxSource = dt.DefaultView;
            //scbxTest.ItemsSource = dt.DefaultView;
        }

        private void dg_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void dg_Source_Drop(object sender, System.Windows.DragEventArgs e)
        {
            foreach (String file in (String [])e.Data.GetData(DataFormats.FileDrop))
            {
                DataTableCsvFileConverter csvFileToDataTable = new DataTableCsvFileConverter();
                DataTable dt = csvFileToDataTable.ToDataTable(new DataTableConvertParams(), file);
                Source.DataTable = dt;
                BindSourceGrid();
            }
        }

        private void dg_Dest_Drop(object sender, System.Windows.DragEventArgs e)
        {
            foreach (String file in (String[])e.Data.GetData(DataFormats.FileDrop))
            {
                DataTableCsvFileConverter csvFileToDataTable = new DataTableCsvFileConverter();
                DataTable dt = csvFileToDataTable.ToDataTable(new DataTableConvertParams(), file);
                Dest.DataTable = dt;
            }
        }

        private void BindSourceGrid()
        {
            if (Source.DataTable != null && Source.DataTable.Columns.Count > 0)
            {
                #region DestID
                if (!Source.Columns.ContainsKey("DestID"))
                {
                    SwagDataColumnWPF sdc = new SwagDataColumnWPF()
                    {
                        ColumnName = "DestID",
                        DataType = typeof(string),
                        //Saving a binding is currently not working
                        Binding = new Binding("[DestID]") { TargetNullValue = "----------", FallbackValue = "----------" }
                    };

                    Source.Columns.Add("DestID", sdc);
                }
                #endregion DestID

                #region Mapping
                if (!Source.DataTable.Columns.Contains("Mapping"))
                {
                    String mappingTemplate = @"<DataTemplate 
                                xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                                xmlns:local='clr-namespace:TestWPF;assembly=TestWPF'
                                xmlns:swag='clr-namespace:SwagOverFlow.WPF.Controls;assembly=SwagOverFlow.WPF'>
                                <swag:SwagComboBox 
                                    DisplayMemberProperty ='{{descriptionField}}' 
                                    ItemsSource='{Binding Dest.DataTable.DefaultView, RelativeSource={RelativeSource AncestorType={x:Type local:MainWindow}}}'
                                    ValueMemberProperty='ItemID'
                                    Value='{Binding DestID}'
                                    Text='{Binding Mapping}'/>
                            </DataTemplate>";
                    mappingTemplate = mappingTemplate.Replace("{{descriptionField}}", descriptionField);

                    SwagDataColumnWPF sdc = new SwagDataColumnWPF()
                    {
                        ColumnName = "Mapping",
                        DataType = typeof(string),
                        DataTemplate = mappingTemplate
                    };

                    Source.Columns.Add("Mapping", sdc);
                }
                #endregion Mapping

                #region (Source, Dest)
                //Need to load in proper order because of dependence on other Columns
                //if (!Source.Columns.ContainsKey("Source, Dest"))
                //{
                //    SwagDataColumnWPF sdc = new SwagDataColumnWPF()
                //    {
                //        ColumnName = "Source, Dest",
                //        DataType = typeof(string),
                //        Expression = "'(' + ItemID + ', ' + DestID + ')'"
                //    };

                //    Source.Columns.Add("Source, Dest", sdc);
                //}
                #endregion (Source, Dest)

                Source.Save();
            }
        }

        private async void MapSimilar_Button_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = Source.DataTable;

            await this.RunInBackground(() =>
            {
                Source.SetContext(null);
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    DataRow drSource = Source.DataTable.Rows[r];
                    String description = drSource[descriptionField].ToString();
                    DataRow[] drMatches = Dest.DataTable.Select($"[{descriptionField}] = '{description.Replace("'", "''")}'");

                    if (drMatches.Length > 0)
                    {
                        DataRow drDest = drMatches[0];
                        drSource["DestID"] = drDest["ItemID"];
                        drSource["Mapping"] = drDest[descriptionField];
                    }
                }
            });

            Source.SetContext(SwagWPFServices.Context);
            Source.DataTable = dt;
        }

        private void Export_Mapppings_Button_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            for (int r = 0; r < Source.DataTable.Rows.Count; r++)
            {
                DataRow drSource = Source.DataTable.Rows[r];
                String mapping = drSource["Source, Dest"].ToString();
                String destID = drSource["DestID"].ToString();

                if (mapping != "" && destID != "")
                {
                    sb.AppendFormat("{0},\n", mapping);
                }
            }

            sb.Length--;
            sb.Length--;
            Clipboard.SetText(sb.ToString());

            MessageBox.Show("Set to clipboard");
        }

        private void Load_Source_From_Clipboard_Button_Click(object sender, RoutedEventArgs e)
        {
            String data = Clipboard.GetText();
            DataTableCsvStringConverter csvStringToDataTable = new DataTableCsvStringConverter();
            DataTableConvertParams context = new DataTableConvertParams();
            context.FieldDelim = '\t';
            DataTable dt = csvStringToDataTable.ToDataTable(context, data);
            Source.DataTable = dt;
            BindSourceGrid();
        }

        private void Load_Dest_From_Clipboard_Button_Click(object sender, RoutedEventArgs e)
        {
            String data = Clipboard.GetText();
            DataTableCsvStringConverter csvStringToDataTable = new DataTableCsvStringConverter();
            DataTableConvertParams context = new DataTableConvertParams();
            context.FieldDelim = '\t';
            DataTable dt = csvStringToDataTable.ToDataTable(context, data);
            Dest.DataTable = dt;
        }

        private void SwagComboBox_ValueChanged(object sender, RoutedEventArgs e)
        {
            SwagComboBox scbx = (SwagComboBox)sender;

            if (scbx.DataContext is DataRowView)
            {
                DataRowView drv = (DataRowView)scbx.DataContext;
                //This is needed notify these properties changed so it can be saved
                drv["DestID"] = scbx.Value;
                drv["Mapping"] = scbx.Text;
            }
        }

        private void SearchTextBox_Search(object sender, RoutedEventArgs e)
        {
            SearchTextBox s = (SearchTextBox)sender;
            MessageBox.Show($"Searching: {s.Text}");
        }

        private void SwagWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SwagWindow.CommandManager.Attach(Source);
        }

    }
}
