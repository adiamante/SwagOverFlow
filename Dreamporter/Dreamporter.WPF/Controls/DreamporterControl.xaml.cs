using Dreamporter.Core;
using Dreamporter.WPF.Services;
using MahApps.Metro.IconPacks;
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
            set { SetValue(SelectedIntegrationProperty, value); }
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
            if (!((SwagSettingGroup)SwagWindow.GlobalSettings).ContainsKey("Dreamporter"))
            {
                SwagSettingWPFGroup swagDataSetting = new SwagSettingWPFGroup() { Icon = PackIconMaterialKind.Cloud };
                SwagWindow.GlobalSettings["Dreamporter"] = swagDataSetting;
                swagDataSetting.IconString = swagDataSetting.IconString;
                swagDataSetting.IconTypeString = swagDataSetting.IconTypeString;
                ((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
            }

            if (!((SwagSettingGroup)SwagWindow.GlobalSettings["Dreamporter"]).ContainsKey("SelectedIntegration"))
            {
                SwagSetting<String> ssSelectedIntegration = new SwagSetting<String>()
                {
                    Icon = PackIconMaterialKind.Bridge
                };

                ssSelectedIntegration.IconString = ssSelectedIntegration.IconString;
                ssSelectedIntegration.IconTypeString = ssSelectedIntegration.IconTypeString;
                ssSelectedIntegration.ValueTypeString = ssSelectedIntegration.ValueTypeString;
                ssSelectedIntegration.ObjValue = ssSelectedIntegration.ObjValue;
                SwagWindow.GlobalSettings["Dreamporter"]["SelectedIntegration"] = ssSelectedIntegration;
                ((SwagWindowSettingGroup)SwagWindow.GlobalSettings).Save();
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
        }

        private void AddIntegration_Click(object sender, RoutedEventArgs e)
        {
            Integration newIntegration = new Integration() { Name = $"Integration {Integrations.Count + 1}" };
            DreamporterWPFContainer.IntegrationDataRepository.Insert(newIntegration);
            Integrations.Add(newIntegration);
            SelectedIntegration = newIntegration;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
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
        #endregion Events
    }
}
