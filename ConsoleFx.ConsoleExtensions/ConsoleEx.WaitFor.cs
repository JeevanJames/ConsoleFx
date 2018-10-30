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
using System.Linq;

namespace ConsoleFx.ConsoleExtensions
{
    public static partial class ConsoleEx
    {
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
                key = Console.ReadKey(intercept: true).KeyChar;
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

        public static void WaitForKeysLoop(IEnumerable<KeyHandler> handlers,
            Action<ConsoleKey> postKeyPress = null, IEnumerable<ConsoleKey> escapeKeys = null)
        {
            if (handlers == null)
                throw new ArgumentNullException(nameof(handlers));
            if (!handlers.Any())
                throw new ArgumentException("Specify at least one handler.", nameof(handlers));

            if (escapeKeys == null)
                escapeKeys = new[] {ConsoleKey.Escape};

            if (handlers.Any(h => escapeKeys.Any(k => k == h.Key)))
                throw new ArgumentException("Specified escape keys should not be specified in the handlers.", nameof(escapeKeys));

            ConsoleKey key = Console.ReadKey(true).Key;
            while (!escapeKeys.Any(k => k == key))
            {
                KeyHandler handler = handlers.FirstOrDefault(h => h.Key == key);
                if (handler != null)
                {
                    handler.Action?.Invoke(key);
                    postKeyPress?.Invoke(key);
                }

                key = Console.ReadKey(true).Key;
            }
        }
    }

    /// <summary>
    /// Represents a handler for a key press in the <see cref="ConsoleEx.WaitForKeysLoop"/> method.
    /// </summary>
    public sealed class KeyHandler
    {
        public KeyHandler(ConsoleKey key, Action<ConsoleKey> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            Key = key;
            Action = action;
        }

        /// <summary>
        /// The key to handle.
        /// </summary>
        public ConsoleKey Key { get; }

        /// <summary>
        /// The handler to call when the key is pressed.
        /// </summary>
        public Action<ConsoleKey> Action { get; }
    }

    public static class ConsoleKeyExtensions
    {
        /// <summary>
        /// Provides a fluid way to create a <see cref="KeyHandler"/> instance when using the
        /// <see cref="ConsoleEx.WaitForKeysLoop"/> method.
        /// </summary>
        /// <param name="key">The key to handle.</param>
        /// <param name="action">The handler to call when the key is pressed.</param>
        /// <returns>An instance of the <see cref="KeyHandler"/> class.</returns>
        public static KeyHandler HandledBy(this ConsoleKey key, Action<ConsoleKey> action) =>
            new KeyHandler(key, action);
    }
}
