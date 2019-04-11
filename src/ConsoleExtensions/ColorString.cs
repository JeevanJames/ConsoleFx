#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2018 Jeevan James

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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleFx.ConsoleExtensions
{
    /// <summary>
    /// Represents a string that includes color information.
    /// </summary>
    /// <remarks>
    /// Internally, this class maintains a collection of text blocks, each with color information.
    /// </remarks>
    public sealed class ColorString : IReadOnlyList<ColorStringBlock>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<ColorStringBlock> _blocks = new List<ColorStringBlock>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CColor? _currentForeColor;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CColor? _currentBackColor;

        public ColorString()
        {
        }

        public ColorString(string initialText)
        {
            Text(initialText, null, null);
        }

        /// <summary>
        /// <para>Adds the text to the color string with the specified foreground and background colors.</para>
        /// <para>The colors are optional and if not specified, they retain whatever values they were before.</para>
        /// <para>Unless you want to dynamically specify the colors, prefer using the color-specified methods like <see cref="Red"/>.</para>
        /// </summary>
        /// <param name="text">The text to add to the color string.</param>
        /// <param name="foreColor">Optional foreground color for the text.</param>
        /// <param name="backColor">Optional background color for the text.</param>
        /// <returns>The current instance of <see cref="ColorString"/>.</returns>
        public ColorString Text(string text, CColor? foreColor = null, CColor? backColor = null)
        {
            if (foreColor.HasValue)
                _currentForeColor = foreColor.Value;
            if (backColor.HasValue)
                _currentBackColor = backColor.Value;
            if (!string.IsNullOrEmpty(text))
                _blocks.Add(new ColorStringBlock(text, _currentForeColor, _currentBackColor));
            return this;
        }

        /// <summary>
        /// Resets the foreground color.
        /// </summary>
        /// <param name="text">Optional text to append to the color string after resetting the foreground color.</param>
        /// <returns>The current instance of <see cref="ColorString"/>.</returns>
        public ColorString Reset(string text = null)
        {
            _currentForeColor = null;
            return Text(text, null, null);
        }

        /// <summary>
        /// Resets the background color.
        /// </summary>
        /// <param name="text">Optional text to append to the color string after resetting the background color.</param>
        /// <returns>The current instance of <see cref="ColorString"/>.</returns>
        public ColorString BgReset(string text = null)
        {
            _currentBackColor = null;
            return Text(text, null, null);
        }

        public ColorString Black(string text = null) => Text(text, CColor.Black);

        public ColorString Blue(string text = null) => Text(text, CColor.Blue);

        public ColorString Cyan(string text = null) => Text(text, CColor.Cyan);

        public ColorString DkBlue(string text = null) => Text(text, CColor.DkBlue);

        public ColorString DkCyan(string text = null) => Text(text, CColor.DkCyan);

        public ColorString DkGray(string text = null) => Text(text, CColor.DkGray);

        public ColorString DkGreen(string text = null) => Text(text, CColor.DkGreen);

        public ColorString DkMagenta(string text = null) => Text(text, CColor.DkMagenta);

        public ColorString DkRed(string text = null) => Text(text, CColor.DkRed);

        public ColorString DkYellow(string text = null) => Text(text, CColor.DkYellow);

        public ColorString Gray(string text = null) => Text(text, CColor.Gray);

        public ColorString Green(string text = null) => Text(text, CColor.Green);

        public ColorString Magenta(string text = null) => Text(text, CColor.Magenta);

        public ColorString Red(string text = null) => Text(text, CColor.Red);

        public ColorString White(string text = null) => Text(text, CColor.White);

        public ColorString Yellow(string text = null) => Text(text, CColor.Yellow);

        public ColorString BgBlack(string text = null) => Text(text, null, CColor.Black);

        public ColorString BgBlue(string text = null) => Text(text, null, CColor.Blue);

        public ColorString BgCyan(string text = null) => Text(text, null, CColor.Cyan);

        public ColorString BgDkBlue(string text = null) => Text(text, null, CColor.BgDkBlue);

        public ColorString BgDkCyan(string text = null) => Text(text, null, CColor.BgDkCyan);

        public ColorString BgDkGray(string text = null) => Text(text, null, CColor.BgDkGray);

        public ColorString BgDkGreen(string text = null) => Text(text, null, CColor.BgDkGreen);

        public ColorString BgDkMagenta(string text = null) => Text(text, null, CColor.BgDkMagenta);

        public ColorString BgDkRed(string text = null) => Text(text, null, CColor.BgDkRed);

        public ColorString BgDkYellow(string text = null) => Text(text, null, CColor.BgDkYellow);

        public ColorString BgGray(string text = null) => Text(text, null, CColor.Gray);

        public ColorString BgGreen(string text = null) => Text(text, null, CColor.Green);

        public ColorString BgMagenta(string text = null) => Text(text, null, CColor.Magenta);

        public ColorString BgRed(string text = null) => Text(text, null, CColor.Red);

        public ColorString BgWhite(string text = null) => Text(text, null, CColor.White);

        public ColorString BgYellow(string text = null) => Text(text, null, CColor.Yellow);

        /// <summary>
        /// Builds a string representing the text blocks defined in the object.
        /// </summary>
        /// <returns>A color string.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (ColorStringBlock block in _blocks)
                sb.Append(block.ToString());
            return sb.ToString();
        }

        #region Interface implementations

        IEnumerator<ColorStringBlock> IEnumerable<ColorStringBlock>.GetEnumerator() =>
            _blocks.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            _blocks.GetEnumerator();

        int IReadOnlyCollection<ColorStringBlock>.Count => _blocks.Count;

        ColorStringBlock IReadOnlyList<ColorStringBlock>.this[int index] => _blocks[index];

        #endregion

        public static implicit operator string(ColorString cstr) => cstr.ToString();

        public static implicit operator ColorString(string cstr) =>
            TryParse(cstr, out ColorString colorStr) ? colorStr : null;

        /// <summary>
        /// Creates an instance of a <see cref="ColorString"/> from the specified color string.
        /// </summary>
        /// <param name="cstr">The color string to parse</param>
        /// <param name="colorStr">The new <see cref="ColorString"/> instance created from the color string.</param>
        /// <returns>True, if the color string could be parsed and a <see cref="ColorString"/> instance created; otherwise false.</returns>
        public static bool TryParse(string cstr, out ColorString colorStr)
        {
            colorStr = new ColorString();

            MatchCollection matches = ColorStringPattern.Matches(cstr);
            if (matches.Count == 0)
            {
                colorStr.Text(cstr);
                return true;
            }

            for (int i = 0; i < matches.Count; i++)
            {
                int startIndex = i == 0 ? 0 : matches[i - 1].Index + matches[i - 1].Length;
                int endIndex = matches[i].Index;
                colorStr.Text(cstr.Substring(startIndex, endIndex - startIndex));

                // First color is always specified. Figure it out.
                string color1Str = matches[i].Groups[1].Value;
                CColor color1 = (CColor)Enum.Parse(typeof(CColor), color1Str, true);
                bool color1IsBackground = color1Str.StartsWith("Bg", StringComparison.OrdinalIgnoreCase);

                // Second color is optional. Check if it is specified.
                string color2Str = matches[i].Groups[2].Value;
                CColor? color2 = null;
                if (!string.IsNullOrEmpty(color2Str))
                {
                    color2 = (CColor)Enum.Parse(typeof(CColor), color2Str, true);
                    bool color2IsBackground = color2Str.StartsWith("Bg", StringComparison.OrdinalIgnoreCase);

                    // Both colors cannot be background or foreground colors
                    if (color1IsBackground == color2IsBackground)
                    {
                        if (color1IsBackground)
                            throw new FormatException($"Specified colors {color1Str} and {color2Str} cannot both be background colors.");
                        throw new FormatException($"Specified colors {color1Str} and {color2Str} cannot both be foreground colors.");
                    }
                }

                // Figure out which specified color is background and which is foreground.
                CColor? foreColor = null, backColor = null;
                if (color1IsBackground)
                    backColor = color1;
                else
                    foreColor = color1;
                if (color2.HasValue)
                {
                    if (color1IsBackground)
                        foreColor = color2.Value;
                    else
                        backColor = color2.Value;
                }

                colorStr.Text(null, foreColor, backColor);
            }

            Match lastMatch = matches[matches.Count - 1];
            colorStr.Text(cstr.Substring(lastMatch.Index + lastMatch.Length));

            return true;
        }

        private static readonly Regex ColorStringPattern = new Regex(@"\[(\w+)(?:\.(\w+))?\]");
    }
}
