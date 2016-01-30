using System;
using System.Diagnostics;

using ConsoleFx.Parser.Styles;
using ConsoleFx.Programs;

using static System.Console;

namespace TestHarness.MultiCommand
{
    internal static class Program
    {
        private static int Main()
        {
            var app = new MultiCommandProgram(new UnixParserStyle());
            app.RegisterCommand<UpdateCommand>();
            app.RegisterCommand<HelpCommand>();
            int exitCode = app.Run();
            if (Debugger.IsAttached)
                ReadLine();
            return exitCode;
        }
    }

    public sealed class UpdateCommand : Command
    {
        public UpdateCommand() : base(new[] { "update", "u" })
        {
        }

        protected override int Handle()
        {
            WriteLine("Update");
            return 0;
        }
    }
}
