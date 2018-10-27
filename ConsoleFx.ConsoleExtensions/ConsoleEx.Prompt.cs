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
using System.Net;

namespace ConsoleFx.ConsoleExtensions
{
    public static partial class ConsoleEx
    {
        /// <summary>
        ///     Displays a message and waits for user input.
        /// </summary>
        /// <param name="message">A string or color string representing the message to be displayed</param>
        /// <returns>The input entered by the user</returns>
        public static string Prompt(ColorString message)
        {
            Print(message);
            return Console.ReadLine();
        }

        /// <summary>
        /// Accepts user input and validates the input with the specified validator.
        /// If the input is not valid, the entered text is cleared and user prompted to enter the input again.
        /// </summary>
        /// <param name="validator">Function to validate the input text</param>
        /// <returns>The input entered by the user</returns>
        public static string Prompt(Func<string, bool> validator)
        {
            (int left, int top) = (Console.CursorLeft, Console.CursorTop);

            string input = Console.ReadLine();
            bool isValid = validator(input);
            while (!isValid)
            {
                Console.SetCursorPosition(left, top);
                Console.Write(new string(' ', input.Length));
                Console.SetCursorPosition(left, top);
                input = Console.ReadLine();
                isValid = validator(input);
            }

            return input;
        }

        public static string Prompt(ColorString message, Func<string, bool> validator)
        {
            Print(message);
            return Prompt(validator);
        }

        public static int PromptList(params string[] options) =>
            PromptList(options, PromptListSettings.Default);

        public static int PromptList(IEnumerable<string> optionsList, PromptListSettings settings = null)
        {
            if (optionsList == null)
                throw new ArgumentNullException(nameof(optionsList));

            List<string> options = optionsList.ToList();
            if (options.Count < 2)
                throw new ArgumentException("Options list must have more than one item.", nameof(optionsList));

            settings = settings ?? PromptListSettings.Default;

            ClearCurrentLine();

            bool cursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;

            try
            {
                int startLine = Console.CursorTop;

                int selectedChoice = settings.SelectedIndex >= 0 && settings.SelectedIndex < options.Count
                    ? settings.SelectedIndex : 0;
                string unselectedPrefix = settings.UnselectedPrefix ?? new string(' ', settings.SelectedPrefix.Length);

                // Print the initial list with the selected value highlighted
                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selectedChoice)
                        PrintLine(new ColorString().Text($"{settings.SelectedPrefix}{options[i]}",
                            settings.SelectedForegroundColor, settings.SelectedBackgroundColor));
                    else
                        PrintLine(new ColorString().Text($"{unselectedPrefix}{options[i]}",
                            settings.UnselectedForegroundColor, settings.UnselectedBackgroundColor));
                }

                // Repeatedly handle up and down arrow key presses until Enter is pressed
                ConsoleKey pressed = WaitForKeys(ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter);
                while (pressed != ConsoleKey.Enter)
                {
                    int oldChoice = selectedChoice;

                    if (pressed == ConsoleKey.UpArrow)
                        selectedChoice--;
                    else
                        selectedChoice++;
                    if (selectedChoice < 0)
                        selectedChoice = options.Count - 1;
                    else if (selectedChoice >= options.Count)
                        selectedChoice = 0;

                    Console.SetCursorPosition(0, startLine + oldChoice);
                    Print(new ColorString().Text($"{unselectedPrefix}{options[oldChoice]}",
                        settings.UnselectedForegroundColor, settings.UnselectedBackgroundColor));

                    Console.SetCursorPosition(0, startLine + selectedChoice);
                    Print(new ColorString().Text($"{settings.SelectedPrefix}{options[selectedChoice]}",
                        settings.SelectedForegroundColor, settings.SelectedBackgroundColor));

                    pressed = WaitForKeys(ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter);
                }

                Console.SetCursorPosition(0, startLine + options.Count);

                return selectedChoice;
            }
            finally
            {
                Console.CursorVisible = cursorVisible;
            }
        }
    }

    public sealed class PromptListSettings
    {
        public int SelectedIndex { get; set; } = 0;

        public string SelectedPrefix { get; set; } = "> ";

        public string UnselectedPrefix { get; set; } = null;

        public CColor? SelectedForegroundColor { get; set; } = CColor.Cyan;

        public CColor? SelectedBackgroundColor { get; set; } = null;

        public CColor? UnselectedForegroundColor { get; set; } = null;

        public CColor? UnselectedBackgroundColor { get; set; } = null;

        public static PromptListSettings Default = new PromptListSettings();
    }
}
