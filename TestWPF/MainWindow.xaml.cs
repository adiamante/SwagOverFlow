using Newtonsoft.Json;
using SwagOverflowWPF.Controllers;
using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Iterator;
using SwagOverflowWPF.Repository;
using SwagOverflowWPF.ViewModels;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SwagWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            SwagWindowSettingController controller = new SwagWindowSettingController(new SwagContext());
            String groupName = $"{Assembly.GetEntryAssembly().GetName().Name}";
            this.Settings = controller.GetWindowSettingGroupByName(groupName);
        }
    }
}
