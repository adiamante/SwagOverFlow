using MahApps.Metro.Controls;
using SwagOverflowWPF.ViewModels;
using System.Windows;

namespace SwagOverflowWPF.Controls
{
    public class SwagWindow : MetroWindow
    {
        #region Settings
        private static readonly DependencyProperty SettingsProperty =
        DependencyProperty.Register("Settings", typeof(WindowSettingCollection), typeof(SwagWindow),
            new FrameworkPropertyMetadata(new WindowSettingCollection()));

        public WindowSettingCollection Settings
        {
            get { return (WindowSettingCollection)GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }
        #endregion Settings

        static SwagWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwagWindow), new FrameworkPropertyMetadata(typeof(SwagWindow)));
        }

        public SwagWindow()
        {
            
        }
    }
}
