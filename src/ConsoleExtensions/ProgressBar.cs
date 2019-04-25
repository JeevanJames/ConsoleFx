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
using System.Diagnostics;

namespace ConsoleFx.ConsoleExtensions
{
    /// <summary>
    ///     Represents a console progress bar.
    /// </summary>
    public sealed class ProgressBar
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressBar" /> class.
        /// </summary>
        /// <param name="value">The starting value of the progress bar. Defaults to the minimum value.</param>
        /// <param name="minValue">The minimum value of the progress bar.</param>
        /// <param name="maxValue">The maximum value of the progress bar.</param>
        /// <param name="line">The line to start rendering the progress bar. Defaults to the current cursor location.</param>
        /// <param name="column">The column to start rendering the progress bar. Defaults to the current cursor location.</param>
        /// <param name="barChar">The character to render the progress bar.</param>
        /// <param name="fillChar">The character to render the progress part of the progress bar.</param>
        public ProgressBar(int value = 0, int minValue = 0, int maxValue = 20,
            int? line = null, int? column = null, char barChar = '░', char fillChar = '▓')
        {
            if (minValue < 0)
                throw new ArgumentOutOfRangeException(nameof(minValue));
            if (minValue >= maxValue)
                throw new ArgumentException("minValue should be less than maxValue");

            if (value < minValue || value > maxValue)
                value = minValue;

            MinValue = minValue;
            MaxValue = maxValue;
            Value = value;
            Line = line ?? Console.CursorTop;
            Column = column ?? Console.CursorLeft;
            BarChar = barChar;
            FillChar = fillChar;
        }

        public int MinValue { get; }

        public int MaxValue { get; }

        public int Value
        {
            get => _value;
            set => Update(value);
        }

        public int Line { get; }

        public int Column { get; }

        public char BarChar { get; }

        public char FillChar { get; }

        private void Update(int value)
        {
            if (value < MinValue || value > MaxValue)
                return;

            _value = value;

            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;
            bool cursorVisible = Console.CursorVisible;

            try
            {
                Console.SetCursorPosition(Line, Column);
                Console.Write(new string(FillChar, value));
                Console.Write(new string(BarChar, MaxValue - MinValue - value));
            }
            finally
            {
                Console.SetCursorPosition(currentLeft, currentTop);
                Console.CursorVisible = cursorVisible;
            }
        }
    }
}
