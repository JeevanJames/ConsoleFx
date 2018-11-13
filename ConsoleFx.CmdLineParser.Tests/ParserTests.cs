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

using System;
using System.Collections.Generic;

using ConsoleFx.CmdLineParser.Validators;
using ConsoleFx.CmdLineParser.WindowsStyle;

using Xunit;

namespace ConsoleFx.CmdLineParser.Tests
{
    public sealed class ParserTests
    {
        private readonly Parser _parser;

        public ParserTests()
        {
            _parser = new Parser(new WindowsParserStyle());
            _parser.Options.Add(new Option("verbose", "v").UsedAsFlag(true));

            var installCommand = new Command("install");
            installCommand.Options.Add(new Option("version", "ver")
                .UsedAsSingleParameter()
                .ValidateWithRegex(@"^\d+\.\d+\.\d+(?:\.\d+)?$", message: "Invalid version specified: {0}"));
            installCommand.Arguments.Add(new Argument()
                .ValidateWithRegex(@"^\w+$"));
            _parser.Commands.Add(installCommand);
        }

        [Theory, MemberData(nameof(Parse_Parses_windows_style_args_Data))]
        public void Parse_Parses_windows_style_args(string[] tokens)
        {
            ParseResult result = _parser.Parse(tokens);
            Assert.NotNull(result);
        }

        public static IEnumerable<object[]> Parse_Parses_windows_style_args_Data()
        {
            string[] tokens1 = { "install", "packageName", "/verbose", "/ver:1.2" };
            yield return new object[] { tokens1 };
        }
    }
}