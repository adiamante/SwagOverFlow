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
            //this["General"]["Theme"] = new SwagSettingViewModel<String>() { Value = "Temp" };
            this["General"]["Theme"]["Base"] = new SwagSettingViewModel<String>() { Value = "Light", ItemsSource = new[] { "Light", "Dark" }, SettingType = SettingType.DropDown };
            this["General"]["Theme"]["Accent"] = new SwagSettingViewModel<String>() { Value = "Blue", ItemsSource = new [] { "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna" }, SettingType = SettingType.DropDown };
            //this["General"]["Theme2"] = new SwagSettingViewModel<String>() { Value = "Option3", ItemsSource = new[] { "Option1", "Option2", "Option3" }, SettingType = SettingType.DropDown };
            //this["General"]["Nest"]["Theme"] = new SwagSettingViewModel<String>() { Value = "Option1", ItemsSource = new[] { "Option1", "Option2", "Option3" }, SettingType = SettingType.DropDown };
            //this["General"]["Nest"]["Theme2"] = new SwagSettingViewModel<String>() { Value = "Option3", ItemsSource = new[] { "Option1", "Option2", "Option3" }, SettingType = SettingType.DropDown };

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
