#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015 Jeevan James

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
using System.Security;
using System.Text;

using static System.Console;

namespace ConsoleFx.Utilities
{
    public static class ConsoleEx
    {
        static ConsoleEx()
        {
            SecretMask = '*';
        }

        #region Prompt methods
        /// <summary>
        /// Displays a message and waits for user input.
        /// </summary>
        /// <param name="message">A composite format string representing the message to be displayed</param>
        /// <param name="args">An array of objects to write using message</param>
        /// <returns>The input entered by the user</returns>
        public static string Prompt(string message, params object[] args)
        {
            Console.Write(message, args);
            return ReadLine();
        }

        /// <summary>
        /// Displays a message in the specific foreground and background colors and waits for user input.
        /// </summary>
        /// <param name="foreColor">The foreground color to display the message. Specify null to use the default foreground color.</param>
        /// <param name="backColor">The background color to display the message. Specify null to use the default background color.</param>
        /// <param name="message">A composite format string representing the message to be displayed</param>
        /// <param name="args">An array of objects to write using message</param>
        /// <returns>The input entered by the user</returns>
        public static string Prompt(ConsoleColor? foreColor, ConsoleColor? backColor, string message, params object[] args)
        {
            Write(foreColor, backColor, message, args);
            return ReadLine();
        }
        #endregion

        #region ReadSecret methods
        /// <summary>
        /// <para>The character to be used when entering a secret value using the ReadSecret methods. The default is '*'.</para>
        /// <para>Changing this value applies globally.</para>
        /// </summary>
        /// <remarks>
        /// <para>You can specify a null character (\x0) to prevent any output from displaying and the cursor
        /// from moving as characters are typed.</para>
        /// <para>Alternatively, you can specify a space character to prevent any output from displaying,
        /// although the cursor will move with the characters typed.</para>
        /// </remarks>
        public static char SecretMask { get; set; }

        public static string ReadSecret(string prompt, params object[] args)
        {
            Console.Write(prompt, args);

            var result = new StringBuilder();

            ConsoleKeyInfo keyInfo = ReadKey(true);
            while (keyInfo.Key != ConsoleKey.Enter)
            {
                result.Append(keyInfo.KeyChar);
                Console.Write(SecretMask);
                keyInfo = ReadKey(true);
            }
            Console.WriteLine();

            return result.ToString();
        }

        public static string ReadSecret()
        {
            return ReadSecret(string.Empty);
        }

        public static SecureString ReadSecretSecure(string prompt, params object[] args)
        {
            Console.WriteLine(prompt, args);

            var secret = new SecureString();

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            while (keyInfo.Key != ConsoleKey.Enter)
            {
                if (keyInfo.Key != ConsoleKey.Backspace)
                {
                    secret.AppendChar(keyInfo.KeyChar);
                    Console.Write(SecretMask);
                }
                else if (secret.Length > 0)
                {
                    secret.RemoveAt(secret.Length - 1);
                    Console.Write(keyInfo.KeyChar);
                    Console.Write(" ");
                    Console.Write(keyInfo.KeyChar);
                }
                keyInfo = Console.ReadKey(true);
            }

            Console.WriteLine();
            secret.MakeReadOnly();
            return secret;
        }

        public static SecureString ReadSecretSecure()
        {
            return ReadSecretSecure(string.Empty);
        }
        #endregion

        #region WaitForXXXX methods
        /// <summary>
        /// Waits for the user to press the ENTER (RETURN) key
        /// </summary>
        public static void WaitForEnter()
        {
            WaitForKeys(ConsoleKey.Enter);
        }

        /// <summary>
        /// Waits for the user to press any key on the keyboard. Displays the character representing
        /// the pressed key in the console window.
        /// </summary>
        /// <returns>Information about the pressed key</returns>
        public static ConsoleKeyInfo WaitForAnyKey()
        {
            return Console.ReadKey(true);
        }

        /// <summary>
        /// Waits for any of a specified set of keys to be pressed by the user.
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
        /// Waits for any of a specified set of keys to be pressed by the user.
        /// </summary>
        /// <param name="keys">An array of ConsoleKey objects representing the allowed set of keys.</param>
        /// <returns>The key pressed by the user</returns>
        public static ConsoleKey WaitForKeys(params ConsoleKey[] keys)
        {
            ConsoleKey key;
            do
            {
                key = ReadKey(true).Key;
            } while (Array.IndexOf(keys, key) < 0);
            return key;
        }
        #endregion

        #region Write & WriteLine methods
        /// <summary>
        /// Displays a string on the Console using the specified foreground and background colors. Similar
        /// to the Console.Write method.
        /// </summary>
        /// <param name="foreColor">The foreground color to display the message. Specify null to use the default foreground color.</param>
        /// <param name="backColor">The background color to display the message. Specify null to use the default background color.</param>
        /// <param name="text">A composite format string representing the message to be displayed</param>
        /// <param name="args">An array of objects to write using message</param>
        public static void Write(ConsoleColor? foreColor, ConsoleColor? backColor, string text, params object[] args)
        {
            if (foreColor.HasValue)
                ForegroundColor = foreColor.Value;
            if (backColor.HasValue)
                BackgroundColor = backColor.Value;
            Console.Write(text, args);
            ResetColor();
        }

        public static void WriteLine(ConsoleColor? foreColor, ConsoleColor? backColor, string text, params object[] args)
        {
            Write(foreColor, backColor, text, args);
            Console.WriteLine();
        }

        /// <summary>
        /// Writes a blank line to the Console. Just a more descriptive way to do a Console.WriteLine().
        /// </summary>
        public static void WriteBlankLine()
        {
            Console.WriteLine();
        }

        public static void WriteLines(params string[] lines)
        {
            foreach (string line in lines)
                Console.WriteLine(line);
        }

        public static void WriteIndented(string text, int indent, bool indentFirstLine = false)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            var indentStr = new string(' ', indent);
            int lineWidth = WindowWidth - indent - 1;

            int startPos = 0;
            while (startPos < text.Length)
            {
                int length = Math.Min(lineWidth, text.Length - startPos);
                string str = text.Substring(startPos, length);
                if (startPos > 0)
                    Console.Write(indentStr);
                Console.WriteLine(str);
                startPos += lineWidth;
            }
        }
        #endregion
    }
}