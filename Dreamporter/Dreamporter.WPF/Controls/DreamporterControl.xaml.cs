﻿using ControlzEx.Theming;
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
        }

        private void ExportIntegration_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIntegration != null)
            {
                SwagItemsControlHelper.ExportDataToFile<Integration>(SelectedIntegration, SelectedIntegration.Name);
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
                RunContext runContext = await SelectedIntegration.RunAsync();
                runContext.ExportDB($"Export\\{SelectedIntegration.Name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.db");

                DataSet dsResult = runContext.GetDataSet();
                runContext.Close();

                #region Resolve DataSets
                SwagDataSetWPF data = new SwagDataSetWPF() { Display = $"Export\\{SelectedIntegration.Name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}" };
                Dictionary<String, DataSet> dictDataSets = new Dictionary<string, DataSet>();

                foreach (DataTable dtbl in dsResult.Tables)
                {
                    String schemaName = "_____", tableName = "";
                    String[] parts = dtbl.TableName.Split('.');   //period delimeter
                    if (parts.Length == 2)  //schema + table
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
                        dictDataSets[schemaName].Tables.Add(dtbl.Copy());
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

                foreach (KeyValuePair<string, DataSet> kvpDataSet in dictDataSets)
                {
                    if (kvpDataSet.Key != "util")
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
                        insbldEnd.SelectedInstruction = selectedInstruction;
                        selectedInstruction.IsSelected = true;
                    }, System.Windows.Threading.DispatcherPriority.Background);
                }
                #endregion Select last Build and Instruction
            }
        }
        #endregion Events

    }
}
