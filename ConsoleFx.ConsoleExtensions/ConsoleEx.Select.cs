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
        public static int SelectSingle(params string[] options) =>
            SelectSingle(options, SelectSingleSettings.Default);

        public static int SelectSingle(IEnumerable<string> optionsList, SelectSingleSettings settings = null)
        {
            if (optionsList == null)
                throw new ArgumentNullException(nameof(optionsList));

            List<string> options = optionsList.ToList();
            if (options.Count < 2)
                throw new ArgumentException("Options list must have more than one item.", nameof(optionsList));

            settings = settings ?? SelectSingleSettings.Default;

            ClearCurrentLine();

            bool cursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;

            try
            {
                int startLine = Console.CursorTop;

                int selectedChoice = settings.StartingIndex >= 0 && settings.StartingIndex < options.Count
                    ? settings.StartingIndex : 0;
                string unselectedPrefix = settings.Unselected.Prefix ?? new string(' ', settings.Selected.Prefix.Length);

                // Print the initial list with the selected value highlighted
                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selectedChoice)
                        PrintLine(new ColorString().Text($"{settings.Selected.Prefix}{options[i]}",
                            settings.Selected.ForegroundColor, settings.Selected.BackgroundColor));
                    else
                        PrintLine(new ColorString().Text($"{unselectedPrefix}{options[i]}",
                            settings.Unselected.ForegroundColor, settings.Unselected.BackgroundColor));
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
                        settings.Unselected.ForegroundColor, settings.Unselected.BackgroundColor));

                    Console.SetCursorPosition(0, startLine + selectedChoice);
                    Print(new ColorString().Text($"{settings.Selected.Prefix}{options[selectedChoice]}",
                        settings.Selected.ForegroundColor, settings.Selected.BackgroundColor));

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

        public static IReadOnlyList<int> SelectMultiple(params string[] options) =>
            SelectMultiple(options, SelectMultipleSettings.Default);

        public static IReadOnlyList<int> SelectMultiple(IEnumerable<string> optionsList, SelectMultipleSettings settings = null)
        {
            if (optionsList == null)
                throw new ArgumentNullException(nameof(optionsList));

            List<string> options = optionsList.ToList();
            if (options.Count < 2)
                throw new ArgumentException("Options list must have more than one item.", nameof(optionsList));

            settings = settings ?? SelectMultipleSettings.Default;

            if (settings.StartingIndex < 0 || settings.StartingIndex >= options.Count)
                throw new ArgumentOutOfRangeException("Selected index in settings is out of range.", nameof(settings));

            if (settings.CheckedIndices.Any(c => c < 0 || c >= options.Count))
                throw new ArgumentOutOfRangeException("Checked indices in settings contain out of range values.", nameof(settings));

            ClearCurrentLine();

            bool cursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;

            try
            {
                int startLine = Console.CursorTop;

                int selectedChoice = settings.StartingIndex;

                List<bool> checkedItems = Enumerable.Repeat(false, options.Count).ToList();
                for (int i = 0; i < settings.CheckedIndices.Count; i++)
                    checkedItems[i] = true;

                // Print the initial list with the selected value highlighted
                for (int i = 0; i < options.Count; i++)
                {
                    string format = checkedItems[i] ? settings.CheckedFormat : settings.UncheckedFormat;
                    if (i == selectedChoice)
                        PrintLine(new ColorString().Text(string.Format(format, options[i]),
                            settings.SelectedFgColor, settings.SelectedBgColor));
                    else
                        PrintLine(new ColorString().Text(string.Format(format, options[i]),
                            settings.UnselectedFgColor, settings.UnselectedBgColor));
                }

                // Repeatedly handle up and down arrow key presses until Enter is pressed
                ConsoleKey pressed = WaitForKeys(ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter, ConsoleKey.Spacebar);
                while (pressed != ConsoleKey.Enter)
                {
                    if (pressed == ConsoleKey.Spacebar)
                    {
                        checkedItems[selectedChoice] = !checkedItems[selectedChoice];
                        string format = checkedItems[selectedChoice] ? settings.CheckedFormat : settings.UncheckedFormat;
                        Console.SetCursorPosition(0, startLine + selectedChoice);
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

                        Console.SetCursorPosition(0, startLine + oldChoice);
                        string format = checkedItems[oldChoice] ? settings.CheckedFormat : settings.UncheckedFormat;
                        Print(new ColorString().Text(string.Format(format, options[oldChoice]),
                            settings.UnselectedFgColor, settings.UnselectedBgColor));

                        Console.SetCursorPosition(0, startLine + selectedChoice);
                        format = checkedItems[selectedChoice] ? settings.CheckedFormat : settings.UncheckedFormat;
                        Print(new ColorString().Text(string.Format(format, options[selectedChoice]),
                            settings.SelectedFgColor, settings.SelectedBgColor));
                    }

                    pressed = WaitForKeys(ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter, ConsoleKey.Spacebar);
                }

                Console.SetCursorPosition(0, startLine + options.Count);

                var checkedIndices = new List<int>();
                for (int i = 0; i < checkedItems.Count; i++)
                {
                    if (checkedItems[i])
                        checkedIndices.Add(i);
                }
                return checkedIndices;
            }
            finally
            {
                Console.CursorVisible = cursorVisible;
            }
        }
    }

    public sealed class SelectSingleSettings
    {
        public int StartingIndex { get; set; } = 0;

        public ListItemSettings Selected { get; } = new ListItemSettings();

        public ListItemSettings Unselected { get; } = new ListItemSettings();

        public static SelectSingleSettings Default = new SelectSingleSettings();
    }

    public sealed class ListItemSettings
    {
        public string Prefix { get; set; }

        public CColor? ForegroundColor { get; set; }

        public CColor? BackgroundColor { get; set; }
    }

    public sealed class SelectMultipleSettings
    {
        public int StartingIndex { get; set; } = 0;

        public IList<int> CheckedIndices { get; } = new List<int>();

        public string CheckedFormat { get; set; } = "[X] {0}";

        public string UncheckedFormat { get; set; } = "[ ] {0}";

        public CColor? SelectedFgColor { get; set; } = CColor.Cyan;

        public CColor? SelectedBgColor { get; set; } = null;

        public CColor? UnselectedFgColor { get; set; } = null;

        public CColor? UnselectedBgColor { get; set; } = null;

        public static SelectMultipleSettings Default = new SelectMultipleSettings();
    }
}