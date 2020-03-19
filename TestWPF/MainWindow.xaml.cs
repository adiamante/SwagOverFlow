﻿using SwagOverflowWPF.Services;
using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Data;
using System;
using System.Reflection;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Extensions.DependencyInjection;
using SwagOverflowWPF.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace TestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SwagWindow
    {
        private static IServiceProvider serviceProvider;
        SwagDataTable _sdtSource = null, _sdtDest = null;
        String descriptionField = "FullDescription";

        #region Source
        public SwagDataTable Source
        {
            get { return _sdtSource; }
            set { SetValue(ref _sdtSource, value); }
        }
        #endregion Source

        #region Dest
        public SwagDataTable Dest
        {
            get { return _sdtDest; }
            set { SetValue(ref _sdtDest, value); }
        }
        #endregion Dest

        public MainWindow()
        {
            InitializeComponent();

            ConfigureServices();
            SwagContext context = serviceProvider.GetService<SwagContext>();
            context.Database.EnsureCreated();
            SwagWindowSettingService settingService = serviceProvider.GetService<SwagWindowSettingService>();
            String settingGoupName = $"{Assembly.GetEntryAssembly().GetName().Name}_Settings";
            this.Settings = settingService.GetWindowSettingGroupByName(settingGoupName);

            SwagDataTableService tableService = serviceProvider.GetService<SwagDataTableService>();
            String sourceTableGroupName = $"{Assembly.GetEntryAssembly().GetName().Name}_Table_Source";
            Source = tableService.GetDataTableByName(sourceTableGroupName);
            String destTableGroupName = $"{Assembly.GetEntryAssembly().GetName().Name}_Table_Dest";
            Dest = tableService.GetDataTableByName(destTableGroupName);

            BindSourceGrid();
            BindDestGrid();
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<SwagContext>(options => SwagContext.SetSqliteOptions(options));
            //services.AddDbContext<SwagContext>(options => SwagContext.SetSqlServerOptions(options));
            services.AddTransient<SwagWindowSettingService>();
            services.AddTransient<SwagDataTableService>();
            serviceProvider = services.BuildServiceProvider();
        }

        private void dg_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void dg_Drop(object sender, System.Windows.DragEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            foreach (String file in (String [])e.Data.GetData(DataFormats.FileDrop))
            {
                CsvFileToDataTable csvFileToDataTable = new CsvFileToDataTable();
                DataTable dt = csvFileToDataTable.FileToDataTable(file);
                dataGrid.ItemsSource = dt.DefaultView;

                if (dataGrid == dgSource)
                {
                    Source.DataTable = dt;
                    BindSourceGrid();
                }

                if (dataGrid == dgDest)
                {
                    Dest.DataTable = dt;
                    BindDestGrid();
                }
            }
        }

        private void BindSourceGrid()
        {
            if (Source.DataTable != null && Source.DataTable.Columns.Count > 0)
            {
                #region Columns
                foreach (DataColumn dc in Source.DataTable.Columns)
                {
                    if (dc.ColumnName != "DestID" && dc.ColumnName != "Mapping" && dc.ColumnName != "Source, Dest")
                    {
                        DataGridTextColumn dgc = new DataGridTextColumn();
                        dc.ReadOnly = false;
                        dgc.Header = dc.ColumnName;
                        dgc.Binding = new Binding(dc.ColumnName);
                        dgc.IsReadOnly = false;
                        dgSource.Columns.Add(dgc);
                    }
                }
                #endregion Columns

                #region DestID
                if (!Source.DataTable.Columns.Contains("DestID"))
                {
                    Source.DataTable.Columns.Add("DestID");
                }

                DataGridTextColumn dgcDestID = new DataGridTextColumn();
                dgcDestID.Header = "DestID";
                dgcDestID.Binding = new Binding("[DestID]") { TargetNullValue = "----------" };
                dgSource.Columns.Add(dgcDestID);
                #endregion DestID

                #region Mapping
                //if (!Source.DataTable.Columns.Contains("Mapping"))
                //{
                //    Source.DataTable.Columns.Add("Mapping");
                //}

                DataGridTemplateColumn dgtc = new DataGridTemplateColumn();
                dgtc.Header = "Mapping";
                DataTemplate template = new DataTemplate();
                DataTemplate itemTemplate = new DataTemplate();

                //https://stackoverflow.com/questions/9385489/why-errors-when-filters-datatable-with-collectionview
                FrameworkElementFactory comboBoxFactory = new FrameworkElementFactory(typeof(AutoFilteredComboBox));
                //comboBoxFactory.SetValue(AutoFilteredComboBox.TextProperty, "");
                comboBoxFactory.SetValue(AutoFilteredComboBox.DisplayMemberPathProperty, descriptionField);
                comboBoxFactory.SetValue(AutoFilteredComboBox.IsEditableProperty, true);
                comboBoxFactory.SetBinding(AutoFilteredComboBox.ItemsSourceProperty, new Binding("Dest.DataTable.DefaultView") { RelativeSource = new RelativeSource() { AncestorType = typeof(MainWindow) }});
                //comboBoxFactory.SetValue(AutoFilteredComboBox.ItemsSourceProperty, Dest.DataTable.DefaultView);

                //comboBoxFactory.SetValue(AutoFilteredComboBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Left);
                //comboBoxFactory.SetValue(AutoFilteredComboBox.VerticalAlignmentProperty, VerticalAlignment.Center);
                comboBoxFactory.SetValue(AutoFilteredComboBox.SelectedValuePathProperty, "ItemID");
                //comboBoxFactory.SetBinding(AutoFilteredComboBox.TextProperty, new Binding("Mapping"));
                comboBoxFactory.SetBinding(AutoFilteredComboBox.SelectedValueProperty, new Binding("DestID"));

                comboBoxFactory.AddHandler(AutoFilteredComboBox.SelectionChangedEvent, new SelectionChangedEventHandler((o, e) =>
                {
                    ComboBox comboBox = (ComboBox)o;

                    if (comboBox.DataContext != null && comboBox.DataContext is DataRowView && e.AddedItems.Count > 0 && e.AddedItems[0] is DataRowView)
                    {
                        DataRowView drvNew = (DataRowView)e.AddedItems[0];
                        DataRowView drv = (DataRowView)comboBox.DataContext;
                        drv["DestID"] = drvNew["ItemID"];
                    }
                    else if (comboBox.DataContext != null && comboBox.DataContext is DataRowView && e.AddedItems.Count > 0)
                    {
                        DataRowView drv = (DataRowView)comboBox.DataContext;
                        drv["DestID"] = null;
                    }
                    //else if (e.AddedItems.Count == 0 && e.RemovedItems.Count > 0)
                    //{
                    //    e.Handled = true;
                    //}
                }));
                template.VisualTree = comboBoxFactory;

                dgtc.CellTemplate = template;
                dgSource.Columns.Add(dgtc);
                #endregion Mapping

                #region (Source, Dest)
                if (!Source.DataTable.Columns.Contains("Source, Dest"))
                {
                    DataColumn dc = new DataColumn();
                    dc.ColumnName = "Source, Dest";
                    dc.Expression = "'(' + ItemID + ', ' + DestID + ')'";
                    Source.DataTable.Columns.Add(dc);
                }

                DataGridTextColumn dgcSourceDest = new DataGridTextColumn();
                dgcSourceDest.Header = "(Source, Dest)";
                dgcSourceDest.Binding = new Binding("Source, Dest");
                dgcSourceDest.IsReadOnly = true;
                dgSource.Columns.Add(dgcSourceDest);
                #endregion (Source, Dest)

                Source.Init();
            }
        }

        private void BindDestGrid()
        {
            if (Dest.DataTable != null)
            {
                foreach (DataColumn dc in Dest.DataTable.Columns)
                {
                    DataGridTextColumn dgc = new DataGridTextColumn();
                    dc.ReadOnly = false;
                    dgc.Header = dc.ColumnName;
                    dgc.Binding = new Binding(dc.ColumnName);
                    dgc.IsReadOnly = false;
                    dgDest.Columns.Add(dgc);
                }

                Dest.Init();
            }
        }

        private void MapSimilar_Button_Click(object sender, RoutedEventArgs e)
        {
            for (int r = 0; r < Source.DataTable.Rows.Count; r++)
            {
                DataRow drSource = Source.DataTable.Rows[r];
                String description = drSource[descriptionField].ToString();
                DataRow[] drMatches = Dest.DataTable.Select($"[{descriptionField}] = '{description.Replace("'", "''")}'");

                if (drMatches.Length > 0)
                {
                    DataRow drDest = drMatches[0];
                    drSource["DestID"] = drDest["ItemID"];
                }
            }
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
            CsvStringToDataTable csvStringToDataTable = new CsvStringToDataTable();
            ToDataTableContext context = new ToDataTableContext();
            context.FieldDelim = '\t';
            DataTable dt = csvStringToDataTable.StringToDataTable(data, context);
            Source.DataTable = dt;
            BindSourceGrid();
        }

        private void Load_Dest_From_Clipboard_Button_Click(object sender, RoutedEventArgs e)
        {
            String data = Clipboard.GetText();
            CsvStringToDataTable csvStringToDataTable = new CsvStringToDataTable();
            ToDataTableContext context = new ToDataTableContext();
            context.FieldDelim = '\t';
            DataTable dt = csvStringToDataTable.StringToDataTable(data, context);
            Dest.DataTable = dt;
            BindDestGrid();
        }
    }
}
