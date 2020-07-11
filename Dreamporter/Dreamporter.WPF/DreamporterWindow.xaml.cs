using Dreamporter.Core;
using Dreamporter.Data;
using Dreamporter.WPF.Services;
using SwagOverFlow.WPF.Controls;
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
                IEnumerable<Integration> integrations = integrationRepo.Get(includeProperties: "Build");
                foreach (Integration integration in integrations)
                {
                    DreamporterWPFContainer.BuildRepository.RecursiveLoadCollection(integration.Build, "Children");
                }
                return new ObservableCollection<Integration>(integrationRepo.Get(includeProperties: "Build"));
            }
        }

        public DreamporterWindow()
        {
            InitializeComponent();
        }
    }
}
