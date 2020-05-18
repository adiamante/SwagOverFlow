using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverFlow.Parsing
{
    //https://github.com/serilog/serilog/blob/dev/src/Serilog/Parsing/AlignmentDirection.cs
    /// <summary>
    /// Defines the direction of the alignment.
    /// </summary>
    public enum AlignmentDirection
    {
        /// <summary>
        /// Text will be left-aligned.
        /// </summary>
        Left,

        /// <summary>
        /// Text will be right-aligned.
        /// </summary>
        Right
    }

    //https://github.com/serilog/serilog/blob/dev/src/Serilog/Parsing/Alignment.cs
    /// <summary>
    /// A structure representing the alignment settings to apply when rendering a property.
    /// </summary>
    public struct Alignment
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Alignment"/>.
        /// </summary>
        /// <param name="direction">The text alignment direction.</param>
        /// <param name="width">The width of the text, in characters.</param>
        public Alignment(AlignmentDirection direction, int width)
        {
            Direction = direction;
            Width = width;
        }

        /// <summary>
        /// The text alignment direction.
        /// </summary>
        public AlignmentDirection Direction { get; }

        /// <summary>
        /// The width of the text.
        /// </summary>
        public int Width { get; }
    }
}
