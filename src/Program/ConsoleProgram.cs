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
using System.Linq;

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineParser;
using ConsoleFx.CmdLineParser.Style;

namespace ConsoleFx.Program
{
    public class ConsoleProgram : Command
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ArgStyle _argStyle;

        public ConsoleProgram(ArgStyle argStyle, ArgGrouping grouping = ArgGrouping.DoesNotMatter)
        {
            if (argStyle is null)
                throw new ArgumentNullException(nameof(argStyle));
            _argStyle = argStyle;
            Grouping = grouping;
        }

        public ArgGrouping Grouping { get; }

        public int Run()
        {
            var parser = new Parser(_argStyle, Grouping);
            List<Argument> arguments = Arguments.ToList();
            foreach (Argument argument in arguments)
                parser.Arguments.Add(argument);
            Options options = Options;
            foreach (Option option in options)
                parser.Options.Add(option);
            List<Command> commands = Commands.ToList();
            foreach (Command command in commands)
                parser.Commands.Add(command);
            try
            {
                ParseResult result = parser.Parse(Environment.GetCommandLineArgs().Skip(1));
                return Handler(result.Arguments, result.Options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }
        }
    }
}
