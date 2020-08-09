using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Services;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverFlow.WPF.Services
{
    public class JObjectToIconEnumServiceWPF : IJObjectToIconEnumService
    {
        public Enum ToEnum(JObject data)
        {
            Enum icon = null;

            if (data.ContainsKey("Icon") && ((JObject)data["Icon"]).ContainsKey("WPF"))
            {
                String strIcon = data["Icon"]["WPF"]["String"].ToString();
                String strIconDataType = data["Icon"]["WPF"]["DataType"].ToString();
                if (icon == null && !String.IsNullOrEmpty(strIconDataType) && !String.IsNullOrEmpty(strIconDataType))
                {
                    Type iconType = JsonConvert.DeserializeObject<Type>(strIconDataType);
                    icon = (Enum)Enum.Parse(iconType, strIcon);
                }
            }

            return icon;
        }

        public JObject ToJObject(Enum iconEnum)
        {
            JObject data = new JObject();
            JObject jIcon = new JObject();
            JObject wpfIcon = new JObject();
            data["Icon"] = jIcon;
            jIcon["WPF"] = wpfIcon;
            wpfIcon["String"] = iconEnum.ToString();
            wpfIcon["DataType"] = JsonHelper.ToJsonString(iconEnum.GetType());
            return data;
        }
    }
}
