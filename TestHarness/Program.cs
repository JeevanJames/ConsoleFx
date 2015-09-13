using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;

using static System.Console;
using static ConsoleFx.Utilities.ConsoleEx;

namespace TestHarness
{
    internal static class Program
    {
        private static string _first;

        private static int Main()
        {
            var program = new MultiCommandProgram<WindowsParserStyle>(SelectProgram);
            return program.Run();
        }


        private static Command<WindowsParserStyle> SelectProgram(string[] commands)
        {
            switch (commands[0])
            {
                case "list": return GetListCommand();
                default: return null;
            }
        }

        private static Command<WindowsParserStyle> GetListCommand()
        {
            var program = new Command<WindowsParserStyle>(ListHandler);
            program.AddOption("first", "f")
                .Required()
                .ParametersRequired()
                .ValidateWith(new StringValidator(5, 10)
                {
                    MinLengthMessage = "First value must be at least 5 characters long"
                })
                .AssignTo(() => _first);
            return program;
        }

        private static int ListHandler()
        {
            WriteLine("In List mode");
            WriteLine($"First = {_first}");
            return 1;
        }

    }
}
