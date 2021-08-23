// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.ConsoleExtensions
{
    public static partial class ConsoleEx
    {
        /// <summary>
        ///     Displays a message and waits for user input.
        /// </summary>
        /// <param name="message">
        ///     A string or color string representing the message to be displayed.
        /// </param>
        /// <returns>The input entered by the user.</returns>
        public static string Prompt(ColorString message)
        {
            Print(message);
            return Console.ReadLine();
        }

        /// <summary>
        ///     Accepts user input and validates the input with the specified validator.
        ///     <para/>
        ///     If the input is not valid, the entered text is cleared and user prompted to enter
        ///     the input again.
        /// </summary>
        /// <param name="validator">Function to validate the input text.</param>
        /// <returns>The input entered by the user.</returns>
        public static string Prompt(Func<string, bool> validator)
        {
            (int left, int top) = (Console.CursorLeft, Console.CursorTop);

            string input = Console.ReadLine();
            bool isValid = validator(input);
            while (!isValid)
            {
                Console.SetCursorPosition(left, top);
                Console.Write(new string(c: ' ', input.Length));
                Console.SetCursorPosition(left, top);
                input = Console.ReadLine();
                isValid = validator(input);
            }

            return input;
        }

        /// <summary>
        ///     Displays a <paramref name="message"/> and waits for user input. The input is validated
        ///     using the specified <paramref name="validator"/>.
        ///     <para/>
        ///     If the input is not valid, the entered text is cleared and user prompted to enter
        ///     the input again.
        /// </summary>
        /// <param name="message">
        ///     A string or color string representing the message to be displayed.
        /// </param>
        /// <param name="validator">Function to validate the input text.</param>
        /// <returns>The input entered by the user.</returns>
        public static string Prompt(ColorString message, Func<string, bool> validator)
        {
            Print(message);
            return Prompt(validator);
        }
    }
}
