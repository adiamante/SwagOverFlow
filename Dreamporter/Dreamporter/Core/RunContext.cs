using Dreamporter.Caching;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Utils;
using SwagOverFlow.Clients;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Web;

namespace Dreamporter.Core
{
    public class RunContext
    {
        #region Private Members
        SqliteClient _main = new SqliteClient();
        //https://stackoverflow.com/questions/29411961/c-sharp-and-thread-safety-of-a-bool
        private int _inErrorStateBackValue = 0;
        private int _inAbortStateBackValue = 0;
        private int _inDebugModeStateBackValue = 0;
        ConcurrentDictionary<String, DataContext> _dataContexts = new ConcurrentDictionary<String, DataContext>();
        ConcurrentDictionary<String, SqlClient> _sqlConnections = new ConcurrentDictionary<String, SqlClient>();
        #endregion Private Members

        #region Delegate/Events
        //https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/event
        public delegate void LogKvp(object sender, ICollection<KeyValuePair<String, String>> keyValuePairs);
        public event LogKvp LogKvpEvent;
        public delegate void LogErrorKvp(object sender, ICollection<KeyValuePair<String, String>> keyValuePairs);
        public event LogErrorKvp LogErrorKvpEvent;
        public delegate void LogWarningKvp(object sender, ICollection<KeyValuePair<String, String>> keyValuePairs);
        public event LogWarningKvp LogWarningKvpEvent;
        #endregion Delegate/Events

        #region Properties
        #region CacheProvider
        public IResultCacheProvider CacheProvider { get; set; } = new SqliteFileCacheProvider();
        #endregion CacheProvider
        #region InErrorState
        public Boolean InErrorState
        {
            get { return (Interlocked.CompareExchange(ref _inErrorStateBackValue, 1, 1) == 1); }
            set
            {
                if (value) Interlocked.CompareExchange(ref _inErrorStateBackValue, 1, 0);
                else Interlocked.CompareExchange(ref _inErrorStateBackValue, 0, 1);
            }
        }
        #endregion InErrorState
        #region InAbortState
        public Boolean InAbortState
        {
            get { return (Interlocked.CompareExchange(ref _inAbortStateBackValue, 1, 1) == 1); }
            set
            {
                if (value) Interlocked.CompareExchange(ref _inAbortStateBackValue, 1, 0);
                else Interlocked.CompareExchange(ref _inAbortStateBackValue, 0, 1);
            }
        }
        #endregion InAbortState
        #region InDebugMode
        public Boolean InDebugMode
        {
            get { return (Interlocked.CompareExchange(ref _inDebugModeStateBackValue, 1, 1) == 1); }
            set
            {
                if (value) Interlocked.CompareExchange(ref _inDebugModeStateBackValue, 1, 0);
                else Interlocked.CompareExchange(ref _inDebugModeStateBackValue, 0, 1);
            }
        }
        #endregion InDebugMode
        #endregion Properties

        #region Methods

        #region DataContext
        public void InitDataContexts(ICollection<DataContext> dataContexts)
        {
            foreach (DataContext dc in dataContexts)
            {
                _dataContexts.SafeSet(dc.Name, dc);
            }
        }
        #endregion DataContext

        #region Main SQLite DB Methods
        public void Open()
        {
            _main.OpenConnection();
            InitUtil();
        }

        public void Open(String connectionString)
        {
            _main = new SqliteClient(connectionString);
            _main.OpenConnection();
            //InitUtil();       //Skip this for now because util.numbers values will be generated again
        }

        public void OpenFileToMemory(String connectionString)
        {
            SqliteClient fileClient = new SqliteClient(connectionString);
            fileClient.OpenConnection();
            _main.OpenConnection();
            fileClient.BackupDatabaseToClient(_main);
            fileClient.CloseConnection();
        }

        private void InitUtil()
        {
            #region Utility Table: util.numbers with column n
            _main.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS [util.log](timestamp datetime, pid number, tid number, type text, name text, message text, info text);");
            _main.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS [util.numbers](n integer primary key);
                WITH RECURSIVE
                    cnt(x) AS(
                     SELECT -100
                     UNION ALL
                     SELECT x + 1 FROM cnt
                      LIMIT 201
                    )
                INSERT INTO [util.numbers](n)
                SELECT x
                FROM cnt;"
            );
            #endregion Utility Table: util.numbers with column n
        }

        public void Close()
        {
            _main.CloseConnection();
            foreach (KeyValuePair<String, SqlClient> kvp in _sqlConnections)
            {
                kvp.Value.Close();
            }
        }

        public void ExecuteNonQuery(String query)
        {
            _main.ExecuteNonQuery(query);
        }

        public DataTable Query(String query)
        {
            return _main.ExecuteReader(query);
        }

        public void AddTables(params DataTable[] tables)
        {
            //if (InDebugMode)
            //{
            //    foreach (DataTable dtbl in tables)
            //    {
            //        if (!dtbl.TableName.StartsWith("debug."))
            //        {
            //            dtbl.TableName = $"debug.{dtbl.TableName}";
            //        }
            //    }
            //}
            _main.InsertToTable(tables);
        }

        public DataSet GetDataSet()
        {
            DataSet ds = new DataSet();
            foreach (DataTable dt in _main.GetDataSet().Tables)
            {
                //if (dt.Rows.Count > 0)
                //{
                    ds.Tables.Add(dt.Copy());
                //}
            }

            return ds;
        }

