using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace SwagOverflowWPF.Data
{
    public class CsvFileToDataTable : IFileToDataTable
    {
        CsvStreamToDataTable _streamToDataTable = new CsvStreamToDataTable();
        public DataTable FileToDataTable(string filePath, ToDataTableContext context = null)
        {
            StreamReader sr = File.OpenText(filePath);
            return _streamToDataTable.StreamToDataTable(sr.BaseStream, context);
        }
    }
}
