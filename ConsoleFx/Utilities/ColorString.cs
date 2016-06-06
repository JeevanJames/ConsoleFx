#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015-2016 Jeevan James

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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleFx.Utilities
{
    public sealed class ColorString
    {
        private ConsoleColor? _currentForeColor;
        private ConsoleColor? _currentBackColor;

        public ColorString Color(ConsoleColor foreColor, ConsoleColor backColor)
        {
            _currentForeColor = foreColor;
            _currentBackColor = backColor;
            return this;
        }

        public ColorString ForeColor(ConsoleColor foreColor)
        {
            _currentForeColor = foreColor;
            return this;
        }

        public ColorString BackColor(ConsoleColor backColor)
        {
            _currentBackColor = backColor;
            return this;
        }

        public ColorString Text(string text, params object[] args)
        {
            Blocks.Add(new ColorStringBlock(text, _currentForeColor, _currentBackColor));
            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (ColorStringBlock block in Blocks)
            {
                if (block.ForeColor.HasValue || block.BackColor.HasValue)
                {
                    sb.Append("[");
                    if (block.ForeColor.HasValue)
                        sb.Append(block.ForeColor.Value);
                    if (block.BackColor.HasValue)
                        sb.Append("|").Append(block.BackColor.Value);
                    sb.Append("]");
                }
                sb.Append(block.Text);
            }
            return sb.ToString();
        }

        internal List<ColorStringBlock> Blocks { get; } = new List<ColorStringBlock>();

        public static implicit operator string(ColorString cstr) => cstr.ToString();

        public static implicit operator ColorString(string cstr)
        {
            ColorString colorStr;
            return TryParse(cstr, out colorStr) ? colorStr : null;
        }

        public static bool TryParse(string cstr, out ColorString colorStr)
        {
            colorStr = new ColorString();

            MatchCollection matches = ColorStringPattern.Matches(cstr);
            if (matches.Count == 0)
            {
                colorStr.Text(cstr);
                return true;
            }

            colorStr = new ColorString();
            for (int i = 0; i < matches.Count; i++)
            {
                int startIndex = i == 0 ? 0 : matches[i - 1].Index + matches[i - 1].Length;
                int endIndex = matches[i].Index;
                colorStr.Text(cstr.Substring(startIndex, endIndex - startIndex));

                var foreColor = (ConsoleColor?) (matches[i].Groups[1].Length > 0
                    ? Enum.Parse(typeof(ConsoleColor), matches[i].Groups[1].Value, true)
                    : null);
                var backColor = (ConsoleColor?) (matches[i].Groups[2].Length > 0
                    ? Enum.Parse(typeof(ConsoleColor), matches[i].Groups[2].Value, true)
                    : null);
                if (foreColor.HasValue && backColor.HasValue)
                    colorStr.Color(foreColor.Value, backColor.Value);
                else if (foreColor.HasValue)
                    colorStr.ForeColor(foreColor.Value);
                else if (backColor.HasValue)
                    colorStr.BackColor(backColor.Value);
            }

            Match lastMatch = matches[matches.Count - 1];
            colorStr.Text(cstr.Substring(lastMatch.Index + lastMatch.Length));

            return true;
        }

        private static readonly Regex ColorStringPattern = new Regex(@"\[(\w+)?(?:\|(\w+))?\]");
    }

    internal sealed class ColorStringBlock
    {
        internal ColorStringBlock(string text, ConsoleColor? foreColor = null, ConsoleColor? backColor = null)
        {
            Text = text;
            ForeColor = foreColor;
            BackColor = backColor;
        }

        internal string Text { get; }

        internal ConsoleColor? ForeColor { get; }

        internal ConsoleColor? BackColor { get; }
    }
}
