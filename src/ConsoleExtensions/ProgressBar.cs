#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

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
        private readonly ProgressBarStyle _style;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, string> _placeholders;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _value;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _status;

        private int _maxLineLength;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressBar" /> class.
        /// </summary>
        /// <param name="spec">The specifications of the progress bar.</param>
        /// <param name="value">
        ///     The starting value of the progress bar. Defaults to the minimum value.
        /// </param>
        /// <param name="style">The style of the progress bar.</param>
        internal ProgressBar(ProgressBarSpec spec = null, int value = 0, ProgressBarStyle style = null)
        {
            _spec = spec ?? new ProgressBarSpec();
            _style = style ?? new ProgressBarStyle();

            // Validate spec properties.
            if (_spec.MinValue >= _spec.MaxValue)
                throw new ArgumentException("Progress bar minimum value cannot be greater or equal to the maximum value.", nameof(spec));
            if (_style.Complete.Char == _style.Incomplete.Char)
                throw new ArgumentException("Progress bar complete char cannot be the same as the incomplete char.", nameof(spec));

            // Validate value param.
            if (value < _spec.MinValue)
                _value = _spec.MinValue;
            else if (value > _spec.MaxValue)
                _value = _spec.MaxValue;
            else
                _value = value;

            // Assign remaining properties.
            Line = _spec.Line.GetValueOrDefault(Console.CursorTop);
            Column = _spec.Column.GetValueOrDefault(Console.CursorLeft);

            _placeholders = new Dictionary<string, string>(5, StringComparer.OrdinalIgnoreCase)
            {
                ["bar"] = string.Empty,
                ["percentage"] = string.Empty,
                ["value"] = value.ToString(),
                ["max"] = _spec.MaxValue.ToString(),
                ["min"] = _spec.MinValue.ToString(),
                ["status"] = string.Empty,
            };

            // Display the progress bar with the initial value.
            Update(_value);

            // Move to the next line, so that the progress bar owns it's own line.
            Console.WriteLine();
        }

        public int Value
        {
            get => _value;
            set => _value = Update(value);
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                Update(Value);
            }
        }

        public int Min
        {
            get => _spec.MinValue;
            set
            {
                //TODO: Validate value
                _spec.MinValue = value;
                _value = Update(_value);
            }
        }

        public int Max
        {
            get => _spec.MaxValue;
            set
            {
                //TODO: Validate value
                _spec.MaxValue = value;
                _value = Update(_value);
            }
        }

        public int Line { get; }

        public int Column { get; }

        private int Update(int value)
        {
            // Correct value, if it isn't in range.
            if (value < _spec.MinValue)
                value = _spec.MinValue;
            else if (value > _spec.MaxValue)
                value = _spec.MaxValue;

            // Calculate placeholder values and update dictionary.
            long total = _spec.MaxValue - _spec.MinValue;
            long complete = 0, incomplete = 0, percentage = 0;

            // Avoid divide by zero exceptions
            if (total > 0)
            {
                complete = (value * _spec.Width) / total;
                incomplete = _spec.Width - complete;
                percentage = (value * 100) / total;
            }

            ColorString barCStr = new ColorString(new string(_style.Complete.Char, (int)complete), _style.Complete.ForeColor, _style.Complete.BackColor)
                .Reset().BgReset()
                .Text(new string(_style.Incomplete.Char, (int)incomplete), _style.Incomplete.ForeColor, _style.Incomplete.BackColor)
                .Reset().BgReset();

            _placeholders["bar"] = barCStr.ToString();
            _placeholders["percentage"] = percentage.ToString().PadLeft(3);
            _placeholders["value"] = value.ToString();
            _placeholders["max"] = _spec.MaxValue.ToString();
            _placeholders["min"] = _spec.MinValue.ToString();
            _placeholders["status"] = Status ?? string.Empty;

            // Build the final string to be displayed.
            string progressBar = FormatPlaceholderPattern.Replace(_spec.Format,
                match => match.Groups[1].Value + _placeholders[match.Groups[2].Value] + match.Groups[3].Value);
            ColorString progressBarCStr = progressBar;

            // Figure out how much padding to add after printing the progress bar, so that no
            // artifacts are left displayed.
            string justText = progressBarCStr.ToText();
            _maxLineLength = Math.Max(justText.Length, _maxLineLength);
            string padding = new string(' ', Math.Max(0, _maxLineLength - justText.Length));

            // Save current console status
            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;
            bool cursorVisible = Console.CursorVisible;

            try
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(Column, Line);
                ConsoleEx.Print(progressBar);
                Console.Write(padding);
            }
            finally
            {
                // Restore console status
                Console.SetCursorPosition(currentLeft, currentTop);
                Console.CursorVisible = cursorVisible;
            }

            return value;
        }

        private static readonly Regex FormatPlaceholderPattern = new Regex(@"([^<]?)\<\<(bar|percentage|min|max|value|status)\>\>([^>]?)",
            RegexOptions.IgnoreCase);
    }

    public sealed class ProgressBarSpec
    {
        private ColorString _format;

        public ProgressBarSpec()
        {
            Format = "<<bar>>";
        }

        public int MinValue { get; set; }

        public int MaxValue { get; set; } = 100;

        public int Width { get; set; } = 40;

        public int? Line { get; set; }

        public int? Column { get; set; }

        public ColorString Format
        {
            get => _format;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));
                string str = value.ToString();
                if (!BarPlaceholderPattern.IsMatch(str))
                    throw new ArgumentException("Format should at least contain the <<bar>> placeholder.", nameof(value));
                _format = value;
            }
        }

        private static readonly Regex BarPlaceholderPattern = new Regex(@"[^<]?\<\<bar\>\>[^>]?", RegexOptions.IgnoreCase);
    }

    public sealed class ProgressBarStyle
    {
        public Part Complete { get; } = new Part();

        public Part Incomplete { get; } = new Part { Char = ' ' };

        public ProgressBarStyle CompleteChar(char c)
        {
            Complete.Char = c;
            return this;
        }

        public ProgressBarStyle CompleteForeColor(CColor? color)
        {
            Complete.ForeColor = color;
            return this;
        }

        public ProgressBarStyle CompleteBackColor(CColor? color)
        {
            Complete.BackColor = color;
            return this;
        }

        public ProgressBarStyle IncompleteChar(char c)
        {
            Incomplete.Char = c;
            return this;
        }

        public ProgressBarStyle IncompleteForeColor(CColor? color)
        {
            Incomplete.ForeColor = color;
            return this;
        }

        public ProgressBarStyle IncompleteBackColor(CColor? color)
        {
            Incomplete.BackColor = color;
            return this;
        }

        public static ProgressBarStyle Default => new ProgressBarStyle().CompleteChar('=');

        public static ProgressBarStyle Dots => new ProgressBarStyle().CompleteChar('.');

        public static ProgressBarStyle Block => new ProgressBarStyle().CompleteChar('█');

        public static ProgressBarStyle HalfBlock => new ProgressBarStyle().CompleteChar('▄');

        public static ProgressBarStyle Lines => new ProgressBarStyle().CompleteChar('≡');

        public static ProgressBarStyle Shaded => new ProgressBarStyle().CompleteChar('█').IncompleteChar('░');

        public sealed class Part
        {
            public char Char { get; set; }

            public CColor? ForeColor { get; set; }

            public CColor? BackColor { get; set; }
        }
    }
}
