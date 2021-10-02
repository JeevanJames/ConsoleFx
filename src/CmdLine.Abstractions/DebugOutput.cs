// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Enables or disables debugging output from the ConsoleFx framework. Any code in the framework
    ///     can use the <see cref="Write(object, IEnumerable{object}, string, string, int)" /> method to
    ///     write debugging information that can be useful to troubleshoot issues.
    ///     <para />
    ///     Debugging output is disabled by default. To enable it, call the <see cref="Enable" /> method.
    /// </summary>
    public static class DebugOutput
    {
        private const string Prefix = "[CONSOLEFX] ";

        private static DebugOutputWriter _writer;
        private static bool _enabled;

        /// <summary>
        ///     Enables debugging output from the framework.
        /// </summary>
        /// <param name="writer">
        ///     The delegate that will perform the actual writing. If <c>null</c>, then a default delegate
        ///     is used that just writes to the console.
        /// </param>
        public static void Enable(DebugOutputWriter writer = null)
        {
            _writer = writer ?? DefaultOutputWriter;
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
        public static void Write(object message, IEnumerable list = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (_enabled)
                _writer(message, list, memberName, sourceFilePath, sourceLineNumber);
        }

        private static void DefaultOutputWriter(object message, IEnumerable list, string memberName,
            string sourceFilePath, int sourceLineNumber)
        {
            (ConsoleColor foregroundColor, ConsoleColor backgroundColor) =
                (Console.ForegroundColor, Console.BackgroundColor);
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

                if (list is not null)
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

    public delegate void DebugOutputWriter(object message, IEnumerable list, string memberName, string sourceFilePath,
        int sourceLineNumber);
}
