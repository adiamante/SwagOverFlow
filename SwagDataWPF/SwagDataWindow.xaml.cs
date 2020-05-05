using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Services;
using SwagOverflowWPF.ViewModels;
using System;
using System.Reflection;
using System.Windows;

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
