using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;
using ConsoleFx.Utilities;

using MyNuGet.Install;
using MyNuGet.Update;

using static System.Console;

namespace MyNuGet
{
    public sealed class Program : ConsoleProgram
    {
        private static int Main()
        {
            WriteLine($"{ForegroundColor} - {BackgroundColor}");
            Palette.AddForeColor("title", ConsoleColor.Gray);
            Palette.Add("tag-line", ConsoleColor.DarkGreen, ConsoleColor.Yellow);
            new Colorizer()
                .Out("MyNuGet NuGet Simulator", "title").Line()
                .Out("The best damn nuget out there", "tag-line").Line();
            ConsoleEx.Write("MyNuGet NuGet Simulator", ConsoleColor.White, ConsoleColor.Magenta);
            ConsoleEx.WriteLine("Written by Jeevan James", ConsoleColor.Blue, ConsoleColor.Yellow);
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
