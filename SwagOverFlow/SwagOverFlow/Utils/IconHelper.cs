using Newtonsoft.Json.Linq;
using SwagOverFlow.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverFlow.Utils
{
    public static class IconHelper
    {
        static IJObjectToIconEnumService _jObjectToIconEnumService;

        public static void ResolveServices(IServiceProvider serviceProvider)
        {
            _jObjectToIconEnumService = (IJObjectToIconEnumService)serviceProvider.GetService(typeof(IJObjectToIconEnumService));
        }
        public static JObject ToObject(Enum iconEnum)
        {
            return _jObjectToIconEnumService.ToJObject(iconEnum);
        }

        public static Enum ToEnum(JObject data)
        {
            return _jObjectToIconEnumService.ToEnum(data);
        }
    }
}
