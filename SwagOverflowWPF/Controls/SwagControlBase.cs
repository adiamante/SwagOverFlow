using MahApps.Metro;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace SwagOverflowWPF.Controls
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
        Theme _theme = ThemeManager.GetTheme("Light.Blue");
        SwagWindow _swagWindow = null;
        #endregion Private Properties

        #region SwagWindow
        public SwagWindow SwagWindow
        {
            get { return _swagWindow; }
        }
        #endregion SwagWindow

        #region Initialization
        public SwagControlBase()
        {
            this.Loaded += SwagControlBase_Loaded;
        }

        private void SwagControlBase_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Application.Current.MainWindow;

            if (window is SwagWindow)
            {
                SwagWindow swagWindow = (SwagWindow)window;
                swagWindow.Settings["Window"]["Theme"]["Base"].PropertyChanged += SwagWindowThemPropertyChanged; ;
                swagWindow.Settings["Window"]["Theme"]["Accent"].PropertyChanged += SwagWindowThemPropertyChanged;
                _swagWindow = swagWindow;
                ApplyTheme();
            }
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
            if (_swagWindow != null)
            {
                Theme currentWindowTheme = _swagWindow.Settings.GetCurrentTheme();

                if (_theme.Name != currentWindowTheme.Name)
                {
                    ThemeManager.ChangeTheme(this.Resources, currentWindowTheme);
                    _theme = currentWindowTheme;
                }
            }
        }
        #endregion Methods
    }
}
