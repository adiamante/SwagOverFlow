using Dreamporter.JsonConverters;
using Newtonsoft.Json;
using SwagOverFlow.JsonConverters;
using SwagOverFlow.Services;
using SwagOverFlow.WPF.ViewModels;
using System.Collections.Generic;

namespace Dreamporter.WPF.Services
{
    public class DreamporterJsonConverterProvider : IJsonConverterProviderService
    {
        public IEnumerable<JsonConverter> GetConverters()
        {
            return new List<JsonConverter>() { 
                BooleanExpressionJsonConverter.Instance, 
                SwagOptionJsonConverter.Instance,
                BuildJsonConverter.Instance,
                InstructionJsonConverter.Instance,
                InstructionListJsonConverter.Instance,
                DataContextJsonConverter.Instance,
                DataContextListJsonConverter.Instance
            };
        }
    }
}
