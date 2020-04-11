using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SwagOverFlow.Clients
{
    public class SqliteClient
    {
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

    }
}
