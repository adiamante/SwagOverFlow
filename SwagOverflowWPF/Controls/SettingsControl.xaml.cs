using SwagOverflowWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SwagOverflowWPF.Controls
{
    /// <summary>
    /// Interaction logic for WindowSettings.xaml
    /// </summary>
    public partial class SettingsControl : SwagControlBase
    {
        #region Settings
        private static readonly DependencyProperty SettingsProperty =
        DependencyProperty.Register("Settings", typeof(SwagSettingGroup), typeof(SettingsControl));

        public SwagSettingGroup Settings
        {
            get { return (SwagSettingGroup)GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }
        #endregion Settings

        public SettingsControl()
        {
            InitializeComponent();
        }
    }
}
