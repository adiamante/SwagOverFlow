using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace SwagOverFlow.Data.Clients
{
    public class SftpClient : FtpClient
    {
        #region Private Members
        Renci.SshNet.SftpClient _client;
        String _remoteDirectory;
        Int32 _port = 22;
        #endregion Private Members

        #region Properties

        #region Host
        public override String Host
        {
            get { return _client.ConnectionInfo.Host; }
            set
            {
                SetValue(ref _host, value);
                Init();
            }
        }
        #endregion Host

        #region UserName
        public override String UserName
        {
            get { return _client.ConnectionInfo.Username; }
            set
            {
                SetValue(ref _userName, value);
                Init();
            }
        }
        #endregion UserName

        #region Password
        public override String Password
        {
            get { return _password; }
            set
            {
                SetValue(ref _password, value);
                Init();
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
                    return _client.WorkingDirectory;
                }
                else
                {
                    return _remoteDirectory;
                }
            }
            set
            {
                _client.ChangeDirectory(value);
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
            get { return _port; }
            set
            {
                SetValue(ref _port, value);
                Init();
            }
        }
        #endregion Port

        #endregion Properties

        #region Initialization
        public SftpClient()
        {
            Init();
        }

        public SftpClient(String url, String userName, String password) : base(url, userName, password)
        {
            Init();
        }

        public void Init()
        {
            _client?.Disconnect();
            _client?.Dispose();
            _client = new Renci.SshNet.SftpClient(_host, _port, _userName == "" ? "anonymous" : _userName, _password);
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
            if (!String.IsNullOrEmpty(RemoteDirectory))
            {
                folder = Path.Combine(RemoteDirectory, folder);
            }

            foreach (Renci.SshNet.Sftp.SftpFile entry in _client.ListDirectory(folder))
            {
                if (!entry.IsDirectory)
                {
                    yield return entry.FullName;
                }
            }
        }

        public override IEnumerable<string> GetFileList(string folder = "")
        {
            if (!String.IsNullOrEmpty(RemoteDirectory))
            {
                folder = Path.Combine(RemoteDirectory, folder);
            }

            foreach (Renci.SshNet.Sftp.SftpFile entry in _client.ListDirectory(folder))
            {
                if (!entry.IsDirectory)
                {
                    yield return entry.Name;
                }
            }
        }

        public override bool UploadFile(string localPath, string remoteDest)
        {
            using (FileStream file = File.OpenRead(localPath))
            {
                _client.UploadFile(file, remoteDest);
                return true;
            }
        }

        public override bool RenameFile(string remotePath, string remoteNewPath)
        {
            _client.RenameFile(remotePath, remoteNewPath);
            return true;
        }

        public override bool DownloadFile(string localDest, string remotePath)
        {
            using (FileStream file = File.OpenWrite(localDest))
            {
                _client.DownloadFile(remotePath, file);
                return true;
            }
        }

        public override Stream DownloadStream(string remotePath)
        {
            Stream stream = new MemoryStream();
            _client.DownloadFile(remotePath, stream);
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
            if (_client.Exists(remotePath))
            {
                Renci.SshNet.Sftp.SftpFile entry = _client.Get(remotePath);
                return !entry.IsDirectory;
            }

            return false;
        }

        public override bool DirectoryExists(string remotePath)
        {
            if (_client.Exists(remotePath))
            {
                Renci.SshNet.Sftp.SftpFile entry = _client.Get(remotePath);
                return entry.IsDirectory;
            }

            return false;
        }

        public override DateTime GetModifiedDateTime(string remotePath)
        {
            Renci.SshNet.Sftp.SftpFile entry = _client.Get(remotePath);
            return entry.LastWriteTime;     //Created Time is not implemented in SSH.NET so we're settling for WriteTime
        }
        #endregion Methods
    }
}
