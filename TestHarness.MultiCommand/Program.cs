using System;
using System.Collections.Generic;
using System.Diagnostics;

using ConsoleFx.Parser.Styles;
using ConsoleFx.Programs;

namespace TestHarness.MultiCommand
{
    internal static class Program
    {
        private static int Main()
        {
            var app = new MultiCommandProgram(new UnixParserStyle());
            try
            {
                app.RegisterCommand<UpdateCommand>();
                app.RegisterCommand<HelpCommand>();
                return app.Run();
            }
            catch (Exception ex)
            {
                return app.HandleError(ex);
            }
            finally
            {
                if (Debugger.IsAttached)
                    Console.ReadLine();
            }
        }
    }

    public sealed class UpdateCommand : Command
    {
        public UpdateCommand() : base(new [] { "update", "u" })
        {
        }

        protected override int InternalRun()
        {
            Console.WriteLine("Update");
            return 0;
        }
    }
}