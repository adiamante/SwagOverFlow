using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace SwagOverflowWPF.Data
{
    public class StreamToDataTableContext : ToDataTableContext
    {
        IStreamToDataTable _strategy;

        public StreamToDataTableContext()
        {

        }

        public StreamToDataTableContext(IStreamToDataTable strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IStreamToDataTable strategy)
        {
            _strategy = strategy;
        }

        public DataTable StreamToDataTable(Stream stream)
        {
            return _strategy.StreamToDataTable(stream, this);
        }
    }
}
