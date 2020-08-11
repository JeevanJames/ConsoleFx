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
using System.Collections.Generic;
using System.Linq;

namespace ConsoleFx.ConsoleExtensions
{
    public static partial class ConsoleEx
    {
        /// <summary>
        ///     Waits for the user to press the ENTER (RETURN) key.
        /// </summary>
        public static void WaitForEnter()
        {
            WaitForKeys(ConsoleKey.Enter);
        }

        /// <summary>
        ///     Waits for the user to press any key on the keyboard. Displays the character representing
        ///     the pressed key in the console window.
        /// </summary>
        /// <returns>Information about the pressed key.</returns>
        public static ConsoleKeyInfo WaitForAnyKey()
        {
            return Console.ReadKey(intercept: true);
        }

        /// <summary>
        ///     Waits for any of a specified set of <paramref name="keys"/> to be pressed by the user.
        /// </summary>
        /// <param name="keys">An array of characters representing the allowed set of characters.</param>
        /// <returns>The character pressed by the user.</returns>
        public static char WaitForKeys(params char[] keys)
        {
            char key;
            do
                key = Console.ReadKey(intercept: true).KeyChar;
            while (Array.IndexOf(keys, key) < 0);
            return key;
        }

        /// <summary>
        ///     Waits for any of a specified set of <paramref name="keys"/> to be pressed by the user.
        /// </summary>
        /// <param name="ignoreCase">Indicates whether to the keys pressed are case sensitive.</param>
        /// <param name="keys">An array of characters representing the allowed set of characters.</param>
        /// <returns>The character pressed by the user.</returns>
        public static char WaitForKeys(bool ignoreCase, params char[] keys)
        {
            char[] casedKeys = ignoreCase ? keys.Select(char.ToUpperInvariant).ToArray() : keys;
            int keyIndex;
            do
            {
                char key = char.ToUpperInvariant(Console.ReadKey(intercept: true).KeyChar);
                keyIndex = Array.IndexOf(casedKeys, key);
            }
            while (keyIndex < 0);

            return keys[keyIndex];
        }

        /// <summary>
        ///     Waits for any of a specified set of <paramref name="keys"/> to be pressed by the user.
        /// </summary>
        /// <param name="keys">
        ///     An array of <see cref="ConsoleKey"/> objects representing the allowed set of keys.
        /// </param>
        /// <returns>The key pressed by the user.</returns>
        public static ConsoleKey WaitForKeys(params ConsoleKey[] keys)
        {
            ConsoleKey key;
            do
                key = Console.ReadKey(intercept: true).Key;
            while (Array.IndexOf(keys, key) < 0);
            return key;
        }

        /// <summary>
        ///     Repeatedly prompts the user for a key press, until any of the specified escape keys
        ///     are pressed.
        ///     <para/>
        ///     If the pressed key if available in the specified handlers, the corresponding handler
        ///     is called. If not, it is ignored and the loop continues.
        /// </summary>
        /// <param name="handlers">Collection of keys to handle and their handlers.</param>
        /// <param name="postKeyPress">
        ///     Optional action to run after any key press, not counting the ignored keys and escape
        ///     keys.
        /// </param>
        /// <param name="escapeKeys">
        ///     The keys that will break the loop. If not specified, defaults to the escape key.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="handlers"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if the <paramref name="handlers"/> collection is empty or if it contains any of
        ///     the <paramref name="escapeKeys"/> characters.
        /// </exception>
        public static void WaitForKeysLoop(IEnumerable<KeyHandler> handlers,
            Action<ConsoleKey> postKeyPress = null, IEnumerable<ConsoleKey> escapeKeys = null)
        {
            if (handlers is null)
                throw new ArgumentNullException(nameof(handlers));

            List<KeyHandler> handlersList = handlers.ToList();

            if (handlersList.Count == 0)
                throw new ArgumentException("Specify at least one handler.", nameof(handlers));

            // If the escapeKeys parameter is not specified, default it to the Escape key.
            if (escapeKeys is null)
                escapeKeys = new[] { ConsoleKey.Escape };

            // Ensure that none of the escape keys are specified in the handlers.
            if (handlersList.Any(h => escapeKeys.Any(k => k == h.Key)))
            {
                throw new ArgumentException("Specified escape keys should not be specified in the handlers.",
                    nameof(escapeKeys));
            }

            List<ConsoleKey> escapeKeysList = escapeKeys.ToList();
            ConsoleKey key = Console.ReadKey(intercept: true).Key;
            while (!escapeKeysList.Any(k => k == key))
            {
                KeyHandler handler = handlersList.FirstOrDefault(h => h.Key == key);
                if (handler != null)
                {
                    handler.Action?.Invoke(key);
                    postKeyPress?.Invoke(key);
                }

                key = Console.ReadKey(intercept: true).Key;
            }
        }
    }
}
