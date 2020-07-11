using Newtonsoft.Json;
using SwagOverFlow.JsonConverters;
using SwagOverFlow.Services;
using SwagOverFlow.WPF.ViewModels;
using System.Collections.Generic;

namespace SwagOverFlow.WPF.Services
{
    public class JsonConverterProviderServiceWPF : IJsonConverterProviderService
    {
        public IEnumerable<JsonConverter> GetConverters()
        {
            return new List<JsonConverter>() { 
                BooleanExpressionJsonConverter.Instance, 
                SwagOptionJsonConverter.Instance, 
                SwagOptionListJsonConverter.Instance 
            };
        }
    }
}
