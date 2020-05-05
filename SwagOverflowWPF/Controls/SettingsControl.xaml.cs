using SwagOverflowWPF.UI;
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

        #region SettingCustomTemplates
        public static readonly DependencyProperty SettingCustomTemplatesProperty =
            DependencyProperty.Register("SettingCustomTemplates", typeof(SwagTemplateCollection), typeof(SettingsControl),
            new FrameworkPropertyMetadata(new SwagTemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public SwagTemplateCollection SettingCustomTemplates
        {
            get { return (SwagTemplateCollection)GetValue(SettingCustomTemplatesProperty); }
            set { SetValue(SettingCustomTemplatesProperty, value); }
        }
        #endregion SettingCustomTemplates

        public SettingsControl()
        {
            InitializeComponent();
        }
    }
}
