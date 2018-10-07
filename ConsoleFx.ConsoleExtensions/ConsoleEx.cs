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
using System.Linq;
using System.Security;
using System.Text;

namespace ConsoleFx.ConsoleExtensions
{
    public static class ConsoleEx
    {
        #region Prompt methods
    
        /// <summary>
        ///     Displays a message and waits for user input.
        /// </summary>
        /// <param name="message">A string or color string representing the message to be displayed</param>
        /// <returns>The input entered by the user</returns>
        public static string Prompt(ColorString message)
        {
            WriteColor(message);
            return Console.ReadLine();
        }

        #endregion

        #region Secret-reading methods
        /// <summary>
        ///     <para>The character to be used when entering a secret value using the ReadSecret methods. The default is '*'.</para>
        ///     <para>Changing this value applies globally.</para>
        /// </summary>
        public static char SecretMask { get; set; } = '*';

        /// <summary>
        ///     Reads a stream of characters from standard output, but obscures the entered characters
        ///     with a mask character.
        /// </summary>
        /// <param name="hideCursor">If true, hides the cursor while the characters are being input.</param>
        /// <param name="hideMask">If true, prevents the mask character from being shown.</param>
        /// <param name="needValue">If true, at least one character must be entered.</param>
        /// <returns>The entered stream of characters as a string.</returns>
        public static string ReadSecret(bool hideCursor = false, bool hideMask = false, bool needValue = false)
        {
            return ReadSecretCommon(
                () => new StringBuilder(),
                (sb, key) => sb.Append(key),
                sb => sb.Length,
                sb => sb.ToString(),
                hideCursor, hideMask, needValue
            );
        }

        /// <summary>
        ///     Displays a text prompt, and then reads a stream of characters from standard output,
        ///     but obscures the entered characters with a mask character.
        /// </summary>
        /// <param name="prompt">The text prompt to display.</param>
        /// <param name="hideCursor">If true, hides the cursor while the characters are being input.</param>
        /// <param name="hideMask">If true, prevents the mask character from being shown.</param>
        /// <param name="needValue">If true, at least one character must be entered.</param>
        /// <returns>The entered stream of characters as a string.</returns>
        public static string ReadSecret(ColorString prompt, bool hideCursor = false, bool hideMask = false, bool needValue = false)
        {
            WriteColor(prompt);
            return ReadSecret(hideCursor, hideMask, needValue);
        }

        public static SecureString ReadSecretSecure(bool hideCursor = false, bool hideMask = false, bool needValue = false)
        {
            return ReadSecretCommon(
                () => new SecureString(),
                (acc, key) => acc.AppendChar(key),
                acc => acc.Length,
                acc => acc,
                hideCursor, hideMask, needValue
            );
        }

        public static SecureString ReadSecretSecure(ColorString prompt, bool hideCursor = false, bool hideMask = false)
        {
            WriteColor(prompt);
            return ReadSecretSecure(hideCursor, hideMask);
        }

        private static TResult ReadSecretCommon<TResult, TAccumulator>(
            Func<TAccumulator> accumulatorFactory,
            Action<TAccumulator, char> accumulatorAppender,
            Func<TAccumulator, int> getAccumulatorLength,
            Func<TAccumulator, TResult> resultExtractor,
            bool hideCursor, bool hideMask, bool needValue
        )
        {
            bool wasCursorVisible = Console.CursorVisible;
            if (hideCursor)
                Console.CursorVisible = false;

            try
            {
                TAccumulator accumulator = accumulatorFactory();

                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                bool done = keyInfo.Key == ConsoleKey.Enter &&
                            ((needValue && getAccumulatorLength(accumulator) > 0) || !needValue);
                while (!done)
                {
                    if (keyInfo.Key != ConsoleKey.Enter)
                    {
                        accumulatorAppender(accumulator, keyInfo.KeyChar);
                        if (!hideMask)
                            Console.Write(SecretMask);
                        //Move the cursor only if hideCursor is false
                        else if (!hideCursor)
                            Console.Write(' ');
                    }

                    keyInfo = Console.ReadKey(true);
                    done = keyInfo.Key == ConsoleKey.Enter &&
                           ((needValue && getAccumulatorLength(accumulator) > 0) || !needValue);
                }

                Console.WriteLine();

                return resultExtractor(accumulator);
            }
            finally
            {
                if (hideCursor)
                    Console.CursorVisible = wasCursorVisible;
            }
        }
        #endregion

