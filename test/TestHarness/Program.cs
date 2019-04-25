using System.Collections.Generic;

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineArgs.Validators;
using ConsoleFx.CmdLineParser.Style;
using ConsoleFx.ConsoleExtensions;
using ConsoleFx.Program;

using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness
{
    internal static class Program
    {
        private static int Main()
        {
            var program = new ConsoleProgram(ParserStyle.Windows);
            program.Options.Add(new Option("force", "f")
                .UsedAsFlag());
            program.Arguments.Add(new Argument("SourceFile")
                .ValidateAsFile(shouldExist: false));
            program.Arguments.Add(new Argument("DestFile"));
            program.Handler = Handler;
            return program.Run();
        }

        private static int Handler(IReadOnlyList<string> arguments, IReadOnlyDictionary<string, object> options)
        {
            PrintLine(new ColorString("Copying file ")
                .Cyan(arguments[index: 0])
                .Reset(" to ")
                .Green(arguments[index: 1]));
            if ((bool)options["force"])
                PrintLine(new ColorString("Force: ").Red("true"));
            WaitForAnyKey();
            return 0;
        }
    }
}
