#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

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

using Shouldly;
using Xunit;

using ParserStatic = ConsoleFx.CmdLine.Parser.Parser;

namespace CmdLine.Parser.Tests
{
    public sealed class ParserTokenizerTests
    {
        [Theory, MemberData(nameof(Tokenizes_string_correctly_Data))]
        public void Tokenizes_string_correctly(string str, IReadOnlyList<string> expectedTokens)
        {
            IEnumerable<string> tokens = ParserStatic.Tokenize(str);

            tokens.ShouldBe(expectedTokens);
        }

        public static IEnumerable<object[]> Tokenizes_string_correctly_Data()
        {
            yield return new object[]
            {
                string.Empty,
                new string[0],
            };

            yield return new object[]
            {
                null,
                new string[0],
            };

            yield return new object[]
            {
                "ConsoleFx is the best framework for console apps",
                new[] { "ConsoleFx", "is", "the", "best", "framework", "for", "console", "apps" },
            };

            // With a delimiter
            yield return new object[]
            {
                @"command exec ""dir *.* /ad"" --verbose",
                new[] { "command", "exec", "dir *.* /ad", "--verbose" },
            };

            // Don't close the delimited part
            yield return new object[]
            {
                @"command exec ""dir *.* /ad",
                new[] { "command", "exec", "dir *.* /ad" },
            };

            yield return new object[]
            {
                @"  ""  ""  ",
                new[] { "  " },
            };
        }
    }
}
