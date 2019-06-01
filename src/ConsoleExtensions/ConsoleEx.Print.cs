#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2019 Jeevan James

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
        ///     Writes a <see cref="ColorString" /> object to the console.
        /// </summary>
        /// <param name="message">The <see cref="ColorString" /> object to write.</param>
        public static void Print(ColorString message)
        {
            foreach (ColorStringBlock block in message)
            {
                SetupColorsForBlockPrinting(block);
                Console.Write(block.Text);

                if (Settings.ColorReset == ColorResetOption.ResetAfterColor)
                    Console.ResetColor();
            }

            if (Settings.ColorReset == ColorResetOption.ResetAfterCommand)
                Console.ResetColor();
        }

        /// <summary>
        ///     Writes one or more <see cref="ColorString" /> objects to the console.
        /// </summary>
        /// <param name="messages">The <see cref="ColorString" /> objects to write.</param>
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
        ///     <para />
        ///     Just a more descriptive way to do a Console.WriteLine().
        /// </summary>
        /// <param name="count">The number of blank lines to write.</param>
        public static void PrintBlank(int count = 1)
        {
            for (var i = 0; i < count; i++)
                Console.WriteLine();
        }

        /// <summary>
        ///     Writes multiple lines to the console, with each line being left-aligned to the specified
        ///     indent.
        /// </summary>
        /// <param name="indent">The indent to left align each line.</param>
        /// <param name="lines">The lines to write.</param>
        public static void PrintIndented(int indent, params string[] lines)
        {
            foreach (string line in lines)
                PrintIndented(line, indent, indentFirstLine: true);
        }

        /// <summary>
        ///     Writes a long piece of text to the console such that each new line is left-aligned
        ///     to the same indent.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="indent">The indent to left align the text.</param>
        /// <param name="indentFirstLine">
        ///     Whether the first line should be indented or just written from the current cursor
        ///     position.
        /// </param>
        public static void PrintIndented(ColorString text, int indent, bool indentFirstLine = false)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            var indentStr = new string(c: ' ', indent);
            int lineWidth = Console.WindowWidth - indent - 1;

            if (indentFirstLine)
                Console.Write(indentStr);

            // Tracks the length of the strings printed on the current line.
            // Once the length crosses the line width, it is reset and we move to the next line.
            int length = 0;

            foreach (ColorStringBlock block in text)
            {
                string[] parts = block.Text.Split(new[] { ' ' }, StringSplitOptions.None);
                for (int i = 0; i < parts.Length; i++)
                {
                    // If the current length of the printed line plus the length of the next part if greater
                    // than the line width, we need to start the next part on the next line.
                    // Write a new line and the indent. Reset length to 0;
                    if (length + parts[i].Length > lineWidth)
                    {
                        Console.WriteLine();
                        Console.Write(indentStr);
                        length = 0;
                    }

                    // Write the part to console and increment length by its length.
                    SetupColorsForBlockPrinting(block);
                    Print(parts[i]);
                    length += parts[i].Length;

                    // Except for the last part, write the separating space character as well.
                    // Increment length by 1.
                    if (i < parts.Length - 1)
                    {
                        SetupColorsForBlockPrinting(block);
                        Console.Write(" ");
                        length++;
                    }
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        ///     Helpers method to setup the console's colors from a <see cref="ColorStringBlock"/> instance.
        /// </summary>
        /// <param name="block">The <see cref="ColorStringBlock"/> instance.</param>
        private static void SetupColorsForBlockPrinting(ColorStringBlock block)
        {
            if (block.ForeColor.HasValue)
            {
                if (block.ForeColor.Value == CColor.Reset)
                {
                    ConsoleColor backColor = Console.BackgroundColor;
                    Console.ResetColor();
                    Console.BackgroundColor = backColor;
                }
                else
                    Console.ForegroundColor = ColorMappings[block.ForeColor.Value];
            }

            if (block.BackColor.HasValue)
            {
                if (block.BackColor.Value == CColor.BgReset)
                {
                    ConsoleColor foreColor = Console.ForegroundColor;
                    Console.ResetColor();
                    Console.ForegroundColor = foreColor;
                }
                else
                    Console.BackgroundColor = ColorMappings[block.BackColor.Value];
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
