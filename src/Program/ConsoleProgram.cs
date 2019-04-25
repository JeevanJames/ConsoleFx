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
using System.Linq;

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineParser;
using ConsoleFx.CmdLineParser.Style;

namespace ConsoleFx.Program
{
    public abstract class ConsoleProgram
    {
        private readonly ParserStyle _parserStyle;

        protected ConsoleProgram(ParserStyle parserStyle)
        {
            if (parserStyle == null)
                throw new ArgumentNullException(nameof(parserStyle));
            _parserStyle = parserStyle;
        }

        protected virtual ArgGrouping Grouping => ArgGrouping.DoesNotMatter;

        protected virtual IEnumerable<Command> GetCommands()
        {
            yield break;
        }

        protected virtual IEnumerable<Option> GetOptions()
        {
            yield break;
        }

        protected virtual IEnumerable<Argument> GetArguments()
        {
            yield break;
        }

        protected abstract int Handle(ParseResult result);

        public int Run()
        {
            var parser = new Parser(_parserStyle, Grouping);
            IEnumerable<Argument> arguments = GetArguments();
            foreach (Argument argument in arguments)
                parser.Arguments.Add(argument);
            IEnumerable<Option> options = GetOptions();
            foreach (Option option in options)
                parser.Options.Add(option);
            IEnumerable<Command> commands = GetCommands();
            foreach (Command command in commands)
                parser.Commands.Add(command);
            try
            {
                ParseResult result = parser.Parse(Environment.GetCommandLineArgs().Skip(1));
                return Handle(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }
        }
    }
}
