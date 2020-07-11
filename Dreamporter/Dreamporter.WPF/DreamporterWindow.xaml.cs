using Dreamporter.Core;
using Dreamporter.Data;
using Dreamporter.WPF.Services;
using SwagOverFlow.Utils;
using SwagOverFlow.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
