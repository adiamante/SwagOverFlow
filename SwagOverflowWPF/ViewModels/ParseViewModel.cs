using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverflowWPF.ViewModels
{
    #region ParseStrategy
    public enum ParseStrategy
    {
        Csv,
        Dbf,
        Xml,
        Json,
        Sqlite
    }
    #endregion ParseStrategy

    public class ParseViewModel : ViewModelBaseExtended
    {
        #region Private Members
        ParseStrategy _parseStategy = ParseStrategy.Csv;
        String _extension = "";
        Char _recordDelim = '\n', _fieldDelim = ',';
        Boolean _hasHeaders = true;
        #endregion Private Members

        #region Properties
        #region ParseStrategy
        public ParseStrategy ParseStrategy
        {
            get { return _parseStategy; }
            set { SetValue(ref _parseStategy, value); }
        }
        #endregion ParseStrategy
        #region RecordDelim
        public Char RecordDelim
        {
            get { return _recordDelim; }
            set { SetValue(ref _recordDelim, value); }
        }
        #endregion RecordDelim
        #region FieldDelim
        public Char FieldDelim
        {
            get { return _fieldDelim; }
            set { SetValue(ref _fieldDelim, value); }
        }
        #endregion FieldDelim
        #region HasHeaders
        public Boolean HasHeaders
        {
            get { return _hasHeaders; }
            set { SetValue(ref _hasHeaders, value); }
        }
        #endregion HasHeaders
        #endregion Properties

    }
}
