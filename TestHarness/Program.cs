using ConsoleFx.Parser.Styles;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;

using static System.Console;
using static ConsoleFx.Utilities.ConsoleEx;

namespace TestHarness
{
    internal static class Program
    {
        public static string First;

        private static int Main()
        {
            var program = new ConsoleProgram<UnixParserStyle>(ListHandler);
            program.AddOption("first", "f")
                .Required()
                .ParametersRequired()
                .ValidateWith(new StringValidator(5, 10)
                {
                    MinLengthMessage = "First value must be at least 5 characters long"
                })
                .AssignTo(() => First);
            return program.Run();
        }


        private static int ListHandler()
        {
            WriteLine("In List mode");
            WriteLine($"First = {First}");
            return 0;
        }
    }
}
