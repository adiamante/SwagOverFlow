using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro;
using SwagOverflowWPF.UI;
using SwagOverflowWPF.ViewModels;

namespace SwagOverflowWPF.Controls
{
    public class SwagWindowSettings : SwagSettingGroupViewModel
    {
        public SwagWindowSettings() : base()
        {
            //String groupName = $"{Assembly.GetEntryAssembly().GetName().Name}";
            //this.Name = this.Display = groupName;

            this["Settings"] = new SwagSettingViewModel() { SettingType = SettingType.Hidden };
            this["Settings"]["IsOpen"] = new SwagSettingViewModel<Boolean>() { GenericValue = true, SettingType = SettingType.Hidden };
            this["General"]["Theme"]["Base"] = new SwagSettingViewModel<String>() { GenericValue = "Light", Sequence = 0, ItemsSource = new[] { "Light", "Dark" }, SettingType = SettingType.DropDown };
            this["General"]["Theme"]["Accent"] = new SwagSettingViewModel<String>() { GenericValue = "Blue", Sequence = 1, ItemsSource = new [] { "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna" }, SettingType = SettingType.DropDown };

            this["General"]["Theme"]["Base"].PropertyChanged += WindowSettingCollection_ThemePropertyChanged;
            this["General"]["Theme"]["Accent"].PropertyChanged += WindowSettingCollection_ThemePropertyChanged;
        }

        public SwagWindowSettings(SwagSettingGroupViewModel baseClass)
        {
            this.Root = baseClass.Root;     //shallow copy
            this["General"]["Theme"]["Base"].PropertyChanged += WindowSettingCollection_ThemePropertyChanged;
            this["General"]["Theme"]["Accent"].PropertyChanged += WindowSettingCollection_ThemePropertyChanged;
        }

        private void WindowSettingCollection_ThemePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                String myBase = this["General"]["Theme"]["Base"].Value.ToString();
                String myAccent = this["General"]["Theme"]["Accent"].Value.ToString();

                MahApps.Metro.Theme theme = ThemeManager.GetTheme($"{myBase}.{myAccent}");
                ThemeManager.ChangeTheme(Application.Current.MainWindow.Resources, theme);

                ResourceDictionary resourceDictionary = new ResourceDictionary();
                resourceDictionary.Source = new Uri($"pack://application:,,,/MahApps.Metro;component/Styles/Themes/{myBase}.{myAccent}.xaml", UriKind.Absolute);

                //When there are duplicate references to Common.xaml (unavoidable since we are using Custom Controls),
                //add the theme to the Merged Dictionary 
                //(inefficient but there is no way to reset the MergedDictionaries without breaking during runtime)
                foreach (var sc in Application.Current.MainWindow.FindVisualChildren<SettingsControl>())
                {
                    sc.Resources.MergedDictionaries.Add(resourceDictionary);
                }
            }
        }
    }

}
