﻿// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading.Tasks;

using ConsoleFx.CmdLine;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness.DeclarativeConsoleProgramTest
{
    internal sealed class Test : TestBase
    {
        internal override async Task RunAsync()
        {
            string args = Prompt($"{Magenta}Enter args: ");
            var tokens = ConsoleFx.CmdLine.Parser.Parser.Tokenize(args);
            var program = new MyProgram();
            await program.RunAsync(tokens);
        }
    }

    public sealed class MyProgram : ConsoleProgram
    {
        [Option("output", "o", Optional = true)]
        public DirectoryInfo OutputDir { get; set; } = new(@"D:\Temp");

        [Argument(Optional = true)]
        public string FirstName { get; set; } = "Jeevan";

        protected override int HandleCommand()
        {
            PrintLine(FirstName);
            PrintLine(OutputDir?.FullName ?? "<No directory specified>");
            return 0;
        }
    }

    public enum BuildConfig
    {
        Debug,
        Release,
    }
}
