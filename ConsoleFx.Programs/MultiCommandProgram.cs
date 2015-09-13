#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015 Jeevan James

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

using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleFx.Programs
{
    public sealed class MultiCommandProgram<TStyle>
        where TStyle : ParserStyle, new()
    {
        private readonly CommandSelector<TStyle> _selector;

        public MultiCommandProgram(CommandSelector<TStyle> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            _selector = selector;
        }

        public int Run()
        {
            try
            {
                string[] allArgs = Environment.GetCommandLineArgs();
                IEnumerable<string> programArgs = allArgs.Skip(2);
                Command<TStyle> program = _selector(new string[] { allArgs[1] });
                return program.Run();
            }
            catch (ParserException ex)
            {
                Console.WriteLine(ex);
                return ex.ErrorCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }
        }
    }

    public sealed class Command<TStyle> : BaseConsoleProgram<TStyle>
        where TStyle : ParserStyle, new()
    {
        public Command(ExecuteHandler handler) : base(handler)
        {
        }
    }

    public delegate Command<TStyle> CommandSelector<TStyle>(string[] commands)
        where TStyle : ParserStyle, new();
}
