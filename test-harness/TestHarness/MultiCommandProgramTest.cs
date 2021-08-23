// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Program.HelpBuilders;
using ConsoleFx.CmdLine.Validators;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness
{
    internal sealed class MultiCommandProgramTest : TestBase
    {
        internal override async Task RunAsync()
        {
            var program = new MyMultiCommandProgram();
            program.HelpBuilder = new DefaultColorHelpBuilder("help", "h");
            string argsStr = Prompt($"{DkBlue.BgWhite}Enter args: ");

            while (!string.IsNullOrWhiteSpace(argsStr))
            {
                string[] args = argsStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                await program.RunAsync(args);

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
    [Help("Installs a package")]
    public sealed class AddCommand : Command
    {
        [Argument]
        [Help("package-name", "The name of the package to install")]
        public string PackageName { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Argument()
                .ValidateAsString(10, minLengthMessage: "Package must be at least 10 character long.");
        }

        protected override int HandleCommand()
        {
            PrintLine($"{Yellow}Add {Reset}new package {PackageName}.");
            return 0;
        }
    }

    [Command("list", "ls")]
    public sealed class ListCommand : Command
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
