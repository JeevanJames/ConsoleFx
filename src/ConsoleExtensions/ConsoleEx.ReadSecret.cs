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
using System.Security;
using System.Text;

namespace ConsoleFx.ConsoleExtensions
{
    public static partial class ConsoleEx
    {
        /// <summary>
        ///     Reads a stream of characters from standard output, but obscures the entered characters
        ///     with a mask character.
        /// </summary>
        /// <param name="hideCursor">If <c>true</c>, hides the cursor while the characters are being input.</param>
        /// <param name="hideMask">If <c>true</c>, prevents the mask character from being shown.</param>
        /// <param name="needValue">If <c>true</c>, at least one character must be entered.</param>
        /// <returns>The entered stream of characters as a string.</returns>
        public static string ReadSecret(bool hideCursor = false, bool hideMask = false, bool needValue = false)
        {
            return ReadSecretCommon(() => new StringBuilder(),
                (sb, key) => sb.Append(key),
                sb => sb.Length,
                sb => sb.ToString(),
                hideCursor, hideMask, needValue);
        }

        /// <summary>
        ///     Displays a text prompt, and then reads a stream of characters from standard output,
        ///     but obscures the entered characters with a mask character.
        /// </summary>
        /// <param name="prompt">The text prompt to display.</param>
        /// <param name="hideCursor">If <c>true</c>, hides the cursor while the characters are being input.</param>
        /// <param name="hideMask">If <c>true</c>, prevents the mask character from being shown.</param>
        /// <param name="needValue">If <c>true</c>, at least one character must be entered.</param>
        /// <returns>The entered stream of characters as a string.</returns>
        public static string ReadSecret(ColorString prompt, bool hideCursor = false, bool hideMask = false,
            bool needValue = false)
        {
            Print(prompt);
            return ReadSecret(hideCursor, hideMask, needValue);
        }

        /// <summary>
        ///     Reads a stream of characters from standard output, but obscures the entered characters
        ///     with a mask character.
        /// </summary>
        /// <param name="hideCursor">If <c>true</c>, hides the cursor while the characters are being input.</param>
        /// <param name="hideMask">If <c>true</c>, prevents the mask character from being shown.</param>
        /// <param name="needValue">If <c>true</c>, at least one character must be entered.</param>
        /// <returns>The entered stream of characters as a <see cref="SecureString"/>.</returns>
        public static SecureString ReadSecretSecure(bool hideCursor = false, bool hideMask = false,
            bool needValue = false)
        {
            return ReadSecretCommon(() => new SecureString(),
                (acc, key) => acc.AppendChar(key),
                acc => acc.Length,
                acc => acc,
                hideCursor, hideMask, needValue);
        }

        /// <summary>
        ///     Displays a text prompt, and then reads a stream of characters from standard output,
        ///     but obscures the entered characters with a mask character.
        /// </summary>
        /// <param name="prompt">The text prompt to display.</param>
        /// <param name="hideCursor">If <c>true</c>, hides the cursor while the characters are being input.</param>
        /// <param name="hideMask">If <c>true</c>, prevents the mask character from being shown.</param>
        /// <param name="needValue">If <c>true</c>, at least one character must be entered.</param>
        /// <returns>The entered stream of characters as a <see cref="SecureString"/>.</returns>
        public static SecureString ReadSecretSecure(ColorString prompt, bool hideCursor = false, bool hideMask = false,
            bool needValue = false)
        {
            Print(prompt);
            return ReadSecretSecure(hideCursor, hideMask, needValue);
        }

        /// <summary>
        ///     Common method used by both the <see cref="ReadSecret(bool,bool,bool)" /> and
        ///     <see cref="ReadSecretSecure(bool,bool,bool)" /> methods to receive a secret input
        ///     from the user.
        /// </summary>
        /// <typeparam name="TResult">The type of the resultant secret.</typeparam>
        /// <typeparam name="TAccumulator">
        ///     The type of the accumulator, used to incrementally build the resultant secret (could be
        ///     the same type as the result).
        /// </typeparam>
        /// <param name="accumulatorFactory">Function to create the accumulator type.</param>
        /// <param name="accumulatorAppender">Function to append a single entered character to the accumulator.</param>
        /// <param name="getAccumulatorLength">Function to retrieve the current length of the accumulator.</param>
        /// <param name="resultExtractor">Function to extract the final result from the accumulator.</param>
        /// <param name="hideCursor">If <c>true</c>, hides the cursor while the characters are being input.</param>
        /// <param name="hideMask">If <c>true</c>, prevents the mask character from being shown.</param>
        /// <param name="needValue">If <c>true</c>, at least one character must be entered.</param>
        /// <returns>The entered data.</returns>
        private static TResult ReadSecretCommon<TResult, TAccumulator>(Func<TAccumulator> accumulatorFactory,
            Action<TAccumulator, char> accumulatorAppender,
            Func<TAccumulator, int> getAccumulatorLength,
            Func<TAccumulator, TResult> resultExtractor,
            bool hideCursor, bool hideMask, bool needValue)
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
                            Console.Write(Settings.SecretMask);

                        //Move the cursor only if hideCursor is false
                        else if (!hideCursor)
                            Console.Write(value: ' ');
                    }

                    keyInfo = Console.ReadKey(intercept: true);
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
