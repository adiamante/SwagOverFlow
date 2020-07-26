using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.Core
{
    //When depending on this instruction, sequence will definitely matter
    public class SetRunParamsInstruction : Instruction
    {
        #region Type
        public override Type Type { get { return typeof(SetRunParamsInstruction); } }
        #endregion Type
        public override void RunHandler(RunContext context, RunParams rp)
        {
            String strOptions = JsonHelper.ToJsonString(Options);
            strOptions = rp.ApplyParams(strOptions);        //Apply current RunParams
            SwagOptionGroup options = (SwagOptionGroup)JsonHelper.ToObject<SwagOptionGroup>(strOptions);
            
            foreach (KeyValuePair<String, String> kvpOpt in options.Dict)
            {
                rp.Params.SafeSet(kvpOpt.Key, kvpOpt.Value);
            }
        }
    }
}
