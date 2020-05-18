using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverFlow.Parsing
{
    //https://github.com/serilog/serilog/blob/dev/src/Serilog/Parsing/Destructuring.cs
    /// <summary>
    /// Instructs the logger on how to store information about provided
    /// parameters.
    /// </summary>
    public enum Destructuring
    {
        /// <summary>
        /// Convert known types and objects to scalars, arrays to sequences.
        /// </summary>
        Default,

        /// <summary>
        /// Convert all types to scalar strings. Prefix name with '$'.
        /// </summary>
        Stringify,

        /// <summary>
        /// Convert known types to scalars, destructure objects and collections
        /// into sequences and structures. Prefix name with '@'.
        /// </summary>
        Destructure
    }
}
