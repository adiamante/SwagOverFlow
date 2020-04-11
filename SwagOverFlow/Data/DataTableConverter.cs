using CsvHelper;
using CsvHelper.Configuration;
using SwagOverFlow.Clients;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SwagOverFlow.Data
{
    #region DataTableConvertContext
    public class DataTableConvertContext
    {
        public Char RecordDelim { get; set; } = '\n';
        public Char FieldDelim { get; set; } = ',';
        public Boolean HasHeaders { get; set; } = true;
    }
    #endregion DataTableConvertContext

    #region IDataTableConverter
    public interface IDataTableConverter
    {
        Object FromDataTableToObject(DataTableConvertContext context, DataTable dt);
        DataTable ToDataTable(DataTableConvertContext context, params object[] args);
    }
    #endregion IDataTableConverter

    #region DataTableConverter<T>
    public abstract class DataTableConverter<T> : IDataTableConverter
    {
        #region FromDataTable
        public virtual Object FromDataTableToObject(DataTableConvertContext context, DataTable dt)
        {
            return (Object)FromDataTable(context, dt);
        }

        abstract public T FromDataTable(DataTableConvertContext context, DataTable dt);
        #endregion FromDataTable

        #region ToDataTable
        public virtual DataTable ToDataTable(DataTableConvertContext context, params object[] args)
        {
            return ToDataTable(context, (T)args[0]);
        }

        abstract public DataTable ToDataTable(DataTableConvertContext context, T input);
        #endregion ToDataTable
    }
    #endregion DataTableConverter<T>

    #region DataTableCsvStreamConverter
    public class DataTableCsvStreamConverter : DataTableConverter<Stream>
    {
        #region Private Members
        CsvConfiguration _conf = new CsvConfiguration(CultureInfo.InvariantCulture);
        #endregion Private Members

        #region FromDataTable
        public override Stream FromDataTable(DataTableConvertContext context, DataTable dt)
        {
            Func<String, String> escapeCsvField = (strInput) =>
            {
                if (context.FieldDelim == ',' && (strInput.Contains(",") || strInput.Contains("\n") || strInput.Contains("\"")))
                {
                    return $"\"{strInput.Replace("\"", "\"\"")}\"";
                }
                return strInput;
            };

            MemoryStream ms = new MemoryStream();
            StreamWriter sr = new StreamWriter(ms);

            if (context.HasHeaders)
            {
                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                                    Select(column => column.ColumnName);
                sr.Write("{0}{1}", string.Join(context.FieldDelim.ToString(), columnNames), context.RecordDelim);
            }

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(
                    field => escapeCsvField(field.ToString()));
                sr.Write("{0}{1}", string.Join(context.FieldDelim.ToString(), fields), context.RecordDelim);
            }

            sr.Close();
            return ms;
        }
        #endregion FromDataTable

        #region ToDataTable
        public override DataTable ToDataTable(DataTableConvertContext context, Stream stream)
        {
            if (context == null)
            {
                context = new DataTableConvertContext();
            }

            StreamReader sr = new StreamReader(stream);
            DataTable dt = new DataTable();

            if (context.RecordDelim != '\n')
            {
                #region If Record Delimiter is overriden, itterate through all characters and replace them with new line
                //https://stackoverflow.com/questions/1232443/writing-to-then-reading-from-a-memorystream
                MemoryStream ms = new MemoryStream();

                StreamWriter sw = new StreamWriter(ms);
                while (sr.Peek() >= 0)
                {
                    Char c = (Char)sr.Read();
                    if (c == context.RecordDelim)
                    {
                        sw.Write('\n');
                    }
                    else
                    {
                        sw.Write(c);
                    }
                }

                sw.Flush();
                ms.Position = 0;

                sr = new StreamReader(ms, Encoding.UTF8);
                #endregion If Record Delimiter is overriden, itterate through all characters and replace them with new line
            }

            //_conf.BadDataFound = cxt =>
            //{
            //For debugging (put breakpoints here)
            //};

            _conf.Delimiter = context.FieldDelim.ToString();
            if (_conf.Delimiter != ",")
            {
                _conf.IgnoreQuotes = true;
            }
            _conf.HasHeaderRecord = context.HasHeaders;
            CsvReader csvReader = new CsvReader(sr, _conf);
            CsvDataReader dataReader = new CsvDataReader(csvReader);

            if (!context.HasHeaders)
            {
                #region If No Headers loop through all records and add columns as columns are found
                while (dataReader.Read())
                {
                    while (dt.Columns.Count < dataReader.FieldCount)
                    {
                        dt.Columns.Add($"Col{dt.Columns.Count}");
                    }

                    DataRow row = dt.NewRow();

                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        row[i] = dataReader.GetValue(i);
                    }

                    dt.Rows.Add(row);
                }

                dataReader.Close();
                sr.Close();
                #endregion If No Headers loop through all records and add columns as columns are found
            }
            else
            {
                #region If there are headers DataTable.Load will suffice
                try
                {
                    dt.Load(dataReader, LoadOption.Upsert);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    dataReader.Close();
                    sr.Close();
                }
                #endregion If there are headers DataTable.Load will suffice
            }

            //CsvHelper seems to make DataTable Columns readonly
            foreach (DataColumn dc in dt.Columns)
            {
                dc.ReadOnly = false;
            }

            return dt;
        }
        #endregion ToDataTable
    }
    #endregion DataTableCsvStreamConverter

    #region DataTableCsvStringConverter
    public class DataTableCsvStringConverter : DataTableConverter<String>
    {
        #region Private Members
        DataTableCsvStreamConverter _dataTableCsvStreamConverter = new DataTableCsvStreamConverter();
        #endregion Private Members

        #region FromDataTable
        public override string FromDataTable(DataTableConvertContext context, DataTable dt)
        {
            Func<String, String> escapeCsvField = (strInput) =>
            {
                if (context.FieldDelim == ',' && (strInput.Contains(",") || strInput.Contains("\n") || strInput.Contains("\"")))
                {
                    return $"\"{strInput.Replace("\"", "\"\"")}\"";
                }
                return strInput;
            };

            StringBuilder sb = new StringBuilder();

            if (context.HasHeaders)
            {
                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                                    Select(column => column.ColumnName);
                sb.AppendFormat("{0}{1}", string.Join(context.FieldDelim.ToString(), columnNames), context.RecordDelim);
            }

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(
                    field => escapeCsvField(field.ToString()));
                sb.AppendFormat("{0}{1}", string.Join(context.FieldDelim.ToString(), fields), context.RecordDelim);
            }

            return sb.ToString();
        }
        #endregion FromDataTable

        #region ToDataTable
        public override DataTable ToDataTable(DataTableConvertContext context, string input)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            return _dataTableCsvStreamConverter.ToDataTable(context, stream);
        }
        #endregion ToDataTable
    }
    #endregion DataTableCsvStringConverter

    #region DataTableCsvFileConverter
    public class DataTableCsvFileConverter : DataTableConverter<String>
    {
        #region Private Members
        DataTableCsvStreamConverter _dataTableCsvStreamConverter = new DataTableCsvStreamConverter();
        DataTableCsvStringConverter _dataTableCsvStringConverter = new DataTableCsvStringConverter();
        #endregion Private Members

        #region FromDataTable
        public override string FromDataTable(DataTableConvertContext context, DataTable dt)
        {
            return _dataTableCsvStringConverter.FromDataTable(context, dt);
        }
        #endregion FromDataTable

        #region ToDataTable
        public override DataTable ToDataTable(DataTableConvertContext context, string input)
        {
            StreamReader sr = File.OpenText(input);
            return _dataTableCsvStreamConverter.ToDataTable(context, sr.BaseStream);
        }
        #endregion ToDataTable
    }
    #endregion DataTableCsvFileConverter

    #region DataTableTSqlCommandConverter
    public class DataTableTSqlCommandConverter : DataTableConverter<String>
    {
        #region Private Members
        #endregion Private Members

        #region FromDataTable
        public override string FromDataTable(DataTableConvertContext context, DataTable dt)
        {
            return SqlHelper.GenerateTable(dt) + Environment.NewLine + SqlHelper.GenerateTableInsert(dt);
        }
        #endregion FromDataTable

        #region ToDataTable
        public override DataTable ToDataTable(DataTableConvertContext context, string input)
        {
            throw new NotImplementedException();
        }
        #endregion ToDataTable
    }
    #endregion DataTableTSqlCommandConverter

    #region DataTableSqliteCommandConverter
    public class DataTableSqliteCommandConverter : DataTableConverter<String>
    {
        #region Private Members
        #endregion Private Members

        #region FromDataTable
        public override string FromDataTable(DataTableConvertContext context, DataTable dt)
        {
            return SqliteHelper.GenerateTable(dt) + Environment.NewLine + SqliteHelper.GenerateTableInsert(dt);
        }
        #endregion FromDataTable

        #region ToDataTable
        public override DataTable ToDataTable(DataTableConvertContext context, string input)
        {
            throw new NotImplementedException();
        }
        #endregion ToDataTable
    }
    #endregion DataTableSqliteCommandConverter
}
