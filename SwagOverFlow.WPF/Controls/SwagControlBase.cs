using ControlzEx.Theming;
using MahApps.Metro;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace SwagOverflow.WPF.Controls
{
    public class SwagControlBase : UserControl, INotifyPropertyChanged
    {
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

        #region Private Properties
        Theme _theme = ThemeManager.Current.GetTheme("Light.Blue");
        #endregion Private Properties

        #region Initialization
        public SwagControlBase()
        {
            this.Loaded += SwagControlBase_Loaded;
        }

        private void SwagControlBase_Loaded(object sender, RoutedEventArgs e)
        {
            SwagWindow.GlobalSettings["Window"]["Theme"]["Base"].PropertyChanged += SwagWindowThemPropertyChanged; ;
            SwagWindow.GlobalSettings["Window"]["Theme"]["Accent"].PropertyChanged += SwagWindowThemPropertyChanged;
            ApplyTheme();
        }
        #endregion Initialization

        #region Events
        private void SwagWindowThemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ApplyTheme();
        }
        #endregion Events

        #region Methods
        private void ApplyTheme()
        {
            Theme currentWindowTheme = SwagWindow.GlobalSettings.GetCurrentTheme();

            if (_theme.Name != currentWindowTheme.Name)
            {
                ThemeManager.Current.ChangeTheme(this, this.Resources, currentWindowTheme);
                _theme = currentWindowTheme;
            }
        }
        #endregion Methods
    }
}
