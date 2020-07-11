using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.Core
{
    public class InstructionBuild : Build
    {
        List<Schema> _requiredData = new List<Schema>();
        GroupInstruction _instructions = new GroupInstruction();

        #region RequiredData
        public List<Schema> RequiredData
        {
            get { return _requiredData; }
            set { SetValue(ref _requiredData, value); }
        }
        #endregion RequiredData
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
    }
}
