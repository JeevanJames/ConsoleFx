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

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineArgs.Base;
using ConsoleFx.CmdLineArgs.Validators;
using ConsoleFx.Program;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness
{
    internal sealed class MultiCommandProgramTest : TestBase
    {
        internal override void Run()
        {
            var program = new MyMultiCommandProgram();
            string argsStr = Prompt($"{DkBlue.BgWhite}Enter args: ");

            while (!string.IsNullOrWhiteSpace(argsStr))
            {
                string[] args = argsStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                program.Run(args);

                PrintBlank();
                argsStr = Prompt($"{DkBlue.BgWhite}Enter args: ");
            }
        }
    }

    [Program(Style = ArgStyle.Unix)]
    public sealed class MyMultiCommandProgram : ConsoleProgram
    {
        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new AddCommand();
            yield return new ListCommand();
        }
    }

    [Command("add", "install")]
    public sealed class AddCommand : ProgramCommand
    {
        [Argument("package")]
        public string PackageName { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Argument("package")
                .ValidateAsString(10, minLengthMessage: "Package must be at least 10 character long.");
        }

        protected override int HandleCommand()
        {
            PrintLine($"{Yellow}Add {Reset}new package {PackageName}.");
            return 0;
        }
    }

    [Command("list", "ls")]
    public sealed class ListCommand : ProgramCommand
    {
        public bool Global { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Option("global", "g")
                .UsedAsFlag();
        }

        protected override int HandleCommand()
        {
            PrintLine($"{Yellow}Lists {Reset}all installed packages.");
            return 0;
        }
    }
}
