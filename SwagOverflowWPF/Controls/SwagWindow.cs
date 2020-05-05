using MahApps.Metro.Controls;
using SwagOverFlow.Logger;
using SwagOverflowWPF.Commands;
using SwagOverflowWPF.Services;
using SwagOverflowWPF.UI;
using SwagOverflowWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SwagOverflowWPF.Controls
{
    public class SwagWindow : MetroWindow, INotifyPropertyChanged
    {
        static SwagWindowSettingGroup _settings;
        static SwagCommandManager _swagCommandManager;
        static SwagWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwagWindow), new FrameworkPropertyMetadata(typeof(SwagWindow)));

            SwagWPFServices.Context.Database.EnsureCreated();
            String settingGoupName = $"{Assembly.GetEntryAssembly().GetName().Name}_Settings";
            _settings = SwagWPFServices.SettingsService.GetWindowSettingGroupByName(settingGoupName);
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
            rdSwagWindow.Source = new Uri("/SwagOverflowWPF;component/Controls/SwagWindow.xaml", UriKind.RelativeOrAbsolute);
            this.Resources.MergedDictionaries.Add(rdSwagWindow);
        }

        private void SwagWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings != null)
            {
                Settings.Initialize();
                CommandManager.Attach(Settings);

                InputBindings.Add(new KeyBinding() { Modifiers = ModifierKeys.Control, Key = Key.Z, Command = CommandManager.UndoCommand });
                InputBindings.Add(new KeyBinding() { Modifiers = ModifierKeys.Control, Key = Key.Y, Command = CommandManager.RedoCommand });

                SwagLogger.SwagSinkEvent += (sse) =>
                {
                    Settings["Window"]["Status"]["Message"].SetValue(sse.Message);
                };
            }
        }
        #endregion Initialization

        #region Events
        public async Task RunInBackground(Action action)
        {
            IsBusy = true;
            await Task.Run(action);
            IsBusy = false;
        }
        #endregion Events
    }
}
