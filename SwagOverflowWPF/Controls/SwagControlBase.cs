using MahApps.Metro;
using System.Windows;
using System.Windows.Controls;

namespace SwagOverflowWPF.Controls
{
    public class SwagControlBase : UserControl
    {
        Theme _theme = ThemeManager.GetTheme("Light.Blue");
        SwagWindow _swagWindow = null;

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

        private void SwagWindowThemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ApplyTheme();
        }

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
    }
}
