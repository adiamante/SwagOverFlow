using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace SwagOverFlow.Clients
{
    public class SqliteClient
    {
        #region Private Members
        SQLiteConnection _connection;
        #endregion Private Members

        #region Properties
        #endregion Properties

        #region Initialization
        public SqliteClient()
        {
            //https://www.sqlite.org/inmemorydb.html
            _connection = new SQLiteConnection($"Data Source=:memory:;Version=3;");
        }

        public SqliteClient(String connectionString)
        {
            //https://stackoverflow.com/questions/9173485/how-can-i-create-an-in-memory-sqlite-database
            //FullUri=file:memdb?mode=memory&cache=shared; => Named In Shared Memory Connection
            _connection = new SQLiteConnection(connectionString);
        }

        #endregion Initialization

        #region Events
        #endregion Events

        #region Methods

        #region Connections
        public void OpenConnection()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
                _connection.BindFunction(new RegExSQLiteFunction());
                _connection.BindFunction(new DateTimeZoneConvertSQLiteFunction());
            }
        }

        public void CloseConnection()
        {
            _connection.Close();
        }
        #endregion Connections

        #region AddTables
        public void AddTables(params DataTable[] tables)
        {
            foreach (DataTable dt in tables)
            {
                String tableCommand = SqliteHelper.GenerateTableDrop(dt);
                tableCommand += SqliteHelper.GenerateTable(dt);
                tableCommand += SqliteHelper.GenerateTableInsert(dt);
                SQLiteCommand generateTableCommand = new SQLiteCommand(tableCommand, _connection);
                generateTableCommand.ExecuteNonQuery();
                generateTableCommand.Dispose();
            }
        }
        #endregion AddTables

        #region InsertToTable
        public void InsertToTable(params DataTable[] tables)
        {
            DataTable dtTables = ExecuteReader("SELECT name FROM sqlite_master WHERE type = 'table'");
            List<String> existingTables = dtTables.AsEnumerable().Select(r => r["name"].ToString()).ToList();

            foreach (DataTable dt in tables)
            {

                if (!existingTables.Contains(dt.TableName))
                {
                    String tableCommand = SqliteHelper.GenerateTable(dt);
                    SQLiteCommand generateTableCommand = new SQLiteCommand(tableCommand, _connection);
                    generateTableCommand.ExecuteNonQuery();
                    generateTableCommand.Dispose();
                }
                else
                {
                    DataTable dtColumns = ExecuteReader($"PRAGMA table_info([{dt.TableName}]);");
                    List<String> existingColumns = dtColumns.AsEnumerable().Select(r => r["name"].ToString().ToUpper()).ToList();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (!existingColumns.Contains(dc.ColumnName.ToUpper()))
                        {
                            String dataType = "TEXT";

                            switch (dc.DataType.GetTypeCode())
                            {
                                case TypeCode.Int16:
                                case TypeCode.Int32:
                                    dataType = "INTEGER";
                                    break;
                                default:
                                    if (dc.DataType.IsNumericType())
                                    {
                                        dataType = "REAL";
                                    }
                                    break;
                            }

                            ExecuteNonQuery($"ALTER TABLE [{dt.TableName}] ADD [{dc.ColumnName}] {dataType}");
                        }
                    }
                }

                String tableInsertCommand = SqliteHelper.GenerateTableInsert(dt);
                SQLiteCommand generateTableInsertCommand = new SQLiteCommand(tableInsertCommand, _connection);
                generateTableInsertCommand.ExecuteNonQuery();
                generateTableInsertCommand.Dispose();
            }
        }
        #endregion InsertToTable

        #region ExecuteQuery
        public int ExecuteNonQuery(String query)
        {
            SQLiteCommand command = new SQLiteCommand(query, _connection);
            return command.ExecuteNonQuery();
        }

        public DataTable ExecuteReader(String query, String resultName = "Result")
        {
            DataTable dtResult = new DataTable();
            dtResult.TableName = resultName;
            SQLiteCommand command = new SQLiteCommand(query, _connection);

            try
            {
                SQLiteDataReader reader = command.ExecuteReader();
                command.Dispose();

                //dtResult.Load(reader);    //This has issues when reading DateTime columns

                Boolean success = false;
                //The first read will populate the columns
                success = reader.Read();

                for (int col = 0; col < reader.FieldCount; col++)
                {
                    String colname = reader.GetName(col);
                    Type colType = reader.GetFieldType(col);
                    if (colType == typeof(Double))
                    {
                        colType = typeof(Decimal);  //prevents scientific notation ToString()
                    }

                    if (!dtResult.Columns.Contains(colname))
                    {
                        dtResult.Columns.Add(colname, colType);
                    }
                }

                if (success)    //means there were rows
                {
                    do
                    {
                        DataRow dr = dtResult.NewRow();

                        for (int col = 0; col < reader.FieldCount; col++)
                        {
                            String colname = reader.GetName(col);
                            Type colType = reader.GetFieldType(col);

                            if (!reader.IsDBNull(col))
                            {
                                switch (colType.GetTypeCode())
                                {
                                    case TypeCode.DateTime:
                                        if (DateTimeOffset.TryParse(reader.GetString(col), out DateTimeOffset dateTimeOffset))
                                        {
                                            dr[colname] = dateTimeOffset.DateTime;
                                        }
                                        break;
                                    default:
                                        dr[colname] = reader[col];
                                        break;
                                }
                            }
                        }

                        dtResult.Rows.Add(dr);
                    } while (reader.Read());
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtResult;
        }
        #endregion ExecuteQuery

        public Int32 GetNumRows(String query)
        {
            DataTable dt = ExecuteReader(query);
            return dt.Rows.Count;
        }

        public Int32 GetNumTables()
        {
            return GetNumRows("SELECT name FROM sqlite_master WHERE type = 'table'");
        }

        public DataSet GetDataSet()
        {
            DataSet ds = new DataSet(_connection.Database);

            DataTable dtTables = ExecuteReader("SELECT name FROM sqlite_master WHERE type = 'table'");
            foreach (DataRow drTable in dtTables.Rows)
            {
                String tableName = drTable["name"].ToString();
                DataTable dtTable = ExecuteReader($"SELECT * FROM [{tableName}]", tableName);
                ds.Tables.Add(dtTable);
            }

            return ds;
        }

        public void BackupDatabase(string filePath)
        {
            //https://stackoverflow.com/questions/11383775/memory-stream-as-db/11385280
            SQLiteConnection fileConnection = new SQLiteConnection($"Data Source={filePath};Version=3;");
            fileConnection.Open();
            _connection.BackupDatabase(fileConnection, "main", "main", -1, null, 0);
            fileConnection.Close();
        }

        #endregion Methods
    }

    public static class SqliteHelper
    {
        #region IterateColumns
        private static void IterateColumns(DataTable dt, Action<DataColumn, String> action)
        {
            foreach (DataColumn dc in dt.Columns)
            {
                String colType = "";
                switch (dc.DataType.GetTypeCode())
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Single:
                        colType = "INTEGER";
                        break;
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                        colType = "REAL";
                        break;
                    case TypeCode.DateTime:
                        colType = "DATETIME";
                        break;
                    case TypeCode.String:
                    case TypeCode.Char:
                    default:
                        colType = "VARCHAR(255)";
                        break;
                }

                action(dc, colType);
            }
        }
        #endregion IterateColumns

        #region Generate SQL Command Strings
        public static String GenerateTable(DataTable dt)
        {
            if (dt.Columns.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                String tableName = dt.TableName == "" ? "Temp" : string.Format("{0}", dt.TableName);
                String columnsDeclare = "";

                IterateColumns(dt, new Action<DataColumn, string>((dc, colType) =>
                {
                    columnsDeclare += string.Format(",[{0}] {1}", dc.ColumnName, colType);
                }));

                columnsDeclare = columnsDeclare.TrimStart(','); //remove beginning comma
                sb.AppendFormat("CREATE TABLE [{0}] ({1});", tableName, columnsDeclare);

                return sb.ToString();
            }
            else
            {
                return "";
            }
        }

        public static String GenerateTableInsert(DataTable dt, Int32 RowsPerLine = Int32.MaxValue)
        {
            StringBuilder sb = new StringBuilder();
            String tableName = dt.TableName == "" ? "Temp" : string.Format("{0}", dt.TableName);
            String columnsInsert = "", allRowInserts = "";

            IterateColumns(dt, new Action<DataColumn, string>((dc, colType) =>
            {
                columnsInsert += string.Format(",[{0}]", dc.ColumnName);
            }));

            columnsInsert = columnsInsert.TrimStart(','); //remove beginning comma

            if (dt.Rows.Count > 0)
            {
                int count = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    if (count % 1000 == 0)
                    {
                        if (allRowInserts != "")
                        {
                            sb.Append(allRowInserts.TrimEnd(',') + ";");
                            allRowInserts = "";
                        }
                        sb.AppendFormat("\r\nINSERT INTO [{0}] ({1}) VALUES\r\n\t", tableName, columnsInsert);
                    }

                    String rowInsert = "";
                    foreach (DataColumn dc in dt.Columns)
                    {
                        String format = (",{0}");
                        String value = dr[dc.ColumnName].ToString().Replace("'", "''");

                        if (value == "" && dc.DataType.IsNumericType())   //blank numeric column
                        {
                            value = "NULL";
                        }
                        else if (value == "" && !dc.DataType.IsNumericType()) //blank nonumeric column
                        {
                            format = (",'{0}'");
                        }
                        else if (dc.DataType.GetTypeCode() == TypeCode.DateTime)
                        {
                            DateTime myDate;
                            if (DateTime.TryParse(value, out myDate))
                            {
                                value = myDate.ToString("yyyy-MM-dd HH:mm:ss.ffff");
                            }
                            format = (",'{0}'");
                        }
                        else if (!dc.DataType.IsNumericType())
                        {
                            format = (",'{0}'");
                        }

                        rowInsert += string.Format(format, value);
                    }
                    rowInsert = rowInsert.Substring(1);
                    String rowInsertFormat = "({0}),";
                    if (count != 0 && count % RowsPerLine == 0)
                    {
                        rowInsertFormat = "\r\n\t({0}),";
                    }
                    allRowInserts += string.Format(rowInsertFormat, rowInsert);

                    count++;
                }
            }

            sb.Append(allRowInserts.TrimEnd(','));

            return sb.ToString();
        }

        public static String GenerateTableTruncate(DataTable dt)
        {
            return $"DELETE FROM [{dt.TableName}];\n";
        }

        public static String GenerateTableDrop(DataTable dt)
        {
            return $"DROP TABLE IF EXISTS {dt.TableName};\n";
        }
        #endregion Generate SQL Command Strings

        #region BindFunction
        public static void BindFunction(this SQLiteConnection connection, SQLiteFunction function)
        {
            var attributes = function.GetType().GetCustomAttributes(typeof(SQLiteFunctionAttribute), true).Cast<SQLiteFunctionAttribute>().ToArray();
            if (attributes.Length == 0)
            {
                throw new InvalidOperationException("SQLiteFunction doesn't have SQLiteFunctionAttribute");
            }
            connection.BindFunction(attributes[0], function);
        }
        #endregion BindFunction
    }

    #region SQLiteFunctions
    //https://stackoverflow.com/questions/24229785/sqlite-net-sqlitefunction-not-working-in-linq-to-sql/26155359#26155359
    // from https://stackoverflow.com/questions/172735/create-use-user-defined-functions-in-system-data-sqlite
    // taken from http://sqlite.phxsoftware.com/forums/p/348/1457.aspx#1457
    [SQLiteFunction(Name = "REGEXP", Arguments = 2, FuncType = FunctionType.Scalar)]
    public class RegExSQLiteFunction : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(Convert.ToString(args[1]), Convert.ToString(args[0]));
        }
    }

    [SQLiteFunction(Name = "DateTZConvert", Arguments = 3, FuncType = FunctionType.Scalar)]
    public class DateTimeZoneConvertSQLiteFunction : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            //First argument is the source date (expecing offset information from source date)
            DateTime dt = DateTime.Parse(args[0].ToString());

            //Second argument is the time zone
            String timeZone = args[1].ToString();
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById($"{timeZone} Standard Time");
            DateTime utcDateTime = dt.ToUniversalTime();
            DateTime convertedDate = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZoneInfo);
            DateTimeOffset dateTimeOffset = new DateTimeOffset(utcDateTime, TimeSpan.Zero);

            if (timeZoneInfo.IsDaylightSavingTime(convertedDate))
            {
                dateTimeOffset = dateTimeOffset.ToOffset(timeZoneInfo.BaseUtcOffset.Add(TimeSpan.FromHours(1)));

            }
            else
            {
                dateTimeOffset = dateTimeOffset.ToOffset(timeZoneInfo.BaseUtcOffset);
            }

            //Third argument is the format. Ex. yyyy-mm-ddTHH:mm:sss.fffzzz
            String format = args[2].ToString();

            return dateTimeOffset.ToString(format);
        }
    }
    #endregion SQLiteFunctions
}
