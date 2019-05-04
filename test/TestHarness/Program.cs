using System.Collections.Generic;
using System.Threading;

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineArgs.Validators;
using ConsoleFx.CmdLineParser.Style;
using ConsoleFx.ConsoleExtensions;
using ConsoleFx.Program;

using static System.Console;
using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness
{
    internal static class Program
    {
        private static int Main()
        {
            PrintLine(LargestWindowWidth.ToString());
            WaitForAnyKey();
            var pb = new ProgressBar(1, maxValue: LargestWindowWidth);
            Thread.Sleep(1000);
            pb.Value = 2;
            Thread.Sleep(1000);
            pb.Value = 3;
            Thread.Sleep(1000);
            pb.Value = 4;

            var program = new ConsoleProgram(ArgStyle.Windows);
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
            PrintLine($"Copying file {Cyan}{arguments[0]} to {Green}{arguments[1]}");
            if ((bool)options["force"])
                PrintLine($"Force: {Red}true");
            WaitForAnyKey();
            return 0;
        }
    }
}
