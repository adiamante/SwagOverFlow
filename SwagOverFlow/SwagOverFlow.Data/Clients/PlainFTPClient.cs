using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace SwagOverFlow.Data.Clients
{
    public enum FtpEncryptionMode
    {
        Explicit,
        Implicit,
        None
    }

    public class PlainFtpClient : FtpClient
    {
        #region Private Members
        FluentFTP.FtpClient _client;
        String _remoteDirectory;
        Int32 _port;
        FtpEncryptionMode _encryptionMode = FtpEncryptionMode.None;
        System.Security.Authentication.SslProtocols _sslProtocols = System.Security.Authentication.SslProtocols.None;
        #endregion Private Members

        #region Properties

        #region Host
        public override String Host
        {
            get { return _client.Host; }
            set
            {
                _client.Host = _host;
                SetValue(ref _host, value);
            }
        }
        #endregion Host

        #region UserName
        public override String UserName
        {
            get { return _client.Credentials.UserName; }
            set
            {
                _client.Credentials.UserName = _userName;
                SetValue(ref _userName, value);
            }
        }
        #endregion UserName

        #region Password
        public override String Password
        {
            get { return _client.Credentials.Password; }
            set
            {
                _client.Credentials.Password = _password;
                SetValue(ref _password, value);
            }
        }
        #endregion Password

        #region RemoteDirectory
        public override string RemoteDirectory
        {
            get
            {
                if (IsConnected)
                {
                    return _client.GetWorkingDirectory();
                }
                else
                {
                    return _remoteDirectory;
                }
            }
            set
            {
                _client.SetWorkingDirectory(value);
                SetValue(ref _remoteDirectory, value);
            }
        }
        #endregion RemoteDirectory

        #region IsConnected
        public override bool IsConnected { get { return _client.IsConnected; } }
        #endregion IsConnected

        #region Port
        public override Int32 Port
        {
            get { return _client.Port; }
            set
            {
                _client.Port = value;
                SetValue(ref _port, value);
            }
        }
        #endregion Port

        #region ExistsUsesListing
        public bool ExistsUsesListing { get; set; } = false;
        #endregion ExistsUsesListing

        #region EncryptionMode
        public FtpEncryptionMode EncryptionMode
        {
            get { return (FtpEncryptionMode)Enum.Parse(typeof(FtpEncryptionMode), _client.EncryptionMode.ToString()); }
            set
            {
                _client.EncryptionMode = (FluentFTP.FtpEncryptionMode)Enum.Parse(typeof(FluentFTP.FtpEncryptionMode), value.ToString());
                SetValue(ref _encryptionMode, value);
            }
        }
        #endregion EncryptionMode

        #region SslProtocols
        public System.Security.Authentication.SslProtocols SslProtocols
        {
            get { return _client.SslProtocols; }
            set
            {
                _client.SslProtocols = value;
                SetValue(ref _sslProtocols, value);
            }
        }
        #endregion SslProtocols

        #endregion Properties

        #region Initialization

        public PlainFtpClient() : base()
        {
            _client = new FluentFTP.FtpClient();
            _client.DataConnectionType = FluentFTP.FtpDataConnectionType.PASV;
        }

        public PlainFtpClient(string host, string userName, string password) : base(host, userName, password)
        {
            _client = new FluentFTP.FtpClient(host);
            if (!String.IsNullOrEmpty(_userName) && !String.IsNullOrEmpty(_password))
            {
                _client.Credentials = new System.Net.NetworkCredential(_userName, _password);
            }
        }

        #endregion Initialization

        #region Methods
        public override void Connect()
        {
            _client.Connect();
            OnPropertyChanged("IsConnected");
        }

        public override void Disconnect()
        {
            _client.Disconnect();
            OnPropertyChanged("IsConnected");
        }

        public override IEnumerable<string> GetFileListFullPath(string folder = "")
        {
            foreach (FluentFTP.FtpListItem item in _client.GetListing(folder))
            {
                if (item.Type == FluentFTP.FtpFileSystemObjectType.File)
                {
                    yield return item.FullName;
                }
            }
        }

        public override IEnumerable<string> GetFileList(string folder = "")
        {
            foreach (FluentFTP.FtpListItem item in _client.GetListing(folder))
            {
                if (item.Type == FluentFTP.FtpFileSystemObjectType.File)
                {
                    yield return item.Name;
                }
            }
        }

        public IEnumerable<string> GetDirectoryListFullPath(string folder = "")
        {
            foreach (FluentFTP.FtpListItem item in _client.GetListing(folder))
            {
                if (item.Type == FluentFTP.FtpFileSystemObjectType.Directory)
                {
                    yield return item.FullName;
                }
            }
        }

        public override Boolean UploadFile(string localPath, string remoteDest)
        {
            var ftpStatus = _client.UploadFile(localPath, remoteDest);
            return ftpStatus == FluentFTP.FtpStatus.Success;
        }

        public override bool RenameFile(string remotePath, string remoteNewPath)
        {
            _client.Rename(remotePath, remoteNewPath);
            return true;
        }

        public override bool DownloadFile(string localDest, string remotePath)
        {
            var ftpStatus = _client.DownloadFile(localDest, remotePath);
            return ftpStatus == FluentFTP.FtpStatus.Success;
        }

        public override Stream DownloadStream(string remotePath)
        {
            Stream stream = new MemoryStream();
            _client.Download(stream, remotePath);
            return stream;
        }

        public override bool DeleteFile(string remotePath)
        {
            _client.DeleteFile(remotePath);
            return true;
        }

        public override bool CreateDirectory(string remotePath)
        {
            _client.CreateDirectory(remotePath);
            return true;
        }

        public override bool DeleteDirectory(string remotePath)
        {
            _client.DeleteDirectory(remotePath);
            return true;
        }

        public override bool FileExists(string remotePath)
        {
            if (ExistsUsesListing)
            {
                return FileExistsFromList(remotePath);
            }
            else
            {
                return _client.FileExists(remotePath);
            }
        }

        public bool FileExistsFromList(string remotePath)
        {
            List<String> files = GetFileListFullPath(Path.GetDirectoryName(remotePath)).ToList();

            if (files.Contains($"/{Path.GetFileName(remotePath)}"))
            {
                return true;
            }

            return false;
        }

        public override bool DirectoryExists(string remotePath)
        {
            if (ExistsUsesListing)
            {
                return DirectoryExistsFromList(remotePath);
            }
            else
            {
                return _client.DirectoryExists(remotePath);
            }
        }

        public bool DirectoryExistsFromList(string remotePath)
        {
            List<String> directories = GetDirectoryListFullPath(Path.GetDirectoryName(remotePath)).ToList();

            if (directories.Contains($"/{remotePath}"))
            {
                return true;
            }

            return false;
        }

        public override DateTime GetModifiedDateTime(string remotePath)
        {
            FluentFTP.FtpListItem item = _client.GetObjectInfo(remotePath);
            return item.Modified;   //Created does not seem to work properyly during tests
        }

        #endregion Methods
    }
}
