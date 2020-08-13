using Dreamporter.Core;
using Dreamporter.Data;
using Dreamporter.WPF.Services;
using MahApps.Metro.IconPacks;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DreamporterWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DreamporterWindow : SwagWindow
    {
        public ObservableCollection<Integration> Integrations
        {
            get
            {
                IIntegrationRepository integrationRepo = DreamporterWPFContainer.IntegrationDataRepository;
                IEnumerable<Integration> integrations = integrationRepo.Get(includeProperties: "Build,TestBuild");
                foreach (Integration integration in integrations)
                {
                    DreamporterWPFContainer.BuildRepository.RecursiveLoadCollection(integration.Build, "Children");
                    DreamporterWPFContainer.BuildRepository.RecursiveLoadCollection(integration.TestBuild, "Children");
                }
                return new ObservableCollection<Integration>(integrations);
            }
        }

        public DreamporterWindow()
        {
            InitializeComponent();
            Icon = UIHelper.GetImageSource(PackIconJamIconsKind.Universe);
            //UIHelper.CreateIcon(PackIconJamIconsKind.Universe);
        }
    }
}
