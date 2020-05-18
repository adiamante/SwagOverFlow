using System;
using System.Collections.Generic;
using System.IO;

namespace SwagOverFlow.Parsing
{
    //https://github.com/serilog/serilog/blob/dev/src/Serilog/Parsing/MessageTemplateToken.cs
    /// <summary>
    /// An element parsed from a message template string.
    /// </summary>
    public abstract class MessageTemplateToken
    {
        /// <summary>
        /// Construct a <see cref="MessageTemplateToken"/>.
        /// </summary>
        /// <param name="startIndex">The token's start index in the template.</param>
        protected MessageTemplateToken(int startIndex)
        {
            StartIndex = startIndex;
        }

        /// <summary>
        /// The token's start index in the template.
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int StartIndex { get; }

        /// <summary>
        /// The token's length.
        /// </summary>
        public abstract int Length { get; }

        /// <summary>
        /// Render the token to the output.
        /// </summary>
        /// <param name="properties">Properties that may be represented by the token.</param>
        /// <param name="output">Output for the rendered string.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract void Render(IReadOnlyDictionary<string, string> properties, TextWriter output, IFormatProvider formatProvider = null);
    }
}
