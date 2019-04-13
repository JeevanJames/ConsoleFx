﻿#region --- License & Copyright Notice ---
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

using Shouldly;

using Xunit;

namespace ConsoleFx.CmdLineParser.Tests
{
    public sealed class OptionsTests
    {
        [Fact]
        public void Add_Works_for_multiple_options()
        {
            var options = new Options
            {
                new Option("install", "i"),
                new Option("uninstall", "un"),
                new Option("update"),
            };

            options.ShouldNotBeNull();
            options.ShouldNotBeEmpty();
            options.Count.ShouldBe(3);
        }

        [Theory]
        [InlineData("install", null, "install", null)]
        [InlineData("install", null, "install", "i")]
        [InlineData("install", "ins", "install", "i")]
        [InlineData("install", "ins", "insert", "ins")]
        public void Add_Throws_on_duplicate_option(string name1, string shortName1, string name2, string shortName2)
        {
            var options = new Options
            {
                new Option(name1, shortName1)
            };

            Should.Throw<ArgumentException>(() => options.Add(new Option(name2, shortName2)));
        }
    }
}