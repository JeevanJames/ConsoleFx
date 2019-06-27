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
    public sealed class StatusLine : IDisposable
    {
        private ColorString _status;

        public StatusLine()
        {
            Line = Console.CursorTop;
        }

        public StatusLine(int line)
        {
            Line = line;
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
