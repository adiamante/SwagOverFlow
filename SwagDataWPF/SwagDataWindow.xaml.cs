using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Services;
using SwagOverflowWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace SwagDataWPF
{
    /// <summary>
    /// Interaction logic for SwagDataWindow.xaml
    /// </summary>
    public partial class SwagDataWindow : SwagWindow
    {
        #region SwagDataSet
        private static readonly DependencyProperty SwagDataSetProperty =
            DependencyProperty.Register("SwagDataSet", typeof(SwagDataSet), typeof(SwagDataWindow));

        public SwagDataSet SwagDataSet
        {
            get { return (SwagDataSet)GetValue(SwagDataSetProperty); }
            set { SetValue(SwagDataSetProperty, value); }
        }
        #endregion SwagDataSet

        public SwagDataWindow()
        {
            InitializeComponent();

            SwagWPFServices.Context.Database.EnsureCreated();
            String settingGoupName = $"{Assembly.GetEntryAssembly().GetName().Name}_Settings";
            this.Settings = SwagWPFServices.SettingsService.GetWindowSettingGroupByName(settingGoupName);

            SwagDataSet = new SwagDataSet();
        }
    }
}
