using MahApps.Metro.IconPacks;
using SwagOverFlow.Clients;
using SwagOverFlow.ViewModels;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.FTP.Core;
using SwagOverFlow.WPF.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SwagOverFlow.WPF.FTP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FTPWindow : SwagWindow
    {
        public CredentialsViewModel Credentials
        {
            get { return SwagWindow.GlobalSettings["FTP"]["Credentials"].GetValue<CredentialsViewModel>(); }
        }

        public FTPWindow()
        {
            InitializeSettings();
            InitializeComponent();
        }

        public void InitializeSettings()
        {
            SwagWindow.GlobalSettings.TryAddChildSetting("FTP", new SwagSettingGroup() { Icon = PackIconMaterialKind.AlphaFBox });
            SwagSetting<CredentialsViewModel> ssCredentials =
            new SwagSetting<CredentialsViewModel>()
            {
                Icon = PackIconCustomKind.ArrowMultipleSweepRight,
                Value = new CredentialsViewModel()
            };
            SwagWindow.GlobalSettings["FTP"].TryAddChildSetting("Credentials", ssCredentials);
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (btnConnect.Content.ToString() == "Connect")
            {
                Credentials.Connect();
                btnConnect.Content = "Disconnect";
            }
            else
            {
                Credentials.Disconnect();
                btnConnect.Content = "Connect";
            }
        }

        private void List_Click(object sender, RoutedEventArgs e)
        {
            var list = Credentials.FtpClient.GetFileList();

            txtList.Text = "";
            foreach (String file in list)
            {
                txtList.Text += file + Environment.NewLine;
            }
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            String remoteFile = txtRemoteFile.Text;
            String fileName = System.IO.Path.GetFileName(remoteFile);
            String localPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            Credentials.FtpClient.DownloadFile(localPath, remoteFile);
        }
    }
}
