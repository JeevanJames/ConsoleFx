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
using System.Collections.Generic;
using System.Linq;

namespace ConsoleFx.ConsoleExtensions
{
    public static partial class ConsoleEx
    {
        public static int SelectSingle(IEnumerable<string> optionsList, SelectSingleSettings settings = null,
            int startingIndex = 0)
        {
            if (optionsList == null)
                throw new ArgumentNullException(nameof(optionsList));

            List<string> options = optionsList.ToList();
            if (options.Count < 2)
                throw new ArgumentException("Options list must have more than one item.", nameof(optionsList));

            settings = settings ?? SelectSingleSettings.Default;

            if (startingIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startingIndex));

            ClearCurrentLine();

            bool cursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;

            try
            {
                int startLine = Console.CursorTop;

                int selectedChoice = startingIndex >= 0 && startingIndex < options.Count
                    ? startingIndex
                    : 0;
                string unselectedPrefix =
                    settings.UnselectedPrefix ?? new string(c: ' ', settings.SelectedPrefix.Length);

                // Print the initial list with the selected value highlighted
                for (var i = 0; i < options.Count; i++)
                {
                    if (i == selectedChoice)
                    {
                        PrintLine(new ColorString().Text($"{settings.SelectedPrefix}{options[i]}",
                            settings.SelectedFgColor, settings.SelectedBgColor));
                    }
                    else
                    {
                        PrintLine(new ColorString().Text($"{unselectedPrefix}{options[i]}",
                            settings.UnselectedFgColor, settings.UnselectedBgColor));
                    }
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

                    Console.SetCursorPosition(left: 0, startLine + oldChoice);
                    Print(new ColorString().Text($"{unselectedPrefix}{options[oldChoice]}",
                        settings.UnselectedFgColor, settings.UnselectedBgColor));

                    Console.SetCursorPosition(left: 0, startLine + selectedChoice);
                    Print(new ColorString().Text($"{settings.SelectedPrefix}{options[selectedChoice]}",
                        settings.SelectedFgColor, settings.SelectedBgColor));

                    pressed = WaitForKeys(ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter);
                }

                Console.SetCursorPosition(left: 0, startLine + options.Count);

                return selectedChoice;
            }
            finally
            {
                Console.CursorVisible = cursorVisible;
            }
        }

        public static IReadOnlyList<int> SelectMultiple<T>(IEnumerable<T> optionsList,
            SelectMultipleSettings settings = null,
            int startingIndex = 0,
            IEnumerable<int> checkedIndices = null,
            Func<T, string> displaySelector = null) /*,
            Func<T, string> keySelector = null*/
        {
            if (optionsList == null)
                throw new ArgumentNullException(nameof(optionsList));

            if (displaySelector == null)
                displaySelector = item => item.ToString();

            List<string> options = optionsList.Select(displaySelector).ToList();
            if (options.Count < 2)
                throw new ArgumentException("Options list must have more than one item.", nameof(optionsList));

            settings = settings ?? SelectMultipleSettings.Default;

            if (startingIndex < 0 || startingIndex >= options.Count)
                throw new ArgumentOutOfRangeException(nameof(settings), "Selected index in settings is out of range.");

            if (checkedIndices == null)
                checkedIndices = Enumerable.Empty<int>();
            if (checkedIndices.Any(c => c < 0 || c >= options.Count))
            {
                throw new ArgumentOutOfRangeException(nameof(settings),
                    "Checked indices in settings contain out of range values.");
            }

            bool cursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;

            try
            {
                ClearCurrentLine();

                int startLine = Console.CursorTop;

                int selectedChoice = startingIndex;

                List<bool> checkedItems = Enumerable.Repeat(element: false, options.Count).ToList();
                foreach (int checkedIndex in checkedIndices)
                    checkedItems[checkedIndex] = true;

                // Print the initial list with the selected value highlighted
                for (var i = 0; i < options.Count; i++)
                {
                    string format = checkedItems[i] ? settings.CheckedFormat : settings.UncheckedFormat;
                    if (i == selectedChoice)
                    {
                        PrintLine(new ColorString().Text(string.Format(format, options[i]),
                            settings.SelectedFgColor, settings.SelectedBgColor));
                    }
                    else
                    {
                        PrintLine(new ColorString().Text(string.Format(format, options[i]),
                            settings.UnselectedFgColor, settings.UnselectedBgColor));
                    }
                }

                // Repeatedly handle up and down arrow key presses until Enter is pressed
                ConsoleKey pressed = WaitForKeys(ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter,
                    ConsoleKey.Spacebar);
                while (pressed != ConsoleKey.Enter)
                {
                    if (pressed == ConsoleKey.Spacebar)
                    {
                        checkedItems[selectedChoice] = !checkedItems[selectedChoice];
                        string format = checkedItems[selectedChoice]
                            ? settings.CheckedFormat
                            : settings.UncheckedFormat;
                        Console.SetCursorPosition(left: 0, startLine + selectedChoice);
                        Print(new ColorString().Text(string.Format(format, options[selectedChoice]),
                            settings.SelectedFgColor, settings.SelectedBgColor));
                    }
                    else
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

                        Console.SetCursorPosition(left: 0, startLine + oldChoice);
                        string format = checkedItems[oldChoice] ? settings.CheckedFormat : settings.UncheckedFormat;
                        Print(new ColorString().Text(string.Format(format, options[oldChoice]),
                            settings.UnselectedFgColor, settings.UnselectedBgColor));

                        Console.SetCursorPosition(left: 0, startLine + selectedChoice);
                        format = checkedItems[selectedChoice] ? settings.CheckedFormat : settings.UncheckedFormat;
                        Print(new ColorString().Text(string.Format(format, options[selectedChoice]),
                            settings.SelectedFgColor, settings.SelectedBgColor));
                    }

                    pressed = WaitForKeys(ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter,
                        ConsoleKey.Spacebar);
                }

                Console.SetCursorPosition(left: 0, startLine + options.Count);

                var result = new List<int>();
                for (var i = 0; i < checkedItems.Count; i++)
                {
                    if (checkedItems[i])
                        result.Add(i);
                }

                return result;
            }
            finally
            {
                Console.CursorVisible = cursorVisible;
            }
        }
    }

    public sealed class SelectSingleSettings
    {
        public string SelectedPrefix { get; set; } = "> ";

        public CColor? SelectedFgColor { get; set; } = CColor.Cyan;

        public CColor? SelectedBgColor { get; set; } = null;

        public string UnselectedPrefix { get; set; } = null;

        public CColor? UnselectedFgColor { get; set; } = null;

        public CColor? UnselectedBgColor { get; set; } = null;

        public static readonly SelectSingleSettings Default = new SelectSingleSettings();
    }

    public sealed class SelectMultipleSettings
    {
        public string CheckedFormat { get; set; } = "[X] {0}";

        public string UncheckedFormat { get; set; } = "[ ] {0}";

        public CColor? SelectedFgColor { get; set; } = CColor.Cyan;

        public CColor? SelectedBgColor { get; set; } = null;

        public CColor? UnselectedFgColor { get; set; } = null;

        public CColor? UnselectedBgColor { get; set; } = null;

        public static readonly SelectMultipleSettings Default = new SelectMultipleSettings();
    }
}
