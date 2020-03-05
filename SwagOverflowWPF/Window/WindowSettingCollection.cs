using System;
using System.Windows;
using MahApps.Metro;
using MahApps.Metro.Controls;
using SwagOverflowWPF.ViewModels;

namespace SwagOverflowWPF.Controls
{
    public class WindowSettingCollection : SwagSettingCollection
    {
        public WindowSettingCollection() : base()
        {
            this.Display = "Root";
            this["SettingsIsOpen"] = new SwagSettingViewModel<Boolean>() { GenericValue = true };
            this["General"]["Theme"]["Base"] = new SwagSettingViewModel<String>() { GenericValue = "Light", ItemsSource = new[] { "Light", "Dark" }, SettingType = SettingType.DropDown };
            this["General"]["Theme"]["Accent"] = new SwagSettingViewModel<String>() { GenericValue = "Blue", ItemsSource = new [] { "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna" }, SettingType = SettingType.DropDown };

            this["General"]["Theme"]["Base"].PropertyChanged += WindowSettingCollection_ThemePropertyChanged;
            this["General"]["Theme"]["Accent"].PropertyChanged += WindowSettingCollection_ThemePropertyChanged;
        }

        private void WindowSettingCollection_ThemePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            String myBase = this["General"]["Theme"]["Base"].Value.ToString();
            String myAccent = this["General"]["Theme"]["Accent"].Value.ToString();

            MahApps.Metro.Theme theme = ThemeManager.GetTheme($"{myBase}.{myAccent}");
            ThemeManager.ChangeTheme(Application.Current.MainWindow, theme);
        }
    }
}
