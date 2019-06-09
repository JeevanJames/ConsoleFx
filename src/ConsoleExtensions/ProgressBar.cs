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
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ConsoleFx.ConsoleExtensions
{
    /// <summary>
    ///     Represents a console progress bar.
    /// </summary>
    public sealed class ProgressBar
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ProgressBarSpec _spec;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly int _line;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly int _column;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, string> _placeholders;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressBar" /> class.
        /// </summary>
        /// <param name="spec">The specifications of the progress bar.</param>
        /// <param name="value">
        ///     The starting value of the progress bar. Defaults to the minimum value.
        /// </param>
        public ProgressBar(ProgressBarSpec spec = null, int value = 0)
        {
            _spec = spec ?? new ProgressBarSpec();

            // Validate spec properties.
            if (_spec.MinValue >= _spec.MaxValue)
                throw new ArgumentException("Progress bar minimum value cannot be greater or equal to the maximum value.", nameof(spec));
            if (_spec.CompleteChar == _spec.IncompleteChar)
                throw new ArgumentException("Progress bar complete char cannot be the same as the incomplete char.", nameof(spec));

            // Validate value param.
            if (value < _spec.MinValue)
                _value = _spec.MinValue;
            else if (value > _spec.MaxValue)
                _value = _spec.MaxValue;
            else
                _value = value;

            // Assign remaining properties.
            _line = _spec.Line.GetValueOrDefault(Console.CursorTop);
            _column = _spec.Column.GetValueOrDefault(Console.CursorLeft);

            _placeholders = new Dictionary<string, string>(5, StringComparer.OrdinalIgnoreCase)
            {
                ["bar"] = string.Empty,
                ["percentage"] = string.Empty,
                ["value"] = value.ToString(),
                ["max"] = _spec.MaxValue.ToString(),
                ["min"] = _spec.MinValue.ToString(),
            };

            Update(_value);
        }

        public int Value
        {
            get => _value;
            set => _value = Update(value);
        }

        public int Line => _line;

        public int Column => _column;

        private int Update(int value)
        {
            // Correct value, if it isn't in range.
            if (value < _spec.MinValue)
                value = _spec.MinValue;
            else if (value > _spec.MaxValue)
                value = _spec.MaxValue;

            // Calculate placeholder values and update dictionary.
            long complete = (value * _spec.Width) / (_spec.MaxValue - _spec.MinValue);
            long incomplete = _spec.Width - complete;
            long percentage = (value * 100) / (_spec.MaxValue - _spec.MinValue);

            _placeholders["bar"] = $"{new string(_spec.CompleteChar, (int)complete)}{new string(_spec.IncompleteChar, (int)incomplete)}";
            _placeholders["percentage"] = percentage.ToString().PadLeft(3);
            _placeholders["value"] = value.ToString();

            // Build the final string to be displayed.
            string progressBar = FormatPlaceholderPattern.Replace(_spec.Format,
                match => match.Groups[1].Value + _placeholders[match.Groups[2].Value] + match.Groups[3].Value);

            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;
            bool cursorVisible = Console.CursorVisible;

            try
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(Column, Line);
                Console.Write(progressBar);
            }
            finally
            {
                Console.SetCursorPosition(currentLeft, currentTop);
                Console.CursorVisible = cursorVisible;
            }

            return value;
        }

        private static readonly Regex FormatPlaceholderPattern = new Regex(@"([^{]?){(bar|percentage|min|max|value)}([^}]?)",
            RegexOptions.IgnoreCase);

        public static ProgressBarSpec Legacy(Action<ProgressBarSpec> configure = null)
        {
            var spec = new ProgressBarSpec
            {
                CompleteChar = '=',
                IncompleteChar = ' ',
            };
            configure?.Invoke(spec);
            return spec;
        }
    }

    public sealed class ProgressBarSpec
    {
        private string _format = "{0}";

        public ProgressBarSpec()
        {
            Format = "{bar}";
        }

        public int MinValue { get; set; }

        public int MaxValue { get; set; } = 100;

        public int Width { get; set; } = 40;

        public int? Line { get; set; }

        public int? Column { get; set; }

        public char IncompleteChar { get; set; } = ' ';

        public char CompleteChar { get; set; } = '=';

        public string Format
        {
            get => _format;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Invalid progress bar format.", nameof(value));
                if (!BarPlaceholderPattern.IsMatch(value))
                    throw new ArgumentException("Format should at least contain the {bar} placeholder.", nameof(value));
                _format = value;
            }
        }

        private static readonly Regex BarPlaceholderPattern = new Regex(@"[^{]?\{bar\}[^}]?", RegexOptions.IgnoreCase);
    }
}
