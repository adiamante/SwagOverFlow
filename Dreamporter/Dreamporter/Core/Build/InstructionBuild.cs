using Newtonsoft.Json;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Dreamporter.Core
{
    public class InstructionBuild : Build
    {
        GroupInstruction _instructions = new GroupInstruction();
        Int32 _tabIndex = 0;            //For the UI

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
        #region TabIndex
        [JsonIgnore]
        [NotMapped]
        public Int32 TabIndex
        {
            get { return _tabIndex; }
            set { SetValue(ref _tabIndex, value); }
        }
        #endregion TabIndex
        #region Methods
        public override void RunHandler(RunContext context, RunParams rp)
        {
            TabIndex = 3;
            Instructions.Run(context, rp);
        }
        #endregion Methods
    }
}
