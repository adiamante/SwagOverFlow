using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Services;
using SwagOverFlow.Utils;
using System;

namespace SwagOverFlow.WPF.Services
{
    public class JObjectToIconEnumServiceWPF : IJObjectToIconEnumService
    {
        public Enum ToEnum(JObject data, params String [] keys)
        {
            Enum icon = null;
            String iconKey = keys.Length > 0 ? keys[0] : "Icon";

            if (data.ContainsKey(iconKey) && ((JObject)data[iconKey]).ContainsKey("WPF"))
            {
                String strIcon = data[iconKey]["WPF"]["String"].ToString();
                String strIconDataType = data[iconKey]["WPF"]["DataType"].ToString();
                if (icon == null && !String.IsNullOrEmpty(strIconDataType) && !String.IsNullOrEmpty(strIconDataType))
                {
                    Type iconType = JsonConvert.DeserializeObject<Type>(strIconDataType);
                    icon = (Enum)Enum.Parse(iconType, strIcon);
                }
            }

            return icon;
        }

        public JObject ToJObject(Enum iconEnum, params String [] keys)
        {
            String iconKey = keys.Length > 0 ? keys[0] : "Icon";
            JObject data = new JObject();
            JObject jIcon = new JObject();
            JObject wpfIcon = new JObject();
            data[iconKey] = jIcon;
            jIcon["WPF"] = wpfIcon;
            wpfIcon["String"] = iconEnum.ToString();
            wpfIcon["DataType"] = JsonHelper.ToJsonString(iconEnum.GetType());
            return data;
        }
    }
}
