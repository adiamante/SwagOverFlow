using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace SwagOverflowWPF.Data
{
    //This strategy uses RecordDelim, FieldDelim, or HasHeaders
    public class CsvStreamToDataTable : IStreamToDataTable
    {
        CsvConfiguration _conf = new CsvConfiguration(CultureInfo.InvariantCulture);
        public DataTable StreamToDataTable(Stream stream, ToDataTableContext context = null)
        {
            if (context == null)
            {
                context = new ToDataTableContext();
            }

            StreamReader sr = new StreamReader(stream);
            DataTable _dt = new DataTable();

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

            _conf.Delimiter = context.FieldDelim.ToString();
            _conf.HasHeaderRecord = context.HasHeaders;
            CsvReader csvReader = new CsvReader(sr, _conf);
            CsvDataReader dataReader = new CsvDataReader(csvReader);

            if (!context.HasHeaders)
            {
                #region If No Headers loop through all records and add columns as columns are found
                while (dataReader.Read())
                {
                    while (_dt.Columns.Count < dataReader.FieldCount)
                    {
                        _dt.Columns.Add($"Col{_dt.Columns.Count}");
                    }

                    DataRow row = _dt.NewRow();

                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        row[i] = dataReader.GetValue(i);
                    }

                    _dt.Rows.Add(row);
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
                    _dt.Load(dataReader, LoadOption.Upsert);
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

            return _dt;
        }
    }
}