        #region WaitForXXXX methods
        /// <summary>
        ///     Waits for the user to press the ENTER (RETURN) key
        /// </summary>
        public static void WaitForEnter()
        {
            WaitForKeys(ConsoleKey.Enter);
        }

        /// <summary>
        ///     Waits for the user to press any key on the keyboard. Displays the character representing
        ///     the pressed key in the console window.
        /// </summary>
        /// <returns>Information about the pressed key</returns>
        public static ConsoleKeyInfo WaitForAnyKey()
        {
            return Console.ReadKey(intercept: true);
        }

        /// <summary>
        ///     Waits for any of a specified set of keys to be pressed by the user.
        /// </summary>
        /// <param name="keys">An array of characters representing the allowed set of characters.</param>
        /// <returns>The character pressed by the user</returns>
        public static char WaitForKeys(params char[] keys)
        {
            char key;
            do
            {
                key = Console.ReadKey(true).KeyChar;
            } while (Array.IndexOf(keys, key) < 0);
            return key;
        }

        /// <summary>
        ///     Waits for any of a specified set of keys to be pressed by the user.
        /// </summary>
        /// <param name="keys">An array of characters representing the allowed set of characters.</param>
        /// <returns>The character pressed by the user</returns>
        public static char WaitForKeys(bool ignoreCase, params char[] keys)
        {
            char[] casedKeys = ignoreCase ? keys.Select(k => char.ToUpperInvariant(k)).ToArray() : keys;
            int keyIndex;
            do
            {
                char key = char.ToUpperInvariant(Console.ReadKey(intercept: true).KeyChar);
                keyIndex = Array.IndexOf(casedKeys, key);
            } while (keyIndex < 0);
            return keys[keyIndex];
        }
        /// <summary>
        ///     Waits for any of a specified set of keys to be pressed by the user.
        /// </summary>
        /// <param name="keys">An array of ConsoleKey objects representing the allowed set of keys.</param>
        /// <returns>The key pressed by the user</returns>
        public static ConsoleKey WaitForKeys(params ConsoleKey[] keys)
        {
            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
            } while (Array.IndexOf(keys, key) < 0);
            return key;
        }
        #endregion

        #region Write & WriteLine methods
        public static void WriteColor(ColorString message)
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

        public static void WriteLineColor(ColorString message)
        {
            WriteColor(message);
            Console.WriteLine();
        }

        /// <summary>
        ///     Writes a blank line to the Console. Just a more descriptive way to do a Console.WriteLine().
        /// </summary>
        public static void WriteBlankLine()
        {
            Console.WriteLine();
        }

        /// <summary>
        ///     Writes multiple lines to the console.
        /// </summary>
        /// <param name="lines">The lines to write.</param>
        public static void WriteLines(params ColorString[] lines)
        {
            foreach (ColorString line in lines)
                WriteLineColor(line);
        }

        /// <summary>
        ///     Writes multiple lines to the console, with each line being left-aligned to the specified indent.
        /// </summary>
        /// <param name="indent">The indent to left align each line.</param>
        /// <param name="lines">The lines to write.</param>
        public static void WriteLines(int indent, params string[] lines)
        {
            foreach (string line in lines)
                WriteIndented(line, indent, true);
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
        public static void WriteIndented(string text, int indent, bool indentFirstLine = false)
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
        #endregion

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
