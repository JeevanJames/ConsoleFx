// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.ConsoleExtensions
{
    public static partial class ConsoleEx
    {
        /// <summary>
        ///     Clears the contents of the current line and resets the cursor position to the start
        ///     of the line.
        /// </summary>
        public static void ClearCurrentLine()
        {
            int currentLine = Console.CursorTop;
            Console.SetCursorPosition(0, currentLine);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLine);
        }

        /// <summary>
        ///     Clears the contents of the specified <paramref name="line"/> and resets the cursor to
        ///     its original position.
        /// </summary>
        /// <param name="line">The index of the line to clear.</param>
        public static void ClearLine(int line)
        {
            var (cursorLeft, cursorTop) = (Console.CursorLeft, Console.CursorTop);
            try
            {
                Console.SetCursorPosition(0, line);
                Console.Write(new string(' ', Console.WindowWidth));
            }
            finally
            {
                Console.SetCursorPosition(cursorLeft, cursorTop);
            }
        }

        private static void DoAndReturnToOriginalPosition(Action action)
        {
            var state = (Console.CursorLeft, Console.CursorTop, Console.CursorVisible);
            try
            {
                action();
            }
            finally
            {
                Console.SetCursorPosition(state.CursorLeft, state.CursorTop);
                Console.CursorVisible = state.CursorVisible;
            }
        }
    }
}
