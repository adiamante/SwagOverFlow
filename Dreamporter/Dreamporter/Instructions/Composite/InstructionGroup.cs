using Dreamporter.Builds;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamporter.Instructions
{
    public class InstructionGroup : Instruction
    {
        #region Properties
        #region Children
        public virtual ICollection<Instruction> Children { get; set; }
        #endregion Children
        #region IsConcurrent
        public Boolean IsConcurrent { get; set; }
        #endregion IsConcurrent
        #endregion Properties

        #region Initialization
        public InstructionGroup()
        {
        }
        #endregion Initialization

        #region Events
        #endregion Events

        #region Methods
        public override void Execute(RuntimeContext context, Dictionary<String, String> parameters)
        {
            if (IsConcurrent)
            {
                ConcurrentOrderedTasks tasks = new ConcurrentOrderedTasks();
                foreach (Instruction child in Children.OrderBy(c => c.Sequence))
                {
                    Task task = new Task(() => child.Run(context, parameters));
                    tasks.Append(task);
                }
                tasks.Execute();
            }
            else
            {
                foreach (Instruction child in Children.OrderBy(c => c.Sequence))
                {
                    child.Run(context, parameters);
                }
            }
        }
        #endregion Methods
    }
}
