using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.Core
{
    public class InstructionBuild : Build
    {
        GroupInstruction _instructions = new GroupInstruction();

        #region Instructions
        public GroupInstruction Instructions
        {
            get { return _instructions; }
            set { SetValue(ref _instructions, value); }
        }
        #endregion Instructions
        #region Type
        public override Type Type { get { return typeof(InstructionBuild); } }
        #endregion Type

        #region Methods
        public override void RunHandler(RunContext context, RunParams rp)
        {
            Instructions.Run(context, rp);
        }
        #endregion Methods
    }
}
