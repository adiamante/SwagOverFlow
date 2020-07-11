using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;
using System.Windows.Data;
using ControlzEx.Theming;
using MahApps.Metro.IconPacks;
using Newtonsoft.Json;
using SwagOverFlow.Data.Persistence;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;

namespace SwagOverFlow.WPF.ViewModels
{
    public class SwagWindowSettingGroup : SwagSettingGroup
    {
        #region Private/Protected Members
        CollectionViewSource _childrenCollectionViewSource;
        #endregion Private/Protected Members

        #region Properties
        #region ChildrenView
        [JsonIgnore]
        [NotMapped]
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView
        #endregion Properties

        #region Initialization
        public SwagWindowSettingGroup() : base()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
        }

        public SwagWindowSettingGroup(SwagSettingGroup swagSettingGroup) : this()
        {
            PropertyCopy.Copy(swagSettingGroup, this);
            foreach (SwagSetting child in Children)
            {
                if (!_dict.ContainsKey(child.Key))
                {
                    _dict.Add(child.Key, child);
                }
            }

            _childrenCollectionViewSource.Source = _children;
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
        }

        public SwagWindowSettingGroup(Boolean doInit) : this()
        {
            if (doInit)
            {
                this["Window"] = new SwagSettingGroup() { SettingType = SettingType.SettingGroup, Icon = PackIconMaterialKind.CogOutline };
                this["Window"]["Status"] = new SwagSettingGroup() { SettingType = SettingType.Hidden };
                this["Window"]["Status"]["Message"] = new SwagSettingString() { SettingType = SettingType.Hidden };
                this["Window"]["Status"]["IsBusy"] = new SwagSettingBoolean() { SettingType = SettingType.Hidden, Value = false };
                this["Window"]["Settings"] = new SwagSettingGroup() { SettingType = SettingType.Hidden };
                this["Window"]["Settings"]["IsOpen"] = new SwagSettingBoolean() { Value = false, SettingType = SettingType.Hidden, CanUndo = false };
                this["Window"]["CommandHistory"] = new SwagSettingGroup() { SettingType = SettingType.Hidden };
                this["Window"]["CommandHistory"]["IsOpen"] = new SwagSettingBoolean() { Value = false, SettingType = SettingType.Hidden, CanUndo = false };
                this["Window"]["Theme"] = new SwagSettingGroup() { SettingType = SettingType.SettingGroup, Icon = PackIconMaterialKind.PaletteOutline };
                this["Window"]["Theme"]["Base"] = new SwagSettingString() { Value = "Light", Icon = PackIconMaterialKind.PaletteSwatchOutline, ItemsSource = new[] { "Light", "Dark" }, SettingType = SettingType.DropDown };
                this["Window"]["Theme"]["Accent"] = new SwagSettingString() { Value = "Blue", Icon = PackIconMaterialKind.Brush, ItemsSource = new[] { "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna" }, SettingType = SettingType.DropDown };
            }
        }

        public void Initialize()
        {
            this["Window"]["Theme"]["Base"].PropertyChanged += WindowSettingCollection_ThemePropertyChanged;
            this["Window"]["Theme"]["Accent"].PropertyChanged += WindowSettingCollection_ThemePropertyChanged;
            WindowSettingCollection_ThemePropertyChanged(this, new PropertyChangedEventArgs("Value"));
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _childrenCollectionViewSource.View.Refresh();
        }
        #endregion Initialization

        #region Methods

        public Theme GetCurrentTheme()
        {
            String myBase = this["Window"]["Theme"]["Base"].ObjValue.ToString();
            String myAccent = this["Window"]["Theme"]["Accent"].ObjValue.ToString();

            Theme theme = ThemeManager.Current.GetTheme($"{myBase}.{myAccent}");
            return theme;
        }

        private void WindowSettingCollection_ThemePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                Theme theme = GetCurrentTheme();
                ThemeManager.Current.ChangeTheme(this, Application.Current.MainWindow.Resources, theme);

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
        #endregion Methods
    }

}
