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

namespace ConsoleFx.Programs
{
    public sealed class ConsoleProgram<TStyle> : BaseConsoleProgram<TStyle>
        where TStyle : ParserStyle, new()
    {
        private readonly List<Option> _options = new List<Option>();
        private readonly List<Argument> _arguments = new List<Argument>();
        private readonly ExecuteHandler _handler;

        public ConsoleProgram(ExecuteHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            _handler = handler;
        }

        public Argument AddArgument(bool optional = false)
        {
            var argument = new Argument
            {
                IsOptional = optional
            };
            _arguments.Add(argument);
            return argument;
        }

        public Option AddOption(string name, string shortName = null, bool caseSensitive = false, int order = int.MaxValue)
        {
            var option = new Option(name)
            {
                CaseSensitive = caseSensitive,
                Order = order,
            };
            if (!string.IsNullOrWhiteSpace(shortName))
                option.ShortName = shortName;

            _options.Add(option);
            return option;
        }

        protected override IEnumerable<Option> GetOptions()
        {
            return _options;
        }

        protected override IEnumerable<Argument> GetArguments()
        {
            return _arguments;
        }

        protected override int Execute()
        {
            return _handler();
        }
    }
}
