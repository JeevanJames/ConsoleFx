// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.ConsoleExtensions
{
    public static class ConsoleKeyExtensions
    {
        /// <summary>
        ///     Provides a fluid way to create a <see cref="KeyHandler" /> instance when using the
        ///     <see cref="ConsoleEx.WaitForKeysLoop" /> method.
        /// </summary>
        /// <param name="key">The key to handle.</param>
        /// <param name="action">The handler to call when the key is pressed.</param>
        /// <returns>An instance of the <see cref="KeyHandler" /> class.</returns>
        public static KeyHandler HandledBy(this ConsoleKey key, Action<ConsoleKey> action)
        {
            return new KeyHandler(key, action);
        }
    }
}
