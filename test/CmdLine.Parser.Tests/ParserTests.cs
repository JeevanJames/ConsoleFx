#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2018 Jeevan James

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

using System.Collections.Generic;
using CmdLine.Parser.Tests.Commands;
using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Parser;
using ConsoleFx.CmdLine.Parser.Style;

using Xunit;

using ParserTool = ConsoleFx.CmdLine.Parser.Parser;

namespace CmdLine.Parser.Tests
{
    public sealed class ParserTests
    {
        private readonly ParserTool _parser;

        public ParserTests()
        {
            var command = new Command("root");
            command.Options.Add(new Option("verbose", "v").UsedAsFlag(true));
            command.Commands.Add(new InstallCommand());

            _parser = new ParserTool(command, new WindowsArgStyle());
        }

        [Theory, MemberData(nameof(Parse_Parses_windows_style_args_Data))]
        public void Parse_Parses_windows_style_args(string[] tokens)
        {
            ParseResult result = _parser.Parse(tokens);
            Assert.NotNull(result);
        }

        public static IEnumerable<object[]> Parse_Parses_windows_style_args_Data()
        {
            string[] tokens1 = { "install", "package-name", "/save-dev" };
            yield return new object[] { tokens1 };
        }
    }
}
