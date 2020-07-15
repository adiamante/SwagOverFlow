using ControlzEx.Theming;
using Dreamporter.Core;
using Dreamporter.WPF.Services;
using MahApps.Metro.IconPacks;
using SwagOverFlow.Iterator;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.UI;
using SwagOverFlow.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                typeof(DreamporterControl));

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
            }
        }

        private void SchemaGroups_Add(object sender, RoutedEventArgs e)
        {
            String group = UIHelper.StringInputDialog("Please enter Group Name:");
            if (!String.IsNullOrEmpty(group))
            {
                SelectedIntegration.SchemaGroups.Add(group);
            }
        }

        private void SchemaGroups_Save(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                if (SelectedIntegration.IntegrationId != 0)
                {
                    SelectedIntegration.SchemaGroups = SelectedIntegration.SchemaGroups;
                    DreamporterWPFContainer.IntegrationDataRepository.Update(SelectedIntegration);
                }
                DreamporterWPFContainer.Context.SaveChanges();
            }
        }

        private void RunIntegration_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                RunContext runContext = new RunContext();
                runContext.Open();
                SelectedIntegration.InitRunContext(runContext);

                RunParams runParams = SelectedIntegration.GenerateRunParams(true);
                SelectedIntegration.Build.Run(runContext, runParams);
                runContext.ExportDB($"Export\\{SelectedIntegration.Name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.db");

                DataSet dsResult = runContext.GetDataSet();
                runContext.Close();

                #region Resolve DataSets
                SwagDataSetWPF data = new SwagDataSetWPF() { Display = $"Export\\{SelectedIntegration.Name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}" };
                List<String> schemas = new List<string>() { "Util" };
                Dictionary<String, DataSet> dictDataSets = new Dictionary<string, DataSet>();

                foreach (String group in SelectedIntegration.SchemaGroups)
                {
                    schemas.Add(group);
                }

                DataSet dsRaw = new DataSet();
                foreach (DataTable dt in dsResult.Tables)
                {
                    Boolean bFoundSchema = false;
                    foreach (String schema in schemas)
                    {
                        if (dt.TableName.ToLower().StartsWith($"{schema.ToLower()}."))
                        {
                            if (!dictDataSets.ContainsKey(schema))
                            {
                                dictDataSets.Add(schema, new DataSet(schema));
                            }
                            dictDataSets[schema].Tables.Add(dt.Copy());
                            bFoundSchema = true;
                            break;
                        }
                    }

                    if (!bFoundSchema)
                    {
                        dsRaw.Tables.Add(dt.Copy());
                    }
                }

                if (dictDataSets.ContainsKey("Util"))
                {
                    DataSet dsUtil = dictDataSets["Util"];
                    if (dsUtil.Tables.Count > 0)
                    {
                        SwagDataSetWPF util = new SwagDataSetWPF(dsUtil) { Display = "Util" };
                        data.Children.Add(util);
                    }
                }

                if (dsRaw.Tables.Count > 0)
                {
                    SwagDataSetWPF raw = new SwagDataSetWPF(dsRaw) { Display = "Raw" };
                    data.Children.Add(raw);
                }

                foreach (KeyValuePair<string, DataSet> kvpDataSet in dictDataSets)
                {
                    if (kvpDataSet.Key != "Util")
                    {
                        SwagDataSetWPF sds = new SwagDataSetWPF(kvpDataSet.Value) { Display = kvpDataSet.Key };
                        data.Children.Add(sds);
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
                    //Iterate though all builds
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
                                        template.Template.Clear();
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
        }

        private void ExportIntegration_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                SwagItemsControlHelper.ExportDataToFile<Integration>(SelectedIntegration, SelectedIntegration.Name);
            }
        }
        #endregion Events

    }
}
