using ControlzEx.Theming;
using MahApps.Metro.Controls;
using SwagOverFlow.Logger;
using SwagOverFlow.WPF.Commands;
using SwagOverFlow.WPF.Services;
using SwagOverFlow.WPF.UI;
using SwagOverFlow.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace SwagOverFlow.WPF.Controls
{
    public class SwagWindow : MetroWindow, INotifyPropertyChanged
    {
        static SwagWindowSettingGroup _settings;
        static SwagCommandManager _swagCommandManager;
        static SwagWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwagWindow), new FrameworkPropertyMetadata(typeof(SwagWindow)));

            SwagWPFContainer.Context.Database.EnsureCreated();
            String settingGoupName = $"{Assembly.GetEntryAssembly().GetName().Name}_Settings";
            _settings = SwagWPFContainer.SettingsService.GetWindowSettingGroupByName(settingGoupName);
            _swagCommandManager = new SwagCommandManager();
        }

        #region Settings
        private static readonly DependencyProperty SettingsProperty =
            DependencyProperty.Register("Settings", typeof(SwagWindowSettingGroup), typeof(SwagWindow), new PropertyMetadata(_settings));

        public SwagWindowSettingGroup Settings
        {
            get 
            {
                SwagWindowSettingGroup settings = (SwagWindowSettingGroup)GetValue(SettingsProperty);
                if (settings == null)
                {
                    SetValue(SettingsProperty, _settings);
                    settings = _settings;
                }
                return settings; 
            }
            set { SetValue(SettingsProperty, value); }
        }

        public static SwagWindowSettingGroup GlobalSettings => _settings;
        #endregion Settings

        #region SettingCustomTemplates
        public static readonly DependencyProperty SettingCustomTemplatesProperty =
            DependencyProperty.Register("SettingCustomTemplates", typeof(SwagTemplateCollection), typeof(SwagWindow),
            new FrameworkPropertyMetadata(new SwagTemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public SwagTemplateCollection SettingCustomTemplates
        {
            get { return (SwagTemplateCollection)GetValue(SettingCustomTemplatesProperty); }
            set { SetValue(SettingCustomTemplatesProperty, value); }
        }
        #endregion SettingCustomTemplates

        #region StatusMessage
        public String StatusMessage
        {
            get { return Settings["Window"]["Status"]["Message"].GetValue<String>(); }
            set 
            {
                Settings["Window"]["Status"]["Message"].SetValue(value);
                OnPropertyChanged();
            }
        }
        #endregion StatusMessage

        #region IsBusy
        public static Boolean GlobalIsBusy
        {
            get { return GlobalSettings["Window"]["Status"]["IsBusy"].GetValue<Boolean>(); }
            set { GlobalSettings["Window"]["Status"]["IsBusy"].SetValue(value); }
        }
        #endregion IsBusy

        #region IsBusy
        public Boolean IsBusy
        {
            get { return Settings["Window"]["Status"]["IsBusy"].GetValue<Boolean>(); }
            set
            {
                Settings["Window"]["Status"]["IsBusy"].SetValue(value);
                OnPropertyChanged();
            }
        }
        #endregion IsBusy

        #region CommandManager
        public static SwagCommandManager CommandManager => _swagCommandManager;
        //private static readonly DependencyProperty CommandManagerProperty =
        //    DependencyProperty.Register("CommandManager", typeof(SwagCommandManager), typeof(SwagWindow),
        //        new PropertyMetadata(new SwagCommandManager()));

        //public SwagCommandManager CommandManager
        //{
        //    get { return (SwagCommandManager)GetValue(CommandManagerProperty); }
        //    set { SetValue(CommandManagerProperty, value); }
        //}
        #endregion CommandManager

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyname = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return;

            backingField = value;
            OnPropertyChanged(propertyname);
        }
        #endregion INotifyPropertyChanged

        #region Initialization

        public SwagWindow()
        {
            Loaded += SwagWindow_Loaded;

            ResourceDictionary rdSwagWindow = new ResourceDictionary();
            rdSwagWindow.Source = new Uri("/SwagOverFlow.WPF;component/Controls/SwagWindow.xaml", UriKind.RelativeOrAbsolute);
            this.Resources.MergedDictionaries.Add(rdSwagWindow);
        }

        private void SwagWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings != null)
            {
                Settings["Window"]["Theme"]["Base"].PropertyChanged += WindowSettingCollection_ThemePropertyChanged;
                Settings["Window"]["Theme"]["Accent"].PropertyChanged += WindowSettingCollection_ThemePropertyChanged;
                Settings["Window"]["Status"]["IsBusy"].PropertyChanged += WindowSettingCollection_StatusIsBusyPropertyChanged;
                WindowSettingCollection_ThemePropertyChanged(this, new PropertyChangedEventArgs("Value"));

                CommandManager.Attach(Settings);

                InputBindings.Add(new KeyBinding() { Modifiers = ModifierKeys.Control, Key = Key.Z, Command = CommandManager.UndoCommand });
                InputBindings.Add(new KeyBinding() { Modifiers = ModifierKeys.Control, Key = Key.Y, Command = CommandManager.RedoCommand });

                SwagLogger.SwagSinkEvent += (sse) =>
                {
                    Settings["Window"]["Status"]["Message"].SetValue(sse.Message);
                };
            }

            SettingsControl settingsControl = this.FindLogicalChild<SettingsControl>();
            settingsControl.Save += SwagWindowSettings_Save;
        }

        private void WindowSettingCollection_StatusIsBusyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("IsBusy");
        }

        protected virtual void SwagWindowSettings_Save(object sender, RoutedEventArgs e)
        {
            _settings["Window"]["Settings"]["IsOpen"].SetValue(false);
            _settings["Window"]["CommandHistory"]["IsOpen"].SetValue(false);
            SwagWPFContainer.Context.SaveChanges();
        }
        #endregion Initialization

        #region Events
        private void WindowSettingCollection_ThemePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                Theme theme = Settings.GetCurrentTheme();

                #region Add Custom Brushes
                //Available Resources https://mahapps.com/docs/themes/thememanager
                String strGradientXaml =
                    @"<LinearGradientBrush xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                            xmlns:options=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
                            EndPoint =""0.5,1"" StartPoint=""0.5,0""
                            options:Freeze=""True"">
                        <GradientStop Color=""{DynamicResource MahApps.Colors.Accent}"" Offset=""0""/>
                        <GradientStop Color=""{DynamicResource MahApps.Colors.Accent2}"" Offset=""0.01""/>
                        <GradientStop Color=""{DynamicResource MahApps.Colors.Accent3}"" Offset=""0.2""/>
                        <GradientStop Color=""{DynamicResource MahApps.Colors.ThemeBackground}"" Offset=""0.21""/>
                    </LinearGradientBrush>";
                XmlReader xmlReader = XmlReader.Create(new StringReader(strGradientXaml));
                LinearGradientBrush linearGradientBrush = (LinearGradientBrush)XamlReader.Load(xmlReader);

                //theme.Resources["MahApps.Brushes.ThemeBackground"] = linearGradientBrush;
                theme.Resources["MahApps.Brushes.Control.Background"] = linearGradientBrush;
                //theme.Resources["MahApps.Brushes.Window.Background"] = linearGradientBrush;
                theme.Resources["MahApps.Brushes.Menu.Background"] = linearGradientBrush;
                theme.Resources["MahApps.Brushes.ContextMenu.Background"] = linearGradientBrush;
                theme.Resources["MahApps.Brushes.SubMenu.Background"] = linearGradientBrush;
                theme.Resources["MahApps.Brushes.MenuItem.Background"] = linearGradientBrush;
                #endregion Add Custom Brushes

                ThemeManager.Current.ChangeTheme(this, this.Resources, theme);

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

        public async Task RunInBackground(Action action)
        {
            IsBusy = true;
            await Task.Run(action);
            IsBusy = false;
        }
        #endregion Events
    }
}
