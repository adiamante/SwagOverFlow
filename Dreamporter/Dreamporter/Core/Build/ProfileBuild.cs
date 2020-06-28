using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.Core
{
    public class ProfileBuild : BaseBuild
    {
        String instructionTemplateGuid;
        SwagOptionGroup _options = new SwagOptionGroup();

        #region InstructionTemplateGUID
        public String InstructionTemplateGUID
        {
            get { return instructionTemplateGuid; }
            set { SetValue(ref instructionTemplateGuid, value); }
        }
        #endregion InstructionTemplateGUID
        #region Options
        public SwagOptionGroup Options
        {
            get { return _options; }
            set { SetValue(ref _options, value); }
        }
        #endregion Options
        #region Type
        public override Type Type { get { return typeof(CoreBuild); } }
        #endregion Type
    }
}
