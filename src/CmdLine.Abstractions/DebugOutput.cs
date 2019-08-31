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
using System.Runtime.CompilerServices;

namespace ConsoleFx.CmdLine
{
    public static class DebugOutput
    {
        private const string Prefix = "[CONSOLEFX] ";

        private static bool _enabled;

        public static void Enable()
        {
            _enabled = true;
        }

        public static void Disable()
        {
            _enabled = false;
        }

        public static void Write(object message, IEnumerable<object> list = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!_enabled)
                return;

            var (foregroundColor, backgroundColor) = (Console.ForegroundColor, Console.BackgroundColor);
            try
            {
                Console.BackgroundColor = ConsoleColor.Black;

                Console.ForegroundColor = ConsoleColor.Magenta;

                Trace.WriteLine($"{Prefix}{memberName} at {sourceFilePath} (line {sourceLineNumber})");

                Console.ForegroundColor = ConsoleColor.White;

                if (message is null)
                    Trace.WriteLine("No message specified.");
                else
                    Trace.WriteLine(message);

                if (list != null)
                {
                    foreach (object item in list)
                        Trace.WriteLine($"* {item?.ToString() ?? "[null]"}");
                }
            }
            finally
            {
                Console.ForegroundColor = foregroundColor;
                Console.BackgroundColor = backgroundColor;
            }
        }
    }
}
