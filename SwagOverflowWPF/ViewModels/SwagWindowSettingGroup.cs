using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro;
using MahApps.Metro.IconPacks;
using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Repository;
using SwagOverflowWPF.UI;

namespace SwagOverflowWPF.ViewModels
{
    public class SwagWindowSettingGroup : SwagSettingGroupViewModel
    {
        SwagContext _context;
        public SwagWindowSettingGroup() : base()
        {
            this["Window"] = new SwagSettingViewModel() { SettingType = SettingType.SettingGroup, Icon = PackIconMaterialKind.SettingsOutline };
            this["Window"]["Status"] = new SwagSettingViewModel() { SettingType = SettingType.Hidden };
            this["Window"]["Status"]["Message"] = new SwagSettingViewModel<String>() { SettingType = SettingType.Hidden };
            this["Window"]["Status"]["IsBusy"] = new SwagSettingViewModel<Boolean>() { SettingType = SettingType.Hidden, GenericValue = false };
            this["Window"]["Settings"] = new SwagSettingViewModel() { SettingType = SettingType.Hidden };
            this["Window"]["Settings"]["IsOpen"] = new SwagSettingViewModel<Boolean>() { GenericValue = false, SettingType = SettingType.Hidden };
            this["Window"]["CommandHistory"] = new SwagSettingViewModel() { SettingType = SettingType.Hidden };
            this["Window"]["CommandHistory"]["IsOpen"] = new SwagSettingViewModel<Boolean>() { GenericValue = false, SettingType = SettingType.Hidden };
            this["Window"]["Theme"] = new SwagSettingViewModel() { SettingType = SettingType.SettingGroup, Icon = PackIconMaterialKind.PaletteOutline };
            this["Window"]["Theme"]["Base"] = new SwagSettingViewModel<String>() { GenericValue = "Light", Icon = PackIconMaterialKind.PaletteSwatchOutline, GenericItemsSource = new[] { "Light", "Dark" }, SettingType = SettingType.DropDown };
            this["Window"]["Theme"]["Accent"] = new SwagSettingViewModel<String>() { GenericValue = "Blue", Icon = PackIconMaterialKind.Brush, GenericItemsSource = new [] { "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna" }, SettingType = SettingType.DropDown };
        }

        public void Initialize()
        {
            this["Window"]["Theme"]["Base"].PropertyChanged += WindowSettingCollection_ThemePropertyChanged;
            this["Window"]["Theme"]["Accent"].PropertyChanged += WindowSettingCollection_ThemePropertyChanged;
            WindowSettingCollection_ThemePropertyChanged(this, new PropertyChangedEventArgs("Value"));

            SwagItemChanged += SwagWindowSettingGroup_SwagItemChanged;
        }

        public void SetContext(SwagContext context)
        {
            _context = context;
        }

        private void SwagWindowSettingGroup_SwagItemChanged(object sender, SwagItemChangedEventArgs e)
        {
            SwagSettingUnitOfWork work = new SwagSettingUnitOfWork(_context);
            work.Settings.Update((SwagSettingViewModel)e.SwagItem);
            work.Complete();
        }

        private void WindowSettingCollection_ThemePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                String myBase = this["Window"]["Theme"]["Base"].Value.ToString();
                String myAccent = this["Window"]["Theme"]["Accent"].Value.ToString();

                MahApps.Metro.Theme theme = ThemeManager.GetTheme($"{myBase}.{myAccent}");
                ThemeManager.ChangeTheme(Application.Current.MainWindow.Resources, theme);

                ResourceDictionary resourceDictionary = new ResourceDictionary();
                resourceDictionary.Source = new Uri($"pack://application:,,,/MahApps.Metro;component/Styles/Themes/{myBase}.{myAccent}.xaml", UriKind.Absolute);

                //When there are duplicate references to Common.xaml (unavoidable since we are using Custom Controls),
                //add the theme to the Merged Dictionary 
                //(inefficient but there is no way to reset the MergedDictionaries without breaking during runtime)
                foreach (SettingsControl sc in Application.Current.MainWindow.FindVisualChildren<SettingsControl>())
                {
                    sc.Resources.MergedDictionaries.Add(resourceDictionary);
                }

                foreach (SwagComboBox scbx in Application.Current.MainWindow.FindVisualChildren<SwagComboBox>())
                {
                    scbx.Resources.MergedDictionaries.Add(resourceDictionary);
                }

                foreach (SwagDataGrid sdg in Application.Current.MainWindow.FindVisualChildren<SwagDataGrid>())
                {
                    sdg.Resources.MergedDictionaries.Add(resourceDictionary);
                }
            }
        }
    }

}
