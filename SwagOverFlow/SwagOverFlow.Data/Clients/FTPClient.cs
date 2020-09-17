using Newtonsoft.Json.Linq;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace SwagOverFlow.Data.Clients
{
    #region FtpType
    public enum FtpType
    {
        PlainFtp,        //uses FluentFTP nuget package https://github.com/robinrodricks/FluentFTP#faq_sftp
        Sftp             //uses SSH.NET nuget package https://github.com/sshnet/SSH.NET
    }
    #endregion FtpType

    public abstract class FtpClient : ViewModelBase
    {
        #region Private/Protected Members
        protected String _host = "", _userName = "", _password = "";
        #endregion Private/Protected Members

        #region Properties
        public abstract String Host { get; set; }
        public abstract String UserName { get; set; }
        public abstract String Password { get; set; }
        public abstract String RemoteDirectory { get; set; }
        public abstract Boolean IsConnected { get; }
        public abstract Int32 Port { get; set; }
        #endregion Properties

        #region Initialization
        protected FtpClient()
        {
            _host = "";
            _userName = "";
            _password = "";
        }

        protected FtpClient(string host, string userName, string password)
        {
            _host = host;
            _userName = userName;
            _password = password;
        }
        #endregion Initialization

        #region Methods
        public abstract void Connect();
        public abstract void Disconnect();
        public abstract IEnumerable<String> GetFileListFullPath(string folder = "");
        public abstract IEnumerable<String> GetFileList(string folder = "");
        public abstract Boolean UploadFile(String localPath, String remoteDest);
        public abstract Boolean RenameFile(String remotePath, String remoteNewPath);
        public abstract Boolean DownloadFile(String localDest, String remotePath);
        public abstract Stream DownloadStream(String remotePath);
        public abstract Boolean DeleteFile(String remotePath);
        public abstract Boolean CreateDirectory(String remotePath);
        public abstract Boolean DeleteDirectory(String remotePath);
        public abstract Boolean FileExists(String remotePath);
        public abstract Boolean DirectoryExists(String remotePath);
        public abstract DateTime GetModifiedDateTime(String remotePath);
        public void ApplyJsonStringConfig(String json)
        {
            ApplyJsonConfig(JObject.Parse(json));
        }
        public void ApplyJsonConfig(JObject jApply)
        {
            Type classType = this.GetType();
            foreach (KeyValuePair<String, JToken> kvpApply in jApply)
            {
                PropertyInfo prop = classType.GetProperty(kvpApply.Key);
                Type destType = prop.PropertyType;
                TypeConverter converter = TypeDescriptor.GetConverter(destType);

                if (kvpApply.Value is JValue)
                {
                    JValue jVal = kvpApply.Value as JValue;
                    Type sourceType = jVal.Value.GetType();

                    if (converter.CanConvertFrom(sourceType))
                    {
                        prop.SetValue(this, converter.ConvertFrom(jVal.Value));
                    }
                }
            }
        }
        #endregion Methods
    }
}
