using System;

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
                return app.Run();
            } catch (Exception ex)
            {
                return app.HandleError(ex);
            }
        }
    }
}