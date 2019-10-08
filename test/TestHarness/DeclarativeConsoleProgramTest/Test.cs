using System;

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
        [Argument]
        public string Project { get; set; }

        [Flag("verbose", "v")]
        public bool Verbose { get; set; }

        [Option("configuration", "c", "cfg")]
        [EnumValidator(typeof(BuildConfig))]
        public BuildConfig Configuration { get; set; }

        protected override int HandleCommand()
        {
            PrintLine($"Project      : {Magenta}{Project}");
            PrintLine($"Verbose      : {Magenta}{Verbose}");
            PrintLine($"Configuration: {Magenta}{Configuration}");
            return 0;
        }
    }

    public enum BuildConfig
    {
        Debug,
        Release,
    }
}
