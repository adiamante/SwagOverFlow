using Dreamporter.Core;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;

namespace Dreamporter.JsonConverters
{
    public class InstructionListJsonConverter : AbstractJsonArrayConverter<List<Instruction>>
    {
        public static InstructionListJsonConverter Instance = new InstructionListJsonConverter();

        protected override List<Instruction> Create(Type objectType, JArray jArray)
        {
            List<Instruction> instructions = new List<Instruction>();
            return instructions;
        }
    }
}
