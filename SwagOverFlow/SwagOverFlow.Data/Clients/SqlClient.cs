﻿using SwagOverFlow.Utils;
using System;
using System.Data;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace SwagOverFlow.Data.Clients
{
    public class SqlClient
    {
        #region Private Members
        SqlConnection _connection;
        #endregion Private Members

        #region Initialization
        public SqlClient(String connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }
        #endregion Initialization

        #region Connections
        public void Open()
        {
            _connection.Open();
        }

        public void Close()
        {
            _connection.Close();
        }
        #endregion Connections

        #region Methods
        public DataSet ExecuteStoredProcedure(String storedProcedureName, Dictionary<String, SqlParam> sqlParams)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand(storedProcedureName, _connection);
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (KeyValuePair<String, SqlParam> sqlParamKvp in sqlParams)
            {
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = sqlParamKvp.Value.Key,
                    Value = sqlParamKvp.Value.Value,
                    SqlDbType = sqlParamKvp.Value.Type,
                    Direction = sqlParamKvp.Value.Direction,
                    Size = sqlParamKvp.Value.Size
                });
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            foreach (SqlParameter sqlParam in cmd.Parameters)
            {
                if (sqlParam.Direction == ParameterDirection.Output)
                {
                    sqlParams[sqlParam.ParameterName].Value = sqlParam.Value;
                }
            }

            return ds;
        }

        public DataSet GetDataSet()
        {
            //SqlCommand cmd = new SqlCommand()
            throw new NotImplementedException();
        }
        #endregion Methods
    }

    #region SqlParam
    public class SqlParam
    {
        public String Key { get; set; }
        public Object Value { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public SqlDbType Type { get; set; }
        public Int32 Size { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ParameterDirection Direction { get; set; }
    }
    #endregion SqlParam

    public static class SqlHelper
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
                        colType = "INT";
                        break;
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                        String decimalMaxCompute = string.Format("MAX([{0}])", dc.ColumnName);
                        Decimal maxDecimal = 0.0m;
                        try
                        {
                            maxDecimal = Convert.ToDecimal(dt.Compute(decimalMaxCompute, string.Empty));
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("Object cannot be cast from DBNull to other types.") && dt.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (dr[dc.ColumnName] == DBNull.Value)
                                    {
                                        dr[dc.ColumnName] = 0.0m;
                                    }
                                }
                                maxDecimal = Convert.ToDecimal(dt.Compute(decimalMaxCompute, string.Empty));
                            }
                            else
                            {
                                maxDecimal = 100000000.000000001m;  //9 digits, 9 decimals => DECIMAL(18,9)
                            }
                        }
                        Int32 NumDigits = NumberHelper.GetNumDigits(maxDecimal);
                        Int32 MaxDecimalPlaces = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows.Cast<DataRow>().Max(row => NumberHelper.GetNumDecimalPlaces(Convert.ToDecimal(row[dc.ColumnName] == DBNull.Value ? 0.0m : row[dc.ColumnName])))) : 9;
                        colType = string.Format("DECIMAL({0},{1})", NumDigits + MaxDecimalPlaces + 1, MaxDecimalPlaces);
                        break;
                    case TypeCode.DateTime:
                        colType = "DATETIME";
                        break;
                    case TypeCode.String:
                    case TypeCode.Char:
                    default:
                        colType = "VARCHAR(MAX)";
                        break;
                }

                action(dc, colType);
            }
        }
        #endregion IterateColumns

        #region Generate SQL Command Strings
        public static String GenerateTable(DataTable dt, Boolean isTempTable = false)
        {
            if (dt.Columns.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                String tableName = isTempTable ?
                    (dt.TableName == "" ? "@Temp" : string.Format("@{0}", dt.TableName.Replace('.', '_').Replace(' ', '_'))) :
                    (dt.TableName == "" ? "Temp" : string.Format("{0}", dt.TableName));
                String columnsDeclare = "";

                IterateColumns(dt, new Action<DataColumn, string>((dc, colType) =>
                {
                    columnsDeclare += string.Format(",[{0}] {1}", dc.ColumnName, colType);
                }));

                columnsDeclare = columnsDeclare.TrimStart(','); //remove beginning comma
                String createFormat = isTempTable ? "DECLARE {0} TABLE ({1})" : "CREATE TABLE [{0}] ({1});";
                sb.AppendFormat(createFormat, tableName, columnsDeclare);

                return sb.ToString();
            }
            else
            {
                return "";
            }
        }

        public static String GenerateTableInsert(DataTable dt, Boolean isTempTable = false, Int32 RowsPerLine = Int32.MaxValue)
        {
            StringBuilder sb = new StringBuilder();
            String tableName = isTempTable ?
                    (dt.TableName == "" ? "@Temp" : string.Format("@{0}", dt.TableName.Replace('.', '_').Replace(' ', '_'))) :
                    (dt.TableName == "" ? "Temp" : string.Format("{0}", dt.TableName));
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
                        String insertFormat = isTempTable ? "\r\nINSERT INTO {0} ({1}) VALUES\r\n\t" : "\r\nINSERT INTO [{0}] ({1}) VALUES\r\n\t";
                        sb.AppendFormat(insertFormat, tableName, columnsInsert);
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
                                value = myDate.ToString("yyyy-MM-dd HH:mm:ss");
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

        public static String GenerateTableSelect(DataTable dt, Boolean isTempTable = false)
        {
            String tableName = isTempTable ?
                    (dt.TableName == "" ? "@Temp" : string.Format("@{0}", dt.TableName.Replace('.', '_').Replace(' ', '_'))) :
                    (dt.TableName == "" ? "Temp" : string.Format("{0}", dt.TableName));

            if (isTempTable)
            {
                return $"SELECT * FROM {tableName}";
            }
            else
            {
                return $"SELECT * FROM [{tableName}]";
            }
        }

        public static String GenerateTableTruncate(DataTable dt)
        {
            return $"DELETE FROM [{dt.TableName}];\n";
        }

        public static String GenerateTableDrop(DataTable dt)
        {
            return $"DROP TABLE IF EXISTS {dt.TableName};\n";
        }

        public static String GenerateSchema(String schema)
        {
            return $"CREATE SCHEMA {schema};\n";
        }

        #endregion Generate SQL Command Strings

    }
}
