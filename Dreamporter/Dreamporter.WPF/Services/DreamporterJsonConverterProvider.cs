using Dreamporter.Core;
using Dreamporter.WPF.ViewModels;
using Newtonsoft.Json;
using SwagOverFlow.Services;
using SwagOverFlow.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.WPF.Services
{
    public class DreamporterJsonConverterProvider : IJsonConverterProviderService
    {
        public IEnumerable<JsonConverter> GetConverters()
        {
            return new List<JsonConverter>() { BooleanExpressionWPFJsonConverter.Instance, SwagOptionWPFJsonConverter.Instance, InstructionWPFJsonConverter.Instance };
        }
    }
}
