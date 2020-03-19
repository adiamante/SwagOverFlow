using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SwagOverflowWPF.Data
{
    public interface IFileToDataTable
    {
        DataTable FileToDataTable(String filePath, ToDataTableContext context = null);
    }
}
