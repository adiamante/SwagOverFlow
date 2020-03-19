using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SwagOverflowWPF.Data
{
    public class FileToDataTableContext : ToDataTableContext
    {
        IFileToDataTable _strategy;

        public FileToDataTableContext()
        {

        }

        public FileToDataTableContext(IFileToDataTable strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IFileToDataTable strategy)
        {
            _strategy = strategy;
        }

        public DataTable FileToDataTable(String filePath)
        {
            return _strategy.FileToDataTable(filePath, this);
        }
    }
}
