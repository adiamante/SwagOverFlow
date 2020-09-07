using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SwagOverFlow.Utils
{
    public static class DataTableHelper
    {
        #region EscapeLikeValue
        //https://stackoverflow.com/questions/386122/correct-way-to-escape-characters-in-a-datatable-filter-expression
        public static string EscapeLikeValue(string value)
        {
            StringBuilder sb = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case ']':
                    case '[':
                    case '%':
                    case '*':
                        sb.Append("[").Append(c).Append("]");
                        break;
                    case '\'':
                        sb.Append("''");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
        #endregion EscapeLikeValue

        #region AutoConvertColumns
        public static void AutoConvertColumns(DataTable dt)
        {
            for (int col = 0; col < dt.Columns.Count; col++)
            {
                DataColumn dc = dt.Columns[col];
                List<Int32?> lstInts = new List<int?>();
                List<Decimal?> lstDecimals = new List<decimal?>();
                //List<TimeSpan?> lstTimeSpans = new List<TimeSpan?>();     //TimeSpan.TryParse detect 0 as a valid timespan so not working out
                List<DateTime?> lstDateTimes = new List<DateTime?>();
                Type colType = typeof(String);
                IEnumerable colVals = null;

                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    DataRow dr = dt.Rows[row];
                    Object val = dr[dc.ColumnName];

                    if (val == DBNull.Value || val == null)
                    {
                        lstInts.Add(null);
                        lstDecimals.Add(null);
                        //lstTimeSpans.Add(null);
                        lstDateTimes.Add(null);
                        continue;
                    }

                    String strVal = val.ToString();
                    if (lstInts.Count == row && Int32.TryParse(strVal, out Int32 intVal))
                    {
                        lstInts.Add(intVal);
                    }
                    if (lstDecimals.Count == row && Decimal.TryParse(strVal, out Decimal decVal))
                    {
                        lstDecimals.Add(decVal);
                    }
                    //if (lstTimeSpans.Count == row && TimeSpan.TryParse(strVal, out TimeSpan timeSpanVal))
                    //{
                    //    lstTimeSpans.Add(timeSpanVal);
                    //}
                    if (lstDateTimes.Count == row && !Int32.TryParse(strVal, out intVal) && DateTime.TryParse(strVal, out DateTime dateTimeVal))
                    {
                        lstDateTimes.Add(dateTimeVal);
                    }
                }

                if (lstDateTimes.Count == dt.Rows.Count)
                {
                    colVals = lstDateTimes;
                    colType = typeof(DateTime);
                }
                //else if (lstTimeSpans.Count == dt.Rows.Count)
                //{
                //    colVals = lstTimeSpans;
                //    colType = typeof(TimeSpan);
                //}
                else if (lstDecimals.Count == dt.Rows.Count)
                {
                    colVals = lstDecimals;
                    colType = typeof(Decimal);
                }
                else if (lstInts.Count == dt.Rows.Count)
                {
                    colVals = lstInts;
                    colType = typeof(Int32);
                }

                if (colVals != null)
                {
                    Int32 ordinal = dc.Ordinal;
                    dt.Columns.Remove(dc);
                    DataColumn newCol = new DataColumn(dc.ColumnName, colType);
                    dt.Columns.Add(newCol);
                    newCol.SetOrdinal(ordinal);

                    int row = 0;
                    foreach (var val in colVals)
                    {
                        DataRow dr = dt.Rows[row];
                        dr[newCol.ColumnName] = val == null ? DBNull.Value : val;
                        row++;
                    }
                }
            }
        }
        #endregion AutoConvertColumns
    }
}