        public void ExportDB(String dbFile)
        {
            try
            {
                String dir = Path.GetDirectoryName(dbFile);

                if (dir != dbFile && !Directory.Exists(dir))    //Not file and directory does not exist 
                {
                    Directory.CreateDirectory(dir);
                }

                if (File.Exists(dbFile))
                {
                    File.Delete(dbFile);
                }
                _main.BackupDatabaseToFile(dbFile);
            }
            catch { }
        }

        public Int32 GetNumRows(String query)
        {
            return _main.GetNumRows(query);
        }
        #endregion Main SQLite DB Methods

        #region SqlConnections
        public SqlClient GetSqlConnection(String connectionName, RunParams rp)
        {
            if (!_sqlConnections.ContainsKey(connectionName))
            {
                if (_dataContexts.ContainsKey(connectionName))
                {
                    if (_dataContexts[connectionName] is SqlConnectionDataContext sqlDataContext)
                    {
                        AddSqlConnection(sqlDataContext.Name, rp.ApplyParams(sqlDataContext.ConnectionString));
                    }
                    else
                    {
                        throw new Exception("SQL DataContext not found");
                    }
                }
                else
                {
                    throw new Exception("SQL DataContext not found");
                }
            }

            return _sqlConnections[connectionName];
        }

        public void AddSqlConnection(String connectionName, String connectionString)
        {
            SqlClient sqlClient = new SqlClient(connectionString);
            _sqlConnections.AddOrUpdate(connectionName, sqlClient, (key, oldVal) => sqlClient);
            sqlClient.Open();
        }
        #endregion SqlConnections

        #region Debug
        public JObject GetDebugObject()
        {
            JObject jObjSql = new JObject();

            DataTable dtTables = _main.ExecuteReader(
                @"SELECT CASE WHEN instr(name, '.') = 0 THEN 'main' ELSE substr(name, 0, instr(name, '.')) END AS schema, 
                         CASE WHEN instr(name, '.') = 0 THEN name ELSE substr(name, instr(name, '.') + 1) END AS name,
                         name AS fullName
                FROM sqlite_master WHERE type = 'table'");

            DataTable dtSchemas = dtTables.DefaultView.ToTable(true, "schema");

            //Schema
            //-SQL
            //-Tables
            //--SQL
            //--Response

            foreach (DataRow drSchema in dtSchemas.Rows)
            {
                String schema = drSchema["schema"].ToString();
                JObject jSchema = new JObject();
                jObjSql[$"{schema}"] = jSchema;
                jSchema["SQL"] = SqlHelper.GenerateSchema(schema);

                //if (_requestInfo.ContainsKey(schema))
                //{
                //    jSchema["RequestInfo"] = _requestInfo[$"{schema}"];
                //}
                //if (_responseInfo.ContainsKey(schema))
                //{
                //    jSchema["ResponseInfo"] = _responseInfo[$"{schema}"];
                //}

                //if (_responseJson.ContainsKey(schema))
                //{
                //    jSchema["ResponseJson"] = _responseJson[$"{schema}"];
                //}

                foreach (DataRow drTable in dtTables.Select($"[schema] = '{schema}'"))
                {
                    String tableName = drTable["fullName"].ToString();
                    DataTable dt = _main.ExecuteReader($"Select * FROM [{tableName}]", tableName);
                    JObject jTable = new JObject();
                    jSchema[dt.TableName] = jTable;
                    jTable["SQL"] = HttpUtility.JavaScriptStringEncode(SqlHelper.GenerateTable(dt)) + Environment.NewLine + HttpUtility.JavaScriptStringEncode(SqlHelper.GenerateTableInsert(dt));
                    jTable["Json"] = JsonConvert.SerializeObject(dt, Formatting.Indented);
                }
            }

            return jObjSql;
        }
        #endregion Debug

        #region Logging
        public void Log(String name, String message, String info)
        {
            LogFinal(name, "Normal", message, info);
        }

        public void LogError(String name, String message, String info)
        {
            LogFinal(name, "Error", message, info);
        }

        public void LogWarning(String name, String message, String info)
        {
            LogFinal(name, "Warning", message, info);
        }

        private void LogFinal(String name, String type, String message, String info)
        {
            String timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");
            Int32 pid = Process.GetCurrentProcess().Id;
            Int32 tid = Thread.CurrentThread.ManagedThreadId;
            _main.ExecuteNonQuery($"INSERT INTO [util.log] (timestamp, pid, tid, type, name, message, info) VALUES('{timeStamp}', {pid}, {tid}, '{type}', '{name}', '{message}', '{info}');");
            Dictionary<String, String> dictValues = new Dictionary<String, String> {
                { "TimeStamp", timeStamp },
                { "PID" , pid.ToString() },
                { "TID" , tid.ToString() },
                { "Type" , type },
                { "Name" , name },
                { "Message" , message },
                { "Info" , info }
            };

            //Invoke events if they were registered
            switch (type)
            {
                default:
                case "Normal":
                    LogKvpEvent?.Invoke(this, dictValues);
                    break;
                case "Error":
                    LogErrorKvpEvent?.Invoke(this, dictValues);
                    break;
                case "Warning":
                    LogWarningKvpEvent?.Invoke(this, dictValues);
                    break;
            }
        }
        #endregion Logging

        #endregion Methods
    }
}
