using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ConsoleFx.CmdLineParser.WindowsStyle;
using ConsoleFx.Parser;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;

using MyNuGet.Install;
using MyNuGet.Update;

using static System.Console;
using static System.ConsoleColor;

using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace MyNuGet
{
    public sealed class Program : ConsoleProgram
    {
        private static int Main()
        {
            WriteLine($"{ForegroundColor} - {BackgroundColor}");
            WriteLine("[gray]MyNuGet NuGet Simulator");
            WriteLine("[DarkGreen|Yellow]The best damn nuget out there");
            Write("MyNuGet NuGet Simulator", White, Magenta);
            WriteLine("Written by Jeevan James", Blue, Yellow);
            int exitCode = new Program(new WindowsParserStyle()).Run();
            if (Debugger.IsAttached)
                ReadLine();
            return exitCode;
        }

        public Program(ParserStyle parserStyle) : base(parserStyle)
        {
        }

        protected override int Handle(ParseResult result)
        {
            WriteLine($"Command: {result.Command?.Name ?? "[Root]"}");
            foreach (KeyValuePair<string, object> kvp in result.Options)
            {
                Write($"Option {kvp.Key}: ");
                var list = kvp.Value as IList;
                if (list != null)
                {
                    foreach (object item in list)
                        Write($"{item?.ToString() ?? "(null)"}, ");
                    WriteLine();
                }
                else
                    WriteLine(kvp.Value);
            }
            foreach (string argument in result.Arguments)
            {
                WriteLine($"Argument {argument}");
            }
            return 0;
        }

        protected override IEnumerable<Command> GetCommands()
        {
            yield return new InstallCommand();
            yield return new UpdateCommand();
        }

        protected override IEnumerable<Option> GetOptions()
        {
            yield return new Option("ConfigFile")
                .UsedAsSingleParameter()
                .ValidateWith(new FileValidator { ShouldExist = true })
                .ParamsOfType(@default: new FileInfo(@"%AppData%\NuGet\NuGet.config"))
                .Description(Root.ConfigFile);
        }
    }
}
