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
        Instruction _selectedInstruction;

        #region Instructions
        public GroupInstruction Instructions
        {
            get { return _instructions; }
            set { SetValue(ref _instructions, value); }
        }
        #endregion Instructions
        #region SelectedInstruction
        [JsonIgnore]
        [NotMapped]
        public Instruction SelectedInstruction
        {
            get { return _selectedInstruction; }
            set { SetValue(ref _selectedInstruction, value); }
        }
        #endregion SelectedInstruction
        #region Type
        public override Type Type { get { return typeof(InstructionBuild); } }
        #endregion Type
        #region Methods
        public override void RunHandler(RunContext context, RunParams rp)
        {
            TabIndex = 3;
            Instructions.Run(context, rp);
        }
        #endregion Methods
    }
}
