using ControlzEx.Theming;
using Dreamporter.Core;
using Dreamporter.WPF.Services;
using MahApps.Metro.IconPacks;
using SwagOverFlow.Iterator;
using SwagOverFlow.Logger;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.UI;
using SwagOverFlow.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace Dreamporter.WPF.Controls
{
    /// <summary>
    /// Interaction logic for DreamporterControl.xaml
    /// </summary>
    public partial class DreamporterControl : SwagControlBase
    {
        #region Integrations
        public static DependencyProperty IntegrationsProperty =
            DependencyProperty.Register(
                "Integrations",
                typeof(ICollection<Integration>),
                typeof(DreamporterControl), 
                new PropertyMetadata(null, IntegrationsPropertyChanged));

        private static void IntegrationsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DreamporterControl dc = (DreamporterControl)d;
            ICollection<Integration> integrations = (ICollection<Integration>)e.NewValue;

            if (integrations != null && dc.SelectedIntegrationName != null)
            {
                Integration integration = integrations.FirstOrDefault(i => i.Name == dc.SelectedIntegrationName);

                if (integration != null)
                {
                    dc.SelectedIntegration = integration;
                }
            }
        }

        public ICollection<Integration> Integrations
        {
            get { return (ICollection<Integration>)GetValue(IntegrationsProperty); }
            set { SetValue(IntegrationsProperty, value); }
        }
        #endregion Integrations

        #region SelectedIntegration
        public static DependencyProperty SelectedIntegrationProperty =
            DependencyProperty.Register(
                "SelectedIntegration",
                typeof(Integration),
                typeof(DreamporterControl),
                new FrameworkPropertyMetadata(null, SelectedIntegration_PropertyChanged));

        private async static void SelectedIntegration_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DreamporterControl dc = (DreamporterControl)d;

            //Delay is needed because of shared binding to SelectedBuild (Integration.Build and Integration.TestBuild)
            //When switching between integrations SelectedBuild becomes Integration.Build even when we set it to Integration.TestBuild
            await Task.Delay(100);      
            if (e.NewValue != null && e.NewValue is Integration integration)
            {
                if (dc.InTestMode)
                {
                    dc.RefreshSelectedBuild(integration.TestBuild);
                    foreach (TestContext testContext in integration.TestContexts)
                    {
                        if (testContext.IsSelected)
                        {
                            integration.SelectedTestContext = testContext;
                        }
                    }
                }
                else
                {
                    dc.RefreshSelectedBuild(integration.Build);
                }
            }
        }

        public Integration SelectedIntegration
        {
            get { return (Integration)GetValue(SelectedIntegrationProperty); }
            set 
            { 
                SetValue(SelectedIntegrationProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion SelectedIntegration

        #region SelectedIntegrationName
        public String SelectedIntegrationName
        {
            get { return (String)SwagWindow.GlobalSettings["Dreamporter"]["SelectedIntegration"].GetValue<String>(); }
            set { SwagWindow.GlobalSettings["Dreamporter"]["SelectedIntegration"].SetValue<String>(value); }
        }
        #endregion Schemas

        #region IsBusy
        public static DependencyProperty IsBusyProperty =
            DependencyProperty.Register(
                "IsBusy",
                typeof(bool),
                typeof(DreamporterControl));

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set 
            {
                SetValue(IsBusyProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion IsBusy

        #region InTestMode
        public static DependencyProperty InTestModeProperty =
            DependencyProperty.Register(
                "InTestMode",
                typeof(bool),
                typeof(DreamporterControl),
                new FrameworkPropertyMetadata(false, InTestMode_PropertyChanged));

        private static void InTestMode_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DreamporterControl dc = (DreamporterControl)d;
            if (e.NewValue != null)
            {
                if (dc.InTestMode)
                {
                    dc.RefreshSelectedBuild(dc.SelectedIntegration.TestBuild);
                    foreach (TestContext testContext in dc.SelectedIntegration.TestContexts)
                    {
                        if (testContext.IsSelected)
                        {
                            dc.SelectedIntegration.SelectedTestContext = testContext;
                        }
                    }
                }
                else
                {
                    dc.RefreshSelectedBuild(dc.SelectedIntegration.Build);
                }
            }
        }

        public void RefreshSelectedBuild(GroupBuild rootBuild)
        {
            Build selectedBuild = null;
            SwagItemPreOrderIterator<Build> bldIterator = new SwagItemPreOrderIterator<Build>(rootBuild);
            for (Build bld = bldIterator.First(); !bldIterator.IsDone; bld = bldIterator.Next())
            {
                if (bld.IsSelected)
                {
                    selectedBuild = bld;
                }
            }

            if (selectedBuild != null)
            {
                SelectedIntegration.SelectedBuild = selectedBuild;
            }
        }

        public bool InTestMode
        {
            get { return (bool)GetValue(InTestModeProperty); }
            set
            {
                SetValue(InTestModeProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion InTestMode

        #region Initialization
        public DreamporterControl()
        {
            InitializeComponent();
            InitSettings();
            InitSelectedOptions();
        }

        private void InitSettings()
        {
            SwagWindow.GlobalSettings.TryAddChildSetting("Dreamporter", new SwagSettingGroup() { Icon = PackIconMaterialKind.Cloud });
            SwagWindow.GlobalSettings["Dreamporter"].TryAddChildSetting("SelectedIntegration", new SwagSettingString() { Icon = PackIconMaterialKind.Bridge });
        }

        private void InitSelectedOptions()
        {
            if (SelectedIntegration != null && SelectedIntegration.SelectedOptions == null)
            {
                SwagItemPreOrderIterator<OptionsNode> iterator = new SwagItemPreOrderIterator<OptionsNode>(SelectedIntegration.OptionsTree);
                for (OptionsNode opt = iterator.First(); !iterator.IsDone; opt = iterator.Next())
                {
                    if (opt.IsSelected)
                    {
                        SelectedIntegration.SelectedOptions = opt;
                        break;
                    }
                }
            }
        }
        #endregion Initialization

        #region Events
        private void cbxIntegrations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedIntegration != null && SelectedIntegrationName != SelectedIntegration.Name)
            {
                SelectedIntegrationName = SelectedIntegration.Name;
            }
            InitSelectedOptions();
        }

        private void AddIntegration_Click(object sender, RoutedEventArgs e)
        {
            Integration newIntegration = new Integration() { Name = $"Integration {Integrations.Count + 1}" };
            DreamporterWPFContainer.IntegrationDataRepository.Insert(newIntegration);
            Integrations.Add(newIntegration);
            SelectedIntegration = newIntegration;
        }

        private void IntegrationSave_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                if (SelectedIntegration.IntegrationId != 0)
                {
                    DreamporterWPFContainer.IntegrationDataRepository.Update(SelectedIntegration);
                }
                DreamporterWPFContainer.Context.SaveChanges();
                SwagLogger.Log("Integration {Integration} Saved {timeStamp}", SelectedIntegration.Name, DateTime.Now);
            }
        }

        private void BuildControl_Save(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                if (SelectedIntegration.IntegrationId != 0)
                {
                    SelectedIntegration.Build = SelectedIntegration.Build;
                    SwagItemPreOrderIterator<Build> iterator = new SwagItemPreOrderIterator<Build>(SelectedIntegration.Build);
                    for (Build b = iterator.First(); !iterator.IsDone; b = iterator.Next())
                    {
                        if (b.BuildId != 0)
                        {
                            DreamporterWPFContainer.BuildRepository.Update(b);
                        }
                        else
                        {
                            DreamporterWPFContainer.BuildRepository.Insert(b);
                        }
                    }
                }
                DreamporterWPFContainer.Context.SaveChanges();
                SwagLogger.Log("Build Saved {timeStamp}", DateTime.Now);
            }
        }

        private void TestBuildControl_Save(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                if (SelectedIntegration.IntegrationId != 0)
                {
                    SelectedIntegration.TestBuild = SelectedIntegration.TestBuild;
                    SwagItemPreOrderIterator<Build> iterator = new SwagItemPreOrderIterator<Build>(SelectedIntegration.TestBuild);
                    for (Build b = iterator.First(); !iterator.IsDone; b = iterator.Next())
                    {
                        if (b.BuildId != 0)
                        {
                            DreamporterWPFContainer.BuildRepository.Update(b);
                        }
                        else
                        {
                            DreamporterWPFContainer.BuildRepository.Insert(b);
                        }
                    }
                }
                DreamporterWPFContainer.Context.SaveChanges();
                SwagLogger.Log("Test Build Saved {timeStamp}", DateTime.Now);
            }
        }

        private void InstructionControl_Save(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                if (SelectedIntegration.IntegrationId != 0)
                {
                    SelectedIntegration.InstructionTemplates = SelectedIntegration.InstructionTemplates;
                    DreamporterWPFContainer.IntegrationDataRepository.Update(SelectedIntegration);
                }
                DreamporterWPFContainer.Context.SaveChanges();
                SwagLogger.Log("InstructionTemplates Saved {timeStamp}", DateTime.Now);
            }
        }

        private void DefaultOptions_Save(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                if (SelectedIntegration.IntegrationId != 0)
                {
                    SelectedIntegration.DefaultOptions = SelectedIntegration.DefaultOptions;
                    DreamporterWPFContainer.IntegrationDataRepository.Update(SelectedIntegration);
                }
                DreamporterWPFContainer.Context.SaveChanges();
                SwagLogger.Log("Default Options Saved {timeStamp}", DateTime.Now);
            }
        }

        private void OptionsTreeControl_Save(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                if (SelectedIntegration.IntegrationId != 0)
                {
                    SelectedIntegration.OptionsTree = SelectedIntegration.OptionsTree;
                    DreamporterWPFContainer.IntegrationDataRepository.Update(SelectedIntegration);
                }
                DreamporterWPFContainer.Context.SaveChanges();
                SwagLogger.Log("Options Tree Saved {timeStamp}", DateTime.Now);
            }
        }

        private void SchemasControl_Save(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                if (SelectedIntegration.IntegrationId != 0)
                {
                    SelectedIntegration.InitialSchemas = SelectedIntegration.InitialSchemas;
                    DreamporterWPFContainer.IntegrationDataRepository.Update(SelectedIntegration);
                }
                DreamporterWPFContainer.Context.SaveChanges();
                SwagLogger.Log("InitialSchemas Saved {timeStamp}", DateTime.Now);
            }
        }

        private void DataContextsControl_Save(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                if (SelectedIntegration.IntegrationId != 0)
                {
                    SelectedIntegration.DataContexts = SelectedIntegration.DataContexts;
                    DreamporterWPFContainer.IntegrationDataRepository.Update(SelectedIntegration);
                }
                DreamporterWPFContainer.Context.SaveChanges();
                SwagLogger.Log("DataContexts Saved {timeStamp}", DateTime.Now);
            }
        }

        private void InstructionTemplatesApply_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                //Iterate through all templates
                SwagItemPreOrderIterator<Instruction> insTemplateIterator = new SwagItemPreOrderIterator<Instruction>(SelectedIntegration.InstructionTemplates);
                for (Instruction insTemplate = insTemplateIterator.First(); !insTemplateIterator.IsDone; insTemplate = insTemplateIterator.Next())
                {
                    //Iterate through all builds
                    SwagItemPreOrderIterator<Build> bldIterator = new SwagItemPreOrderIterator<Build>(SelectedIntegration.Build);
                    for (Build bld = bldIterator.First(); !bldIterator.IsDone; bld = bldIterator.Next())
                    {
                        if (bld is InstructionBuild insBld)
                        {
                            //Iterate through every InstructionBuild
                            SwagItemPreOrderIterator<Instruction> insIterator = new SwagItemPreOrderIterator<Instruction>(insBld.Instructions);
                            for (Instruction ins = insIterator.First(); !insIterator.IsDone; ins = insIterator.Next())
                            {
                                if (ins is TemplateInstruction template)
                                {
                                    if (template.TemplateKey == insTemplate.Path)
                                    {
                                        template.Template.Children.Clear();
                                        Instruction clone = JsonHelper.Clone<Instruction>(insTemplate);
                                        template.Template.Children.Add(clone);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ImportIntegration_Click(object sender, RoutedEventArgs e)
        {
            Integration integation = SwagItemsControlHelper.GetDataFromFile<Integration>();
            //Iterate though all builds
            SwagItemPreOrderIterator<Build> bldIterator = new SwagItemPreOrderIterator<Build>(integation.Build);
            integation.IntegrationId = 0;
            integation.BuildId = 0;
            integation.TestBuildId = 0;

            for (Build bld = bldIterator.First(); !bldIterator.IsDone; bld = bldIterator.Next())
            {
                bld.BuildId = 0;
            }

            SwagItemPreOrderIterator<Build> tstBldIterator = new SwagItemPreOrderIterator<Build>(integation.TestBuild);
            for (Build bld = tstBldIterator.First(); !tstBldIterator.IsDone; bld = tstBldIterator.Next())
            {
                bld.BuildId = 0;
            }

            DreamporterWPFContainer.IntegrationDataRepository.Insert(integation);
            Integrations.Add(integation);
            SelectedIntegration = integation;
        }

        private void ExportIntegration_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                SwagItemsControlHelper.ExportDataToFile<Integration>(SelectedIntegration, SelectedIntegration.Name);
            }
        }

        private void ExportFolderOpen_Click(object sender, RoutedEventArgs e)
        {
            String directoryPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Export");
            if (Directory.Exists(directoryPath))
            {
                Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", directoryPath);
            }
            else
            {
                MessageBox.Show("Export Folder Not Found");
            }
            tbShortCuts.IsChecked = false;
        }

        private void CacheFolderOpen_Click(object sender, RoutedEventArgs e)
        {
            String directoryPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "DataCache");
            if (Directory.Exists(directoryPath))
            {
                Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", directoryPath);
            }
            else
            {
                MessageBox.Show("DataCache Folder Not Found");
            }
            tbShortCuts.IsChecked = false;
        }

        private void TestContexts_Save(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                if (SelectedIntegration.IntegrationId != 0)
                {
                    SelectedIntegration.TestContexts = SelectedIntegration.TestContexts;
                    DreamporterWPFContainer.IntegrationDataRepository.Update(SelectedIntegration);
                }
                DreamporterWPFContainer.Context.SaveChanges();
                SwagLogger.Log("Test Contexts Saved {timeStamp}", DateTime.Now);
            }
        }

        private void TestContexts_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (String filePath in files)
                {
                    TestContext testContext = new TestContext() { InitialDB = filePath };
                    SelectedIntegration.TestContexts.Add(testContext);
                }
            }
        }

        private void TestBuild_ApplyQueriesByTargetPath(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                //Iterate through all test builds
                SwagItemPreOrderIterator<Build> testBldIterator = new SwagItemPreOrderIterator<Build>(SelectedIntegration.TestBuild);
                for (Build testBld = testBldIterator.First(); !testBldIterator.IsDone; testBld = testBldIterator.Next())
                {
                    if (testBld is InstructionBuild testInsBld)
                    {
                        //Iterate through all test build instructions
                        SwagItemPreOrderIterator<Instruction> testInsIterator = new SwagItemPreOrderIterator<Instruction>(testInsBld.Instructions);

                        for (Instruction testIns = testInsIterator.First(); !testInsIterator.IsDone; testIns = testInsIterator.Next())
                        {
                            if (testIns is QueryInstruction testQryIns && testQryIns.Options.Dict.ContainsKey("TargetPath"))
                            {
                                //Iterate though all builds
                                SwagItemPreOrderIterator<Build> bldIterator = new SwagItemPreOrderIterator<Build>(SelectedIntegration.Build);
                                for (Build bld = bldIterator.First(); !bldIterator.IsDone; bld = bldIterator.Next())
                                {
                                    if (bld is InstructionBuild insBld)
                                    {
                                        String buildPath = insBld.Path;
                                        //Iterate through every InstructionBuild
                                        SwagItemPreOrderIterator<Instruction> insIterator = new SwagItemPreOrderIterator<Instruction>(insBld.Instructions);
                                        for (Instruction ins = insIterator.First(); !insIterator.IsDone; ins = insIterator.Next())
                                        {
                                            String insPath = ins.Path;
                                            String fullPath = $"[{buildPath}] {insPath}";
                                            System.Diagnostics.Debug.WriteLine(fullPath);

                                            //If path matches
                                            if (ins is QueryInstruction qryIns && fullPath == testQryIns.Options.Dict["TargetPath"])
                                            {
                                                qryIns.Query = testQryIns.Query;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private async void RunIntegration_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                SelectedIntegration.Build.TabIndex = 3;

                #region Keep track of Selected Build and Instruction
                Build selectedBuild = SelectedIntegration.SelectedBuild;
                Instruction selectedInstruction = null;

                if (selectedBuild is InstructionBuild insbldStart)
                {
                    selectedInstruction = insbldStart.SelectedInstruction;
                }
                #endregion Keep track of Selected Build and Instruction

                IsBusy = true;
                RunContext runContext = null;
                runContext = await SelectedIntegration.RunAsync(inTestMode: InTestMode);
                runContext.ExportDB($"Export\\{SelectedIntegration.Name}{(InTestMode ? "_Test" : "")}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.db");

                DataSet dsResult = runContext.GetDataSet();
                runContext.Close();

                #region Resolve DataSets
                SwagDataSetWPF data = new SwagDataSetWPF() { Display = $"Export\\{SelectedIntegration.Name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}" };
                Dictionary<String, DataSet> dictDataSets = new Dictionary<string, DataSet>();

                foreach (DataTable dtbl in dsResult.Tables)
                {
                    String schemaName = "_____", tableName = "";
                    String[] parts = dtbl.TableName.Split('.');   //period delimeter
                    if (parts.Length > 2)
                    {
                        tableName = parts[parts.Length - 1];
                        schemaName = "";
                        for (int i = 0; i < parts.Length - 1; i++)
                        {
                            schemaName += $"{parts[i]}.";
                        }
                        schemaName = schemaName.TrimEnd('.');
                    }
                    else if (parts.Length == 2)  //schema + table
                    {
                        schemaName = parts[0];
                        tableName = parts[1];
                    }
                    else if (parts.Length == 1)     //just table
                    {
                        tableName = parts[0];
                    }

                    if (dtbl.TableName.ToLower().StartsWith($"{schemaName.ToLower()}."))
                    {
                        if (!dictDataSets.ContainsKey(schemaName))
                        {
                            dictDataSets.Add(schemaName, new DataSet(schemaName));
                        }

                        DataTable dtblcopy = dtbl.Copy();
                        DataTableHelper.AutoConvertColumns(dtblcopy);
                        dictDataSets[schemaName].Tables.Add(dtblcopy);
                    }
                }

                if (dictDataSets.ContainsKey("util"))
                {
                    DataSet dsUtil = dictDataSets["util"];
                    if (dsUtil.Tables.Count > 0)
                    {
                        SwagDataSetWPF util = new SwagDataSetWPF(dsUtil) { Display = "util" };
                        data.Children.Add(util);
                    }
                }

                if (InTestMode)
                {
                    List<String> targetSchemas = new List<string>();
                    RunParams rp = SelectedIntegration.GenerateRunParams();
                    if (rp.Params.ContainsKey("TestTargetSchemas"))
                    {
                        String[] schemas = rp.Params["TestTargetSchemas"].Split(',');
                        foreach (String schema in schemas)
                        {
                            targetSchemas.Add(schema.ToLower());
                        }
                    }

                    foreach (KeyValuePair<string, DataSet> kvpDataSet in dictDataSets)
                    {
                        if (kvpDataSet.Key != "util" && (targetSchemas.Count == 0 || targetSchemas.Contains(kvpDataSet.Key.ToLower())))
                        {
                            SwagDataSetWPF sds = new SwagDataSetWPF(kvpDataSet.Value) { Display = kvpDataSet.Key };
                            data.Children.Add(sds);
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, DataSet> kvpDataSet in dictDataSets)
                    {
                        if (kvpDataSet.Key != "util")
                        {
                            SwagDataSetWPF sds = new SwagDataSetWPF(kvpDataSet.Value) { Display = kvpDataSet.Key };
                            data.Children.Add(sds);
                        }
                    }
                }
                
                #endregion Resolve DataSets

                #region DataSets Window
                SwagWindow swagWindow = new SwagWindow();
                SwagDataControl swagDataControl = new SwagDataControl();
                swagDataControl.SwagDataSet = data;
                swagWindow.Content = swagDataControl;
                swagWindow.Show();
                #endregion DataSets Window

                IsBusy = false;

                #region Select last Build and Instruction
                if (selectedBuild != null)
                {
                    GroupBuild parent = selectedBuild.Parent;
                    while (parent != null)
                    {
                        parent.IsExpanded = true;
                        parent = parent.Parent;
                    }
                    SelectedIntegration.SelectedBuild = selectedBuild;
                    selectedBuild.IsSelected = true;
                }

                if (selectedBuild is InstructionBuild insbldEnd && selectedInstruction != null)
                {
                    GroupInstruction parent = selectedInstruction.Parent;
                    while (parent != null)
                    {
                        parent.IsExpanded = true;
                        parent = parent.Parent;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        //Delay selection because Instruction gets selected automatically upon creation (last child gets selected)
                        selectedInstruction.IsSelected = true;
                        insbldEnd.SelectedInstruction = selectedInstruction;
                    }, System.Windows.Threading.DispatcherPriority.Background);
                }
                #endregion Select last Build and Instruction
            }
        }
        #endregion Events

    }
}
