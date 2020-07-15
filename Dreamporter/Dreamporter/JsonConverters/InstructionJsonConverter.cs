﻿using Dreamporter.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.JsonConverters;
using SwagOverFlow.Utils;
using System;

namespace Dreamporter.JsonConverters
{
    public class InstructionJsonConverter : AbstractJsonConverter<Instruction>
    {
        public static InstructionJsonConverter Instance = new InstructionJsonConverter();

        protected override Instruction Create(Type objectType, JObject jObject)
        {
            String strType = jObject["Type"].ToString();
            Type type = Type.GetType(strType);
            JArray jaChildren = null;
            JObject jTemplate = null;

            if (jObject.ContainsKey("Parent"))  //Prevent ReadJson Null error
            {
                jObject.Remove("Parent");
            }

            if (jObject.ContainsKey("Children")) //Children are handled below
            {
                jaChildren = (JArray)jObject["Children"];
                jObject.Remove("Children");
            }

            if (jObject.ContainsKey("Template")) //Template are handled below
            {
                jTemplate = (JObject)jObject["Template"];
                jObject.Remove("Template");
            }

            //https://stackoverflow.com/questions/13394401/json-net-deserializing-list-gives-duplicate-items
            //ObjectCreationHandling.Replace is for list properties
            Instruction instruction = (Instruction)JsonConvert.DeserializeObject(jObject.ToString(), type, new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace, Converters = new JsonConverter[] { SwagOptionJsonConverter.Instance, BooleanExpressionJsonConverter.Instance } });

            switch (instruction)
            {
                case GroupInstruction grp:
                    foreach (JToken token in jaChildren)
                    {
                        JObject jChild = (JObject)token;
                        Instruction child = (Instruction)JsonConvert.DeserializeObject(jChild.ToString(), typeof(Instruction), new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace, Converters = new JsonConverter[] { InstructionJsonConverter.Instance, SwagOptionJsonConverter.Instance, BooleanExpressionJsonConverter.Instance } });
                        grp.Children.Add(child);
                    }
                    break;
                case TemplateInstruction tmplt:
                    GroupInstruction template = (GroupInstruction)JsonConvert.DeserializeObject(jTemplate.ToString(), typeof(GroupInstruction), new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace, Converters = new JsonConverter[] { InstructionJsonConverter.Instance, SwagOptionJsonConverter.Instance, BooleanExpressionJsonConverter.Instance } });
                    tmplt.Template = template;
                    break;
            }

            return instruction;
        }
    }
}
