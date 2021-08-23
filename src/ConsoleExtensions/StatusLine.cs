// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.ConsoleExtensions
{
    public sealed class StatusLine : IDisposable
    {
        private ColorString _status;

        public StatusLine()
        {
            Line = Console.CursorTop;
            ConsoleEx.ClearCurrentLine();
            Console.WriteLine();
        }

        public StatusLine(int line)
        {
            Line = line;
            ConsoleEx.ClearLine(line);
        }

        public void Dispose()
        {
            Console.WriteLine();
        }

        public ColorString Status
        {
            get => _status;
            set
            {
                _status = value;
                Update();
            }
        }

        public int Line { get; }

        private void Update()
        {
            var (left, top) = (Console.CursorLeft, Console.CursorTop);
            try
            {
                ConsoleEx.ClearLine(Line);
                Console.SetCursorPosition(0, Line);
                ConsoleEx.Print(_status ?? string.Empty);
            }
            finally
            {
                Console.SetCursorPosition(left, top);
            }
        }
    }
}
