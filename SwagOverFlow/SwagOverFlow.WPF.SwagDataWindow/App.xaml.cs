using SwagOverFlow.Logger;
using SwagOverFlow.WPF.UI;
using SwagOverFlow.Utils;
using System.Windows;

namespace SwagOverFlow.WPF.SwagDataWindow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            SwagLogger.Log(e.Exception, e.Exception.Message);

#if !DEBUG
            UIHelper.StringInputDialog(e.Exception.DeepMessage());
#endif

            e.Handled = true;
        }
    }
}
