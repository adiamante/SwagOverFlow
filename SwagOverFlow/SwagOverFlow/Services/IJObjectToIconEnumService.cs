using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverFlow.Services
{
    public interface IJObjectToIconEnumService
    {
        Enum ToEnum(JObject data, params String [] keys);
        JObject ToJObject(Enum iconEnum, params String[] keys);
    }
}
