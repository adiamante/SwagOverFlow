using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverflowWPF.Data
{
    public class ToDataTableContext
    {
        public Char RecordDelim { get; set; } = '\n';
        public Char FieldDelim { get; set; } = ',';
        public Boolean HasHeaders { get; set; } = true;
    }
}
