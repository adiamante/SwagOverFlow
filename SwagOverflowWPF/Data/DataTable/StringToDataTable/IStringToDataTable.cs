using System;
using System.Data;

namespace SwagOverflowWPF.Data
{
    public interface IStringToDataTable
    {
        DataTable StringToDataTable(String str, ToDataTableContext context = null);
    }
}
