// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.CmdLine.ErrorHandling
{
    public sealed class DefaultErrorHandler : ErrorHandler
    {
        public ConsoleColor? ForeColor { get; set; }

        public ConsoleColor? BackColor { get; set; }

        public override int HandleError(Exception ex)
        {
            var (fg, bg) = (Console.ForegroundColor, Console.BackgroundColor);
            try
            {
                if (ForeColor.HasValue)
                    Console.ForegroundColor = ForeColor.Value;
                if (BackColor.HasValue)
                    Console.BackgroundColor = BackColor.Value;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ForegroundColor = fg;
                Console.BackgroundColor = bg;
            }

            return -1;
        }
    }
}
