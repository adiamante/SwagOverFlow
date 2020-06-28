﻿using Dreamporter.Caching;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Clients;
using System;
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
        #endregion Properties

        #region Methods

        #region Main SQLite DB Methods
        public void Open()
        {
            _main.OpenConnection();
            //AddSchema("util");
            #region Utility Table: util.numbers with column n
            _main.ExecuteNonQuery(@"CREATE TABLE [util.numbers](n integer primary key);
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
            _main.ExecuteNonQuery(@"CREATE TABLE [util.log](timestamp datetime, pid number, tid number, type text, name text, message text, info text);");
            #endregion Utility Table: util.numbers with column n
        }

        public void Close()
        {
            _main.CloseConnection();
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
            _main.InsertToTable(tables);
        }

        public DataSet GetDataSet()
        {
            DataSet ds = new DataSet();
            foreach (DataTable dt in _main.GetDataSet().Tables)
            {
                if (dt.Rows.Count > 0)
                {
                    ds.Tables.Add(dt.Copy());
                }
            }

            return ds;
        }

        public void ExportDB(String dbFile)
        {
            try
            {
                String dir = Path.GetDirectoryName(dbFile);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                if (File.Exists(dbFile))
                {
                    File.Delete(dbFile);
                }
                _main.BackupDatabase(dbFile);
            }
            catch { }

        }

        public Int32 GetNumRows(String query)
        {
            return _main.GetNumRows(query);
        }
        #endregion Main SQLite DB Methods

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