using System;
using System.Threading.Tasks;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.ErrorHandling;
using ConsoleFx.CmdLine.Help;

namespace ConsoleFx.TestHarness.DotNetCliSample
{
    internal sealed class Program : ConsoleProgram
    {
        private static async Task<int> Main()
        {
            Program program = new();
            program.HandleErrorsWith<DefaultErrorHandler>();
            program.WithHelpBuilder(() => new DefaultColorHelpBuilder("help", "h"));
            program.DisplayHelpOnError();
            program.ScanEntryAssemblyForCommands();
#if DEBUG
            return await program.RunDebugAsync(condition: () => true);
#else
            return await program.RunWithCommandLineArgsAsync();
#endif
        }
    }
}
