using Dreamporter.Core;
using Dreamporter.WPF.Services;
using MahApps.Metro.IconPacks;
using SwagOverFlow.Iterator;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.ViewModels;
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
        }

        private void InitSettings()
        {
            SwagWindow.GlobalSettings.TryAddChildSetting("Dreamporter", new SwagSettingGroup() { Icon = PackIconMaterialKind.Cloud });
            SwagWindow.GlobalSettings["Dreamporter"].TryAddChildSetting("SelectedIntegration", new SwagSettingString() { Icon = PackIconMaterialKind.Bridge });
        }
        #endregion Initialization

        #region Events
        private void cbxIntegrations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedIntegration != null && SelectedIntegrationName != SelectedIntegration.Name)
            {
                SelectedIntegrationName = SelectedIntegration.Name;
            }
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
        #endregion Events

        
    }
}
