using System;
using System.Data;
using System.IO;
using System.Text;

namespace SwagOverflowWPF.Data
{
    public class CsvStringToDataTable : IStringToDataTable
    {
        CsvStreamToDataTable _streamToDataTable = new CsvStreamToDataTable();
        public DataTable StringToDataTable(string str, ToDataTableContext context = null)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(str);
            MemoryStream stream = new MemoryStream(byteArray);
            return _streamToDataTable.StreamToDataTable(stream, context);
        }
    }
}
