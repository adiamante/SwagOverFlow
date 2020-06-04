using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverFlow.Services
{
    public interface IJsonConverterProviderService
    {
        IEnumerable<JsonConverter> GetConverters();
    }
}
