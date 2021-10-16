// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ConsoleFx.CmdLine;

using Spectre.Console;

namespace TestHarness.DeclarativeConsoleProgramTest
{
    internal sealed class Test : TestBase
    {
        internal override async Task RunAsync()
        {
            string args = AnsiConsole.Ask<string>("[magenta]Enter args: [/]");
            IEnumerable<string> tokens = ConsoleFx.CmdLine.Parser.Parser.Tokenize(args);
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
            AnsiConsole.MarkupLine(FirstName);
            AnsiConsole.MarkupLine(OutputDir?.FullName ?? "[red]<No directory specified>[/]");
            return 0;
        }
    }

    public enum BuildConfig
    {
        Debug,
        Release,
    }
}
