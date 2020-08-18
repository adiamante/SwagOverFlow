using SwagOverFlow.Data.Clients;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverFlow.WPF.FTP.Core
{
    public class CredentialsViewModel : ViewModelBase
    {
        FtpClient _ftpClient;
        String _host, _userName, _password, _remoteDirectory;
        Int32 _port;
        FtpType _ftpType;
        Boolean _isConnected;
        FtpEncryptionMode _ftpEncryptionMode = FtpEncryptionMode.None;

        #region FtpClient
        public FtpClient FtpClient
        {
            get { return _ftpClient; }
            set { SetValue(ref _ftpClient, value); }
        }
        #endregion FtpClient

        #region Host
        public String Host
        {
            get { return _host; }
            set { SetValue(ref _host, value); }
        }
        #endregion Host

        #region UserName
        public String UserName
        {
            get { return _userName; }
            set { SetValue(ref _userName, value); }
        }
        #endregion UserName

        #region Password
        public String Password
        {
            get { return _password; }
            set { SetValue(ref _password, value); }
        }
        #endregion Password

        #region RemoteDirectory
        public String RemoteDirectory
        {
            get { return _remoteDirectory; }
            set { SetValue(ref _remoteDirectory, value); }
        }
        #endregion RemoteDirectory

        #region Port
        public Int32 Port
        {
            get { return _port; }
            set { SetValue(ref _port, value); }
        }
        #endregion Port

        #region FtpType
        public FtpType FtpType
        {
            get { return _ftpType; }
            set { SetValue(ref _ftpType, value); }
        }
        #endregion FtpType

        #region IsConnected
        public bool IsConnected
        {
            get 
            {
                if (_ftpClient != null)
                {
                    return _ftpClient.IsConnected;
                }
                return false;
            }
        }


        #endregion IsConnected

        #region FtpEncryptionMode
        public FtpEncryptionMode FtpEncryptionMode
        {
            get { return _ftpEncryptionMode; }
            set { SetValue(ref _ftpEncryptionMode, value); }
        }
        #endregion FtpEncryptionMode

        #region Methods
        public void Connect()
        {
            switch (FtpType)
            {
                default:
                case FtpType.PlainFtp:
                    var client = new PlainFtpClient(Host, UserName, Password);
                    FtpClient = client;
                    client.EncryptionMode = FtpEncryptionMode;
                    
                    break;
                case FtpType.Sftp:
                    FtpClient = new SftpClient(Host, UserName, Password);
                    break;
            }

            FtpClient.Connect();
            FtpClient.RemoteDirectory = _remoteDirectory;
            OnPropertyChanged("IsConnected");
        }

        public void Disconnect()
        {
            if (FtpClient != null)
            {
                FtpClient.Disconnect();
            }
            OnPropertyChanged("IsConnected");
        }
        #endregion Methods
    }
}
