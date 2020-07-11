using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Commands;
using SwagOverFlow.WPF.UI;
using System;
using System.Windows;
using System.Windows.Input;

namespace SwagOverFlow.WPF.Controls
{
    /// <summary>
    /// Interaction logic for WindowSettings.xaml
    /// </summary>
    public partial class SettingsControl : SwagControlBase
    {
        ICommand _saveCommand;

        #region Settings
        private static readonly DependencyProperty SettingsProperty =
        DependencyProperty.Register("Settings", typeof(SwagSettingGroup), typeof(SettingsControl));

        public SwagSettingGroup Settings
        {
            get { return (SwagSettingGroup)GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }
        #endregion Settings
        #region Save
        public static readonly RoutedEvent SaveEvent =
            EventManager.RegisterRoutedEvent(
            "Save",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SettingsControl));

        public event RoutedEventHandler Save
        {
            add { AddHandler(SaveEvent, value); }
            remove { RemoveHandler(SaveEvent, value); }
        }
        #endregion Save
        #region SaveCommand
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ??
                    (_saveCommand = new RelayCommand<object>((s) =>
                    {
                        RaiseEvent(new RoutedEventArgs(SaveEvent, s ?? this));
                    }));
            }
        }
        #endregion SaveCommand
        #region ShowSaveButton
        public static DependencyProperty ShowSaveButtonProperty =
            DependencyProperty.Register(
                "ShowSaveButton",
                typeof(Boolean),
                typeof(SettingsControl),
                new PropertyMetadata(false));

        public Boolean ShowSaveButton
        {
            get { return (Boolean)GetValue(ShowSaveButtonProperty); }
            set
            {
                SetValue(ShowSaveButtonProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowSaveButton
        #region SaveButtonVerticalAlignment
        public static DependencyProperty SaveButtonVerticalAlignmentProperty =
            DependencyProperty.Register(
                "SaveButtonVerticalAlignment",
                typeof(VerticalAlignment),
                typeof(SettingsControl));
        public VerticalAlignment SaveButtonVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(SaveButtonVerticalAlignmentProperty); }
            set
            {
                SetValue(SaveButtonVerticalAlignmentProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion SaveButtonVerticalAlignment
        #region SaveButtonHorizontalAlignment
        public static DependencyProperty SaveButtonHorizontalAlignmentProperty =
            DependencyProperty.Register(
                "SaveButtonHorizontalAlignment",
                typeof(HorizontalAlignment),
                typeof(SettingsControl));

        public HorizontalAlignment SaveButtonHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(SaveButtonHorizontalAlignmentProperty); }
            set
            {
                SetValue(SaveButtonHorizontalAlignmentProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion SaveButtonHorizontalAlignment
        #region SettingCustomTemplates
        public static readonly DependencyProperty SettingCustomTemplatesProperty =
            DependencyProperty.Register("SettingCustomTemplates", typeof(SwagTemplateCollection), typeof(SettingsControl),
            new FrameworkPropertyMetadata(new SwagTemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public SwagTemplateCollection SettingCustomTemplates
        {
            get { return (SwagTemplateCollection)GetValue(SettingCustomTemplatesProperty); }
            set { SetValue(SettingCustomTemplatesProperty, value); }
        }
        #endregion SettingCustomTemplates

        public SettingsControl()
        {
            InitializeComponent();
        }
    }
}
