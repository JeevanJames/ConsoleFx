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
using System.Collections.Generic;

namespace ConsoleFx.ConsoleExtensions
{
    public static partial class ConsoleEx
    {
        /// <summary>
        /// Writes a <see cref="ColorString"/> object to the console.
        /// </summary>
        /// <param name="message">The <see cref="ColorString"/> object to write.</param>
        public static void Print(ColorString message)
        {
            foreach (ColorStringBlock block in message)
            {
                if (block.ForeColor.HasValue)
                    Console.ForegroundColor = ColorMappings[block.ForeColor.Value];
                if (block.BackColor.HasValue)
                    Console.BackgroundColor = ColorMappings[block.BackColor.Value];
                Console.Write(block.Text);
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Writes one or more <see cref="ColorString"/> objects to the console.
        /// </summary>
        /// <param name="messages">The <see cref="ColorString"/> objects to write.</param>
        public static void PrintLine(params ColorString[] messages)
        {
            foreach (ColorString message in messages)
            {
                Print(message);
                Console.WriteLine();
            }
        }

        /// <summary>
        ///     Writes one or more blank lines to the Console.
        ///     Just a more descriptive way to do a <code>Console.WriteLine()</code>.
        /// </summary>
        public static void PrintBlank(int count = 1)
        {
            for (int i = 0; i < count; i++)
                Console.WriteLine();
        }

        /// <summary>
        ///     Writes multiple lines to the console, with each line being left-aligned to the specified indent.
        /// </summary>
        /// <param name="indent">The indent to left align each line.</param>
        /// <param name="lines">The lines to write.</param>
        public static void PrintIndented(int indent, params string[] lines)
        {
            foreach (string line in lines)
                PrintIndented(line, indent, true);
        }

        /// <summary>
        ///     Writes a long piece of text to the console such that each new line is left-aligned to the same indent.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="indent">The indent to left align the text.</param>
        /// <param name="indentFirstLine">
        ///     Whether the first line should be indented or just written from the current cursor
        ///     position.
        /// </param>
        public static void PrintIndented(string text, int indent, bool indentFirstLine = false)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var indentStr = new string(' ', indent);
            int lineWidth = Console.WindowWidth - indent - 1;

            var startPos = 0;
            while (startPos < text.Length)
            {
                int length = Math.Min(lineWidth, text.Length - startPos);
                string str = text.Substring(startPos, length);
                if (startPos > 0 || indentFirstLine)
                    Console.Write(indentStr);
                Console.WriteLine(str);
                startPos += lineWidth;
            }
        }

        private static readonly Dictionary<CColor, ConsoleColor> ColorMappings = new Dictionary<CColor, ConsoleColor>
        {
            [CColor.Black] = ConsoleColor.Black,
            [CColor.Blue] = ConsoleColor.Blue,
            [CColor.Cyan] = ConsoleColor.Cyan,
            [CColor.DkBlue] = ConsoleColor.DarkBlue,
            [CColor.DkCyan] = ConsoleColor.DarkCyan,
            [CColor.DkGray] = ConsoleColor.DarkGray,
            [CColor.DkGreen] = ConsoleColor.DarkGreen,
            [CColor.DkMagenta] = ConsoleColor.DarkMagenta,
            [CColor.DkRed] = ConsoleColor.DarkRed,
            [CColor.DkYellow] = ConsoleColor.DarkYellow,
            [CColor.Gray] = ConsoleColor.Gray,
            [CColor.Green] = ConsoleColor.Green,
            [CColor.Magenta] = ConsoleColor.Magenta,
            [CColor.Red] = ConsoleColor.Red,
            [CColor.White] = ConsoleColor.White,
            [CColor.Yellow] = ConsoleColor.Yellow,

            [CColor.BgBlack] = ConsoleColor.Black,
            [CColor.BgBlue] = ConsoleColor.Blue,
            [CColor.BgCyan] = ConsoleColor.Cyan,
            [CColor.BgDkBlue] = ConsoleColor.DarkBlue,
            [CColor.BgDkCyan] = ConsoleColor.DarkCyan,
            [CColor.BgDkGray] = ConsoleColor.DarkGray,
            [CColor.BgDkGreen] = ConsoleColor.DarkGreen,
            [CColor.BgDkMagenta] = ConsoleColor.DarkMagenta,
            [CColor.BgDkRed] = ConsoleColor.DarkRed,
            [CColor.BgDkYellow] = ConsoleColor.DarkYellow,
            [CColor.BgGray] = ConsoleColor.Gray,
            [CColor.BgGreen] = ConsoleColor.Green,
            [CColor.BgMagenta] = ConsoleColor.Magenta,
            [CColor.BgRed] = ConsoleColor.Red,
            [CColor.BgWhite] = ConsoleColor.White,
            [CColor.BgYellow] = ConsoleColor.Yellow,
        };
    }
}
