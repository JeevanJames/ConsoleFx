using System.IO;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;

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

    public sealed class Program : ConsoleProgram
    {
        [Option("output", "o", Optional = true)]
        public DirectoryInfo OutputDir { get; set; } = new DirectoryInfo(@"D:\Temp");

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
