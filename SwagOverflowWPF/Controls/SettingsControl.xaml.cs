using SwagOverflowWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SwagOverflowWPF.Controls
{
    /// <summary>
    /// Interaction logic for WindowSettings.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        #region Settings
        private static readonly DependencyProperty SettingsProperty =
        DependencyProperty.Register("Settings", typeof(SwagWindowSettingGroup), typeof(SettingsControl));

        public SwagWindowSettingGroup Settings
        {
            get { return (SwagWindowSettingGroup)GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }
        #endregion Settings

        public SettingsControl()
        {
            InitializeComponent();
        }
    }
}
