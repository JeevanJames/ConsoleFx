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
using System.Security;
using System.Text;

namespace ConsoleFx.ConsoleExtensions
{
    public static partial class ConsoleEx
    {
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
    }
}
