#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System.Text;

namespace ConsoleFx.ConsoleExtensions
{
    /// <summary>
    ///     Represents a colored text block in a <see cref="ColorString" /> instance.
    /// </summary>
    public sealed class ColorStringBlock
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorStringBlock"/> class.
        /// </summary>
        /// <param name="text">
        ///     The text of the block that will be colored as per the specified colors.
        /// </param>
        /// <param name="foreColor">The foreground color of the text in this block.</param>
        /// <param name="backColor">The background color of the text in this block.</param>
        internal ColorStringBlock(string text, CColor? foreColor, CColor? backColor)
        {
            Text = text;
            ForeColor = foreColor;
            BackColor = backColor;
        }

        /// <summary>
        ///     Gets the text of this block.
        /// </summary>
        public string Text { get; }

        /// <summary>
        ///     Gets the foreground color of this block.
        /// </summary>
        public CColor? ForeColor { get; }

        /// <summary>
        ///     Gets the background color of this block.
        /// </summary>
        public CColor? BackColor { get; }

        /// <summary>
        ///     Returns a string representing this color block, which can be used in a
        ///     <see cref="ColorString"/>.
        /// </summary>
        /// <returns>A string representing this color block.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (ForeColor.HasValue || BackColor.HasValue)
            {
                sb.Append("[");
                if (ForeColor.HasValue)
                    sb.Append(ForeColor.Value);
                if (ForeColor.HasValue && BackColor.HasValue)
                    sb.Append(".");
                if (BackColor.HasValue)
                    sb.Append($"Bg{BackColor.Value}");
                sb.Append("]");
            }

            if (!string.IsNullOrEmpty(Text))
                sb.Append(Text);
            return sb.ToString();
        }
    }
}
