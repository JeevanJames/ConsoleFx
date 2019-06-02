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

namespace ConsoleFx.CmdLine.Program.ErrorHandlers
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
