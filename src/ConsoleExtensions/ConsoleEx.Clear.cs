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
