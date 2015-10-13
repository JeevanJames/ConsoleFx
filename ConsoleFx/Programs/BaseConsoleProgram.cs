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
    public interface IConsoleProgram
    {
        int Run();
    }

    public abstract class BaseConsoleProgram<TStyle> : IConsoleProgram
        where TStyle : ParserStyle, new()
    {
        private readonly ExecuteHandler _handler;

        private readonly CommandGrouping _grouping;
        private readonly object _scope;

        protected BaseConsoleProgram(CommandGrouping grouping = CommandGrouping.DoesNotMatter, object scope = null)
        {
            _grouping = grouping;
            _scope = scope;
        }

        public int Run()
        {
            try
            {
                var parser = new Parser<TStyle>();
                parser.Behaviors.Grouping = _grouping;
                parser.Behaviors.Scope = _scope;
                foreach (Option option in GetOptions())
                    parser.Options.Add(option);
                foreach (Argument argument in GetArguments())
                    parser.Arguments.Add(argument);

                parser.Parse(GetCommandLineArgs());

                return Execute();
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

        protected virtual IEnumerable<string> GetCommandLineArgs()
        {
            return Environment.GetCommandLineArgs().Skip(1);
        }

        protected virtual IEnumerable<Option> GetOptions()
        {
            yield break;
        }

        protected virtual IEnumerable<Argument> GetArguments()
        {
            yield break;
        }

        protected abstract int Execute();
    }

    public delegate int ExecuteHandler();
}
