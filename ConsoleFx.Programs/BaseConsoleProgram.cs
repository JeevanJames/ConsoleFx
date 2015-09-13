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
    public abstract class BaseConsoleProgram<TStyle> : BaseParser<TStyle>
        where TStyle : ParserStyle, new()
    {
        private readonly ExecuteHandler _handler;

        protected BaseConsoleProgram(ExecuteHandler handler)
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
            Arguments.Add(argument);
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

            Options.Add(option);

            return option;
        }

        public int Run()
        {
            try
            {
                IEnumerable<string> args = Environment.GetCommandLineArgs().Skip(1);
                Parse(args);
                return _handler();
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

    public delegate int ExecuteHandler();

}
