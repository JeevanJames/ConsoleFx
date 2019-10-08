using System;
using System.Collections.Generic;
using System.IO;
using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Validators;
using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness.DeclarativeConsoleProgramTest
{
    internal sealed class Test : TestBase
    {
        internal override void Run()
        {
            string args = Prompt($"{Magenta}Enter args: ");
            var tokens = ConsoleFx.CmdLine.Parser.Parser.Tokenize(args);
            var program = new Program();
            program.Run(tokens);
        }
    }

    [Program("decl")]
    public sealed class Program : ConsoleProgram
    {
        [Option("output")]
        public DirectoryInfo OutputDir { get; set; }

        protected override int HandleCommand()
        {
            return 0;
        }
    }

    public enum BuildConfig
    {
        Debug,
        Release,
    }
}
