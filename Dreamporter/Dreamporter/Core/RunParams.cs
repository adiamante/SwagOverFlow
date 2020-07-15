using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.Core
{
    public class RunParams
    {
        public string Build { get; set; }
        public string Instruction { get; set; }
        public Dictionary<String, String> Params { get; set; } = new Dictionary<string, string>();
        public string FullPath
        {
            get
            {
                return $"[{Build}] {Instruction}";
            }
        }

        public string ApplyParams(string strInput)
        {
            String strOutput = strInput;
            foreach (KeyValuePair<String, String> kvp in Params)
            {
                strOutput = strOutput.Replace($"{{{kvp.Key}}}", kvp.Value);
            }
            return strOutput;
        }
    }
}
