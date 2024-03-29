﻿#region --- License & Copyright Notice ---
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

using ConsoleFx.CmdLine;

using Shouldly;

using Xunit;

namespace CmdLine.Parser.Tests
{
    public sealed class ArgumentsTests
    {
        [Fact]
        public void Add_Works_for_multiple_arguments()
        {
            var arguments = new Arguments
            {
                new Argument(),
                new Argument(),
                new Argument(),
            };

            arguments.ShouldNotBeNull();
            arguments.ShouldNotBeEmpty();
            arguments.Count.ShouldBe(3);
        }

        [Fact]
        public void Add_Throws_on_optional_arguments_before_mandatory()
        {
            var arguments = new Arguments
            {
                new Argument(isOptional: true),
            };

            Should.Throw<ParserException>(() => arguments.Add(new Argument()));
        }
    }
}
