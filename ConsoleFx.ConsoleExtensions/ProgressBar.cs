using System;

namespace ConsoleFx.ConsoleExtensions
{
    public sealed class ProgressBar
    {
        private int _minValue = 0;
        private int _maxValue = 20;

        public int MinValue
        {
            get => _minValue;
            set
            {
                if (value < 0 || value >= MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _minValue = value;
            }
        }

        public int MaxValue
        {
            get => _maxValue;
            set
            {
                if (value <= MinValue)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _maxValue = value;
            }
        }

        public int? Line { get; set; } = null;

        public int? Column { get; set; } = null;

        public void Update(int value)
        {
            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;
            bool cursorVisible = Console.CursorVisible;

            try
            {
                Console.SetCursorPosition(Line ?? currentLeft, Column ?? currentTop);
                Console.Write(new string('▓', value));
                Console.Write(new string('░', MaxValue - MinValue - value));
            }
            finally
            {
                Console.SetCursorPosition(currentLeft, currentTop);
                Console.CursorVisible = cursorVisible;
            }
        }
    }
}