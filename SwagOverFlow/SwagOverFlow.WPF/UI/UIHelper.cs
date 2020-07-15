using SwagOverFlow.WPF.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SwagOverFlow.WPF.UI
{
    public static class UIHelper
    {
        public static string StringInputDialog(string message = "Please enter input:", String strDefault = "")
        {
            #region Window Setup

            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            FrameworkElement mainChild = mainWindow.Content as FrameworkElement;

            SwagWindow window = new SwagWindow();
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;

            #endregion Window Setup

            #region Content Setup

            DockPanel dockPanel = new DockPanel();

            TextBlock tbMessage = new TextBlock();
            tbMessage.Margin = new Thickness(5, 5, 5, 5);
            tbMessage.Text = message;
            DockPanel.SetDock(tbMessage, Dock.Top);

            TextBox txtInput = new TextBox();
            txtInput.Text = strDefault;
            txtInput.SelectAll();
            txtInput.Margin = new Thickness(5, 0, 5, 5);

            Grid gridButtons = new Grid();
            DockPanel.SetDock(gridButtons, Dock.Bottom);
            gridButtons.ColumnDefinitions.Add(new ColumnDefinition());
            gridButtons.ColumnDefinitions.Add(new ColumnDefinition());
            Button btnOK = new Button() { Content = "OK" };
            btnOK.Margin = new Thickness(5, 0, 5, 5);
            btnOK.Click += (s, e) =>
            {
                window.Close();
            };
            Grid.SetColumn(btnOK, 0);
            gridButtons.Children.Add(btnOK);
            Button btnCancel = new Button() { Content = "Cancel" };
            btnCancel.Margin = new Thickness(5, 0, 5, 5);
            btnCancel.Click += (s, e) =>
            {
                txtInput.Text = "";
                window.Close();
            };
            Grid.SetColumn(btnCancel, 1);
            gridButtons.Children.Add(btnCancel);

            txtInput.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Return)
                {
                    btnOK.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
                else if (e.Key == Key.Escape)
                {
                    btnCancel.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            };

            dockPanel.Children.Add(tbMessage);
            dockPanel.Children.Add(gridButtons);
            dockPanel.Children.Add(txtInput);

            window.Content = dockPanel;

            #endregion Content Setup

            #region Show Dialog

            window.Left = mainWindow.Left + (mainWindow.Width - window.ActualWidth) / 2;
            window.Top = mainWindow.Top + (mainWindow.Height - window.ActualHeight) / 2;
            txtInput.Focus();
            window.ShowDialog();

            #endregion Show Dialog

            return txtInput.Text;
        }
    }
}
