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

using System;
using System.Linq;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;

namespace ConsoleFx.Programs
{
    public sealed class SimpleProgram : ConsoleProgram
    {
        private readonly Parser.Parser _parser;
        private readonly ExecuteHandler _handler;

        public SimpleProgram(ExecuteHandler handler, ParserStyle parserStyle,
            CommandGrouping grouping = CommandGrouping.DoesNotMatter, object scope = null)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            _parser = new Parser.Parser(parserStyle, grouping, scope);
            _handler = handler;
        }

        public Argument AddArgument(bool optional = false) => _parser.AddArgument(optional);

        public Option AddOption(string name, string shortName = null, bool caseSensitive = false,
            int order = int.MaxValue) => _parser.AddOption(name, shortName, caseSensitive, order);

        protected override int InternalRun()
        {
            string[] args = Environment.GetCommandLineArgs();
            _parser.Parse(args.Skip(1));
            return _handler(_parser.Scope);
        }

    }

    public delegate int ExecuteHandler(object scope);
}