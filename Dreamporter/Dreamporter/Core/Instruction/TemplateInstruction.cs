using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.Core
{
    public class TemplateInstruction : Instruction
    {
        String _templateKey;
        SwagOptionGroup _options = new SwagOptionGroup();
        GroupInstruction _template;

        #region Type
        public override Type Type { get { return typeof(TemplateInstruction); } }
        #endregion Type
        #region TemplateKey
        public String TemplateKey
        {
            get { return _templateKey; }
            set { SetValue(ref _templateKey, value); }
        }
        #endregion TemplateKey
        #region Options
        public SwagOptionGroup Options
        {
            get { return _options; }
            set { SetValue(ref _options, value); }
        }
        #endregion Options
        #region Template
        public GroupInstruction Template
        {
            get { return _template; }
            set { SetValue(ref _template, value); }
        }
        #endregion Template

        public override void RunHandler(RunContext context, RunParams rp)
        {
            String strTemplate = JsonHelper.ToJsonString(Template);
            String resolvedInstructions = Instruction.ResolveParameters(strTemplate, Options.Dict);
            GroupInstruction groupInstruction = JsonHelper.ToObject<GroupInstruction>(resolvedInstructions);
            groupInstruction.Run(context, rp);
        }
    }
}
