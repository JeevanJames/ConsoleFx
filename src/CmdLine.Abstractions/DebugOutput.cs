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
    /// <summary>
    ///     Enables or disables debugging output from the ConsoleFx framework. Any code in the framework
    ///     can use the <see cref="Write(object, IEnumerable{object}, string, string, int)"/> method to
    ///     write debugging information that can be useful to troubleshoot issues.
    ///     <para/>
    ///     Debugging output is disabled by default. To enable it, call the <see cref="Enable"/> method.
    /// </summary>
    public static class DebugOutput
    {
        private const string Prefix = "[CONSOLEFX] ";

        private static bool _enabled;

        /// <summary>
        ///     Enables debugging output from the framework.
        /// </summary>
        public static void Enable()
        {
            _enabled = true;
        }

        /// <summary>
        ///     Disables debugging output from the framework.
        /// </summary>
        public static void Disable()
        {
            _enabled = false;
        }

        /// <summary>
        ///     Writes debugging output to the console. This method should only be called from ConsoleFx
        ///     code.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="list">Optional list of sub-messages.</param>
        /// <param name="memberName">The calling member name.</param>
        /// <param name="sourceFilePath">The calling member source file path.</param>
        /// <param name="sourceLineNumber">The calling member source line number.</param>
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
