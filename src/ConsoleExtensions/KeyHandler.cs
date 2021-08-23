// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.ConsoleExtensions
{
    /// <summary>
    ///     Represents a handler for a key press in the <see cref="ConsoleEx.WaitForKeysLoop" /> method.
    /// </summary>
    public sealed class KeyHandler
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="KeyHandler"/> class with a specified
        ///     <paramref name="key"/> and <paramref name="action"/> to take when the key is pressed.
        /// </summary>
        /// <param name="key">The key to handle.</param>
        /// <param name="action">The action to perform if the key is pressed.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the <paramref name="action"/> is <c>null</c>.
        /// </exception>
        public KeyHandler(ConsoleKey key, Action<ConsoleKey> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));
            Key = key;
            Action = action;
        }

        /// <summary>
        ///     Gets the key to handle.
        /// </summary>
        public ConsoleKey Key { get; }

        /// <summary>
        ///     Gets the handler to call when the key is pressed.
        /// </summary>
        public Action<ConsoleKey> Action { get; }
    }
}
