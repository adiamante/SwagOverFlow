using System;

namespace Dreamporter.Instructions
{
    public class InstructionException : Exception
    {
        public Boolean Abort { get; set; } = false;
        public string Info { get; set; } = "";
        public InstructionException()
        {

        }

        public InstructionException(string message) : base(message)
        {

        }
    }
}