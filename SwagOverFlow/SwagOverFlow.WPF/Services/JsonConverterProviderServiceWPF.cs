using Newtonsoft.Json;
using SwagOverFlow.Services;
using SwagOverFlow.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverFlow.WPF.Services
{
    public class JsonConverterProviderServiceWPF : IJsonConverterProviderService
    {
        public IEnumerable<JsonConverter> GetConverters()
        {
            return new List<JsonConverter>() { BooleanExpressionWPFJsonConverter.Instance, SwagOptionWPFJsonConverter.Instance };
        }
    }
}
