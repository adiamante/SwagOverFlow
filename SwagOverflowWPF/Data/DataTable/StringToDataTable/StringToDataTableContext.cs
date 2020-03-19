using System;
using System.Data;

namespace SwagOverflowWPF.Data
{
    public class StringToDataTableContext : ToDataTableContext
    {
        IStringToDataTable _strategy;

        public StringToDataTableContext()
        {

        }

        public StringToDataTableContext(IStringToDataTable strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IStringToDataTable strategy)
        {
            _strategy = strategy;
        }

        public DataTable StringToDataTable(String StringPath)
        {
            return _strategy.StringToDataTable(StringPath, this);
        }
    }
}
