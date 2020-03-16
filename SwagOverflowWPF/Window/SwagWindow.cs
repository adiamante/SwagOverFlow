using MahApps.Metro.Controls;
using SwagOverflowWPF.Commands;
using SwagOverflowWPF.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SwagOverflowWPF.Controls
{
    public class SwagWindow : MetroWindow
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

        static SwagWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwagWindow), new FrameworkPropertyMetadata(typeof(SwagWindow)));
        }

        public SwagWindow()
        {
            Loaded += SwagWindow_Loaded;
        }

        private void SwagWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings != null)
            {
                Settings.Initialize();
                CommandManager.Attach(Settings);

                InputBindings.Add(new KeyBinding() { Modifiers = ModifierKeys.Control, Key = Key.Z, Command = CommandManager.UndoCommand });
                InputBindings.Add(new KeyBinding() { Modifiers = ModifierKeys.Control, Key = Key.Y, Command = CommandManager.RedoCommand });
            }
        }
    }
}
