using System;
using System.ComponentModel;
using System.Windows;
using MahApps.Metro;
using MahApps.Metro.IconPacks;
using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Repository;
using SwagOverflowWPF.UI;

namespace SwagOverflowWPF.ViewModels
{
    public class SwagWindowSettingGroup : SwagSettingGroup
    {
        SwagContext _context;
        public SwagWindowSettingGroup() : base()
        {
            this["Window"] = new SwagSetting() { SettingType = SettingType.SettingGroup, Icon = PackIconMaterialKind.SettingsOutline };
            this["Window"]["Status"] = new SwagSetting() { SettingType = SettingType.Hidden };
            this["Window"]["Status"]["Message"] = new SwagSettingString() { SettingType = SettingType.Hidden };
            this["Window"]["Status"]["IsBusy"] = new SwagSettingBoolean() { SettingType = SettingType.Hidden, GenericValue = false };
            this["Window"]["Settings"] = new SwagSetting() { SettingType = SettingType.Hidden };
            this["Window"]["Settings"]["IsOpen"] = new SwagSettingBoolean() { GenericValue = false, SettingType = SettingType.Hidden };
            this["Window"]["CommandHistory"] = new SwagSetting() { SettingType = SettingType.Hidden };
            this["Window"]["CommandHistory"]["IsOpen"] = new SwagSettingBoolean() { GenericValue = false, SettingType = SettingType.Hidden };
            this["Window"]["Theme"] = new SwagSetting() { SettingType = SettingType.SettingGroup, Icon = PackIconMaterialKind.PaletteOutline };
            this["Window"]["Theme"]["Base"] = new SwagSettingString() { GenericValue = "Light", Icon = PackIconMaterialKind.PaletteSwatchOutline, GenericItemsSource = new[] { "Light", "Dark" }, SettingType = SettingType.DropDown };
            this["Window"]["Theme"]["Accent"] = new SwagSettingString() { GenericValue = "Blue", Icon = PackIconMaterialKind.Brush, GenericItemsSource = new [] { "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna" }, SettingType = SettingType.DropDown };
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
            work.Settings.Update((SwagSetting)e.SwagItem);
            work.Complete();
        }

        public Theme GetCurrentTheme()
        {
            String myBase = this["Window"]["Theme"]["Base"].Value.ToString();
            String myAccent = this["Window"]["Theme"]["Accent"].Value.ToString();

            Theme theme = ThemeManager.GetTheme($"{myBase}.{myAccent}");
            return theme;
        }

        private void WindowSettingCollection_ThemePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                Theme theme = GetCurrentTheme();
                ThemeManager.ChangeTheme(Application.Current.MainWindow.Resources, theme);

                #region OLD - Theme switching is now handled in SwagControlBase.cs
                //ResourceDictionary resourceDictionary = new ResourceDictionary();
                //resourceDictionary.Source = new Uri($"pack://application:,,,/MahApps.Metro;component/Styles/Themes/{theme.BaseColorScheme}.{theme.ColorScheme}.xaml", UriKind.Absolute);

                ////When there are duplicate references to Common.xaml (unavoidable since we are using Custom Controls),
                ////add the theme to the Merged Dictionary 
                ////(inefficient but there is no way to reset the MergedDictionaries without breaking during runtime)
                //foreach (SettingsControl sc in Application.Current.MainWindow.FindVisualChildren<SettingsControl>())
                //{
                //    sc.Resources.MergedDictionaries.Add(resourceDictionary);
                //}

                //foreach (SwagComboBox scbx in Application.Current.MainWindow.FindVisualChildren<SwagComboBox>())
                //{
                //    scbx.Resources.MergedDictionaries.Add(resourceDictionary);
                //}

                //foreach (SwagDataGrid sdg in Application.Current.MainWindow.FindVisualChildren<SwagDataGrid>())
                //{
                //    sdg.Resources.MergedDictionaries.Add(resourceDictionary);
                //}

                //foreach (SearchTextBox stb in Application.Current.MainWindow.FindVisualChildren<SearchTextBox>())
                //{
                //    stb.Resources.MergedDictionaries.Add(resourceDictionary);
                //}
                #endregion OLD - Theme switching is now handled in SwagControlBase.cs
            }
        }
    }

}
