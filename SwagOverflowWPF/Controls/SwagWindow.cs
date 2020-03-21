using MahApps.Metro.Controls;
using SwagOverflowWPF.Commands;
using SwagOverflowWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace SwagOverflowWPF.Controls
{
    public class SwagWindow : MetroWindow, INotifyPropertyChanged
    {
        #region Settings
        private static readonly DependencyProperty SettingsProperty =
            DependencyProperty.Register("Settings", typeof(SwagWindowSettingGroup), typeof(SwagWindow));

        public SwagWindowSettingGroup Settings
        {
            get { return (SwagWindowSettingGroup)GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }
        #endregion Settings

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
        private static readonly DependencyProperty CommandManagerProperty =
            DependencyProperty.Register("CommandManager", typeof(SwagCommandManager), typeof(SwagWindow),
                new PropertyMetadata(new SwagCommandManager()));

        public SwagCommandManager CommandManager
        {
            get { return (SwagCommandManager)GetValue(CommandManagerProperty); }
            set { SetValue(CommandManagerProperty, value); }
        }
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

        static SwagWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwagWindow), new FrameworkPropertyMetadata(typeof(SwagWindow)));
        }

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
                //Settings.Initialize();
                CommandManager.Attach(Settings);

                InputBindings.Add(new KeyBinding() { Modifiers = ModifierKeys.Control, Key = Key.Z, Command = CommandManager.UndoCommand });
                InputBindings.Add(new KeyBinding() { Modifiers = ModifierKeys.Control, Key = Key.Y, Command = CommandManager.RedoCommand });
            }
        }
    }
}
