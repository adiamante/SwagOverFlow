using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace SwagOverflowWPF.Data
{
    public interface IStreamToDataTable
    {
        DataTable StreamToDataTable(Stream stream, ToDataTableContext context = null);
    }
}
