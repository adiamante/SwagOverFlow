using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

namespace SwagOverFlow.Data
{
    #region IDataSetConverter
    public interface IDataSetConverter
    {
        Object FromDataSetToObject(DataSetConvertParams cnvParams, DataSet ds);
        DataSet ToDataSet(DataSetConvertParams cnvParams, params object[] args);
    }
    #endregion IDataSetConverter

    #region DataSetConvertParams
    public class DataSetConvertParams
    {
        public Char RecordDelim { get; set; } = '\n';
        public Char FieldDelim { get; set; } = ',';
        public Boolean HasHeaders { get; set; } = true;
    }
    #endregion DataSetConvertParams

    #region DataSetConvertContext
    public class DataSetConvertContext
    {
        public IDataSetConverter Converter { get; set; }
        public DataSetConvertParams Params { get; set; }

        public DataSetConvertContext(IDataSetConverter converter, DataSetConvertParams parameters)
        {
            Converter = converter;
            Params = parameters;
        }

        #region FromDataSetToObject
        public object FromDataSet(DataSet dt)
        {
            return Converter.FromDataSetToObject(Params, dt);
        }
        #endregion FromDataSetToObject

        #region ToDataSet
        public DataSet ToDataSet(params object[] args)
        {
            return Converter.ToDataSet(Params, args);
        }
        #endregion ToDataSet
    }
    #endregion DataSetConvertContext

    #region DataSetConverter<T>
    public abstract class DataSetConverter<T> : IDataSetConverter
    {
        #region FromDataSet
        public virtual Object FromDataSetToObject(DataSetConvertParams context, DataSet dt)
        {
            return (Object)FromDataSet(context, dt);
        }

        abstract public T FromDataSet(DataSetConvertParams context, DataSet dt);
        #endregion FromDataSet

        #region ToDataSet
        public virtual DataSet ToDataSet(DataSetConvertParams context, params object[] args)
        {
            return ToDataSet(context, (T)args[0]);
        }

        abstract public DataSet ToDataSet(DataSetConvertParams context, T input);
        #endregion ToDataSet
    }
    #endregion DataSetConverter<T>

    #region DataSetXmlStreamConverter
    public class DataSetXmlStreamConverter : DataSetConverter<Stream>
    {
        #region Private Members
        #endregion Private Members

        #region FromDataSet
        public override Stream FromDataSet(DataSetConvertParams context, DataSet ds)
        {
            MemoryStream ms = new MemoryStream();
            ds.WriteXml(ms);
            return ms;
        }
        #endregion FromDataSet

        #region ToDataSet
        public override DataSet ToDataSet(DataSetConvertParams context, Stream stream)
        {
            DataSet ds = new DataSet();
            //Will add in try catch with handlings later
            ds.ReadXml(stream);
            return ds;
        }
        #endregion ToDataSet
    }
    #endregion DataSetXmlStreamConverter

    #region DataSetXmlStringConverter
    public class DataSetXmlStringConverter : DataSetConverter<String>
    {
        #region Private Members
        DataSetXmlStreamConverter _dataSetXmlStreamConverter = new DataSetXmlStreamConverter();
        #endregion Private Members

        #region FromDataSet
        public override string FromDataSet(DataSetConvertParams context, DataSet ds)
        {
            StreamReader sr = new StreamReader(_dataSetXmlStreamConverter.FromDataSet(context, ds));
            return sr.ReadToEnd();
        }
        #endregion FromDataSet

        #region ToDataSet
        public override DataSet ToDataSet(DataSetConvertParams context, string input)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            return _dataSetXmlStreamConverter.ToDataSet(context, stream);
        }
        #endregion ToDataSet
    }
    #endregion DataSetXmlStringConverter

    #region DataSetXmlFileConverter
    public class DataSetXmlFileConverter : DataSetConverter<String>
    {
        #region Private Members
        DataSetXmlStreamConverter _dataSetXmlStreamConverter = new DataSetXmlStreamConverter();
        DataSetXmlStringConverter _dataSetXmlStringConverter = new DataSetXmlStringConverter();
        #endregion Private Members

        #region FromDataSet
        public override string FromDataSet(DataSetConvertParams context, DataSet dt)
        {
            return _dataSetXmlStringConverter.FromDataSet(context, dt);
        }
        #endregion FromDataSet

        #region ToDataSet
        public override DataSet ToDataSet(DataSetConvertParams context, string input)
        {
            StreamReader sr = File.OpenText(input);
            return _dataSetXmlStreamConverter.ToDataSet(context, sr.BaseStream);
        }
        #endregion ToDataSet
    }
    #endregion DataSetXmlFileConverter

    #region DataSetConverterContextCache
    public class DataSetConverterContextCache
    {
        private ConcurrentDictionary<String, DataSetConvertContext> _converters = new ConcurrentDictionary<String, DataSetConvertContext>();

        public DataSetConvertContext this[String extension]
        {
            get
            {
                switch (extension.ToLower())
                {
                    
                }

                if (!_converters.ContainsKey(extension.ToLower()))
                {
                    return null;
                }

                return _converters[extension];
            }
        }
    }
    #endregion DataSetConverterContextCache

    #region DataSetConverterFileContextCache
    public class DataSetConverterFileContextCache
    {
        private ConcurrentDictionary<String, DataSetConvertContext> _converters = new ConcurrentDictionary<String, DataSetConvertContext>();

        public DataSetConvertContext this[String extension]
        {
            get
            {
                switch (extension.ToLower())
                {
                    case "xml":
                    case ".xml":
                        IDataSetConverter XmlConverter = new DataSetXmlFileConverter();
                        DataSetConvertParams XmlContext = new DataSetConvertParams();
                        _converters.TryAdd(extension.ToLower(), new DataSetConvertContext(XmlConverter, XmlContext));
                        break;
                }

                if (!_converters.ContainsKey(extension.ToLower()))
                {
                    return null;
                }

                return _converters[extension];
            }
        }
    }
    #endregion DataSetConverterFileContextCache

    #region DataSetConverterHelper
    public static class DataSetConverterHelper
    {
        private static DataSetConverterFileContextCache _converterFileContextCache = new DataSetConverterFileContextCache();
        public static DataSetConverterFileContextCache ConverterFileContexts => _converterFileContextCache;
    }
    #endregion DataSetConverterHelper
}
