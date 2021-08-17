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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleFx.ConsoleExtensions
{
    /// <summary>
    ///     Represents a string that includes color information.
    /// </summary>
    /// <remarks>
    ///     Internally, this class maintains a collection of text blocks, each with color information.
    /// </remarks>
    public sealed partial class ColorString
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<ColorStringBlock> _blocks = new List<ColorStringBlock>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CColor? _currentForeColor;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CColor? _currentBackColor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorString"/> class.
        /// </summary>
        public ColorString()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorString"/> class with the specified
        ///     <paramref name="initialText"/>.
        /// </summary>
        /// <param name="initialText">The initial text to add to the <see cref="ColorString"/>.</param>
        /// <param name="foreColor">Optional foreground color for the text.</param>
        /// <param name="backColor">Optional background color for the text.</param>
        public ColorString(string initialText, CColor? foreColor = null, CColor? backColor = null)
        {
            Text(initialText, foreColor, backColor);
        }

        /// <summary>
        ///     Adds the text to the color string with the specified foreground and background colors.
        ///     <para />
        ///     The colors are optional and if not specified, they retain whatever values they were
        ///     before.
        ///     <para />
        ///     Unless you want to dynamically specify the colors, prefer using the color-specified
        ///     methods like <see cref="Red" />.
        /// </summary>
        /// <param name="text">The text to add to the color string.</param>
        /// <param name="foreColor">Optional foreground color for the text.</param>
        /// <param name="backColor">Optional background color for the text.</param>
        /// <returns>The current instance of <see cref="ColorString" />.</returns>
        public ColorString Text(string text, CColor? foreColor = null, CColor? backColor = null)
        {
            if (foreColor.HasValue)
                _currentForeColor = foreColor.Value;
            if (backColor.HasValue)
                _currentBackColor = backColor.Value;

            if (text != null)
                _blocks.Add(new ColorStringBlock(text, _currentForeColor, _currentBackColor));

            return this;
        }

        /// <summary>
        ///     Resets the foreground color.
        /// </summary>
        /// <param name="text">
        ///     Optional text to append to the color string after resetting the foreground color.
        /// </param>
        /// <returns>The current instance of <see cref="ColorString" />.</returns>
        public ColorString Reset(string text = null)
        {
            return Text(text, CColor.Reset);
        }

        /// <summary>
        ///     Resets the background color.
        /// </summary>
        /// <param name="text">
        ///     Optional text to append to the color string after resetting the background color.
        /// </param>
        /// <returns>The current instance of <see cref="ColorString" />.</returns>
        public ColorString BgReset(string text = null)
        {
            return Text(text, foreColor: null, CColor.Reset);
        }

        public ColorString Black(string text = null)
        {
            return Text(text, CColor.Black);
        }

        public ColorString Blue(string text = null)
        {
            return Text(text, CColor.Blue);
        }

        public ColorString Cyan(string text = null)
        {
            return Text(text, CColor.Cyan);
        }

        public ColorString DkBlue(string text = null)
        {
            return Text(text, CColor.DkBlue);
        }

        public ColorString DkCyan(string text = null)
        {
            return Text(text, CColor.DkCyan);
        }

        public ColorString DkGray(string text = null)
        {
            return Text(text, CColor.DkGray);
        }

        public ColorString DkGreen(string text = null)
        {
            return Text(text, CColor.DkGreen);
        }

        public ColorString DkMagenta(string text = null)
        {
            return Text(text, CColor.DkMagenta);
        }

        public ColorString DkRed(string text = null)
        {
            return Text(text, CColor.DkRed);
        }

        public ColorString DkYellow(string text = null)
        {
            return Text(text, CColor.DkYellow);
        }

        public ColorString Gray(string text = null)
        {
            return Text(text, CColor.Gray);
        }

        public ColorString Green(string text = null)
        {
            return Text(text, CColor.Green);
        }

        public ColorString Magenta(string text = null)
        {
            return Text(text, CColor.Magenta);
        }

        public ColorString Red(string text = null)
        {
            return Text(text, CColor.Red);
        }

        public ColorString White(string text = null)
        {
            return Text(text, CColor.White);
        }

        public ColorString Yellow(string text = null)
        {
            return Text(text, CColor.Yellow);
        }

        public ColorString BgBlack(string text = null)
        {
            return Text(text, foreColor: null, CColor.Black);
        }

        public ColorString BgBlue(string text = null)
        {
            return Text(text, foreColor: null, CColor.Blue);
        }

        public ColorString BgCyan(string text = null)
        {
            return Text(text, foreColor: null, CColor.Cyan);
        }

        public ColorString BgDkBlue(string text = null)
        {
            return Text(text, foreColor: null, CColor.DkBlue);
        }

        public ColorString BgDkCyan(string text = null)
        {
            return Text(text, foreColor: null, CColor.DkCyan);
        }

        public ColorString BgDkGray(string text = null)
        {
            return Text(text, foreColor: null, CColor.DkGray);
        }

        public ColorString BgDkGreen(string text = null)
        {
            return Text(text, foreColor: null, CColor.DkGreen);
        }

        public ColorString BgDkMagenta(string text = null)
        {
            return Text(text, foreColor: null, CColor.DkMagenta);
        }

        public ColorString BgDkRed(string text = null)
        {
            return Text(text, foreColor: null, CColor.DkRed);
        }

        public ColorString BgDkYellow(string text = null)
        {
            return Text(text, foreColor: null, CColor.DkYellow);
        }

        public ColorString BgGray(string text = null)
        {
            return Text(text, foreColor: null, CColor.Gray);
        }

        public ColorString BgGreen(string text = null)
        {
            return Text(text, foreColor: null, CColor.Green);
        }

        public ColorString BgMagenta(string text = null)
        {
            return Text(text, foreColor: null, CColor.Magenta);
        }

        public ColorString BgRed(string text = null)
        {
            return Text(text, foreColor: null, CColor.Red);
        }

        public ColorString BgWhite(string text = null)
        {
            return Text(text, foreColor: null, CColor.White);
        }

        public ColorString BgYellow(string text = null)
        {
            return Text(text, foreColor: null, CColor.Yellow);
        }

        /// <summary>
        ///     Builds a string representing the text blocks defined in this <see cref="ColorString"/>.
        /// </summary>
        /// <returns>A color string.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (ColorStringBlock block in _blocks)
                sb.Append(block);
            return sb.ToString();
        }

        /// <summary>
        ///     Builds a string representing just the text from the text blocks defined in this
        ///     <see cref="ColorString"/>.
        /// </summary>
        /// <returns>
        ///     A string that is just the text from all the <see cref="ColorString"/> blocks.
        /// </returns>
        public string ToText()
        {
            return this.Aggregate(new StringBuilder(),
                (sb, block) => sb.Append(block.Text)).ToString();
        }

        /// <summary>
        ///     Creates an instance of a <see cref="ColorString" /> from the specified color string.
        /// </summary>
        /// <param name="cstr">The color string to parse.</param>
        /// <param name="colorStr">
        ///     The new <see cref="ColorString" /> instance created from the color string.
        /// </param>
        /// <returns>
        ///     True, if the color string could be parsed and a <see cref="ColorString" /> instance
        ///     created; otherwise false.
        /// </returns>
        public static bool TryParse(string cstr, out ColorString colorStr)
        {
            if (cstr is null)
            {
                colorStr = new ColorString(string.Empty);
                return true;
            }

            // Search the string for color blocks. If none are found, just return the string.
            MatchCollection matches = ColorStringPattern.Matches(cstr);
            if (matches.Count == 0)
            {
                colorStr = new ColorString(cstr);
                return true;
            }

            colorStr = new ColorString(cstr.Substring(0, matches[0].Index));
            for (var i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];

                string[] colorParts = match.Groups[1].Value.Split('.');
                CColor? foregroundColor = null;
                CColor? backgroundColor = null;
                foreach (string colorPart in colorParts)
                {
                    bool isBackground = colorPart.StartsWith("Bg", StringComparison.OrdinalIgnoreCase);
                    string actualColorStr = isBackground ? colorPart.Substring(2) : colorPart;
                    var color = (CColor)Enum.Parse(typeof(CColor), actualColorStr, ignoreCase: true);
                    if (isBackground)
                        backgroundColor = color;
                    else
                        foregroundColor = color;
                }

                int startIndex = match.Index + match.Length;
                int endIndex = i < matches.Count - 1 ? matches[i + 1].Index : cstr.Length;
                colorStr.Text(cstr.Substring(startIndex, endIndex - startIndex), foregroundColor, backgroundColor);
            }

            return true;
        }

        private static readonly Regex ColorStringPattern = new Regex(
            @"\[((?:Bg)?(?:Dk)?(?:Black|Blue|Cyan|Gray|Green|Magenta|Red|Yellow|White|Reset)(?:\.(?:Bg)?(?:Dk)?(?:Black|Blue|Cyan|Gray|Green|Magenta|Red|Yellow|White|Reset))*)\]",
            RegexOptions.IgnoreCase);

        public static implicit operator string(ColorString cstr)
        {
            return cstr.ToString();
        }

        public static implicit operator ColorString(string cstr)
        {
            return TryParse(cstr, out ColorString colorStr) ? colorStr : null;
        }

        public static readonly ColorString Empty = new();
    }

    // IReadOnlyList implementation
    public sealed partial class ColorString : IReadOnlyList<ColorStringBlock>
    {
        IEnumerator<ColorStringBlock> IEnumerable<ColorStringBlock>.GetEnumerator()
        {
            return _blocks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _blocks.GetEnumerator();
        }

        int IReadOnlyCollection<ColorStringBlock>.Count => _blocks.Count;

        ColorStringBlock IReadOnlyList<ColorStringBlock>.this[int index] => _blocks[index];
    }
}
