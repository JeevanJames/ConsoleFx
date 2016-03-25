using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;
using ConsoleFx.Parser.Validators;
using ConsoleFx.Programs;

using static System.Console;

namespace TestHarness
{
    public enum BackupType
    {
        Full,
        Incremental
    }

    public static class Program
    {
        private static int Main()
        {
            int exitCode = new NpmProgram(new WindowsParserStyle()).Run();
            if (Debugger.IsAttached)
                ReadLine();
            return exitCode;
        }
    }

    public sealed class NpmProgram : ConsoleProgram
    {
        public NpmProgram(ParserStyle parserStyle) : base(parserStyle)
        {
        }

        protected override int Handle(ParseResult result)
        {
            BaseParseResult currentResult = result;
            while (currentResult != null)
            {
                var commandResult = currentResult as ParseCommandResult;
                if (commandResult == null)
                    WriteLine("Root Command");
                else
                    WriteLine($"Command: {commandResult.Name}");
                foreach (KeyValuePair<string, object> kvp in currentResult.Options)
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
                        WriteLine($"Option {kvp.Key}: {kvp.Value}");
                }
                currentResult = currentResult.Command;
            }
            return 0;
        }

        protected override IEnumerable<Option> GetOptions()
        {
            yield return new Option("verbose") { ShortName = "v" }
                .Optional();
        }

        protected override IEnumerable<Command> GetCommands()
        {
            yield return new InstallCommand();
            yield return new UpdateCommand();
        }
    }

    public sealed class InstallCommand : CommandBuilder
    {
        protected override string Name => "install";

        protected override IEnumerable<Option> Options
        {
            get
            {
                yield return new Option("force") { ShortName = "f" }
                    .NoParameters();
            }
        }

        protected override IEnumerable<Argument> Arguments
        {
            get
            {
                yield return new Argument();
            }
        }
    }

    public sealed class UpdateCommand : CommandBuilder
    {
        protected override string Name => "update";

        protected override IEnumerable<Option> Options
        {
            get
            {
                yield return new Option("version") { ShortName = "ver" }
                    .ParamsOfType<int>()
                    .ParametersRequired();
                yield return new Option("force") { ShortName = "f" }
                    .NoParameters();
                yield return new Option("format")
                    .FormatParamsAs(v => v.ToUpperInvariant())
                    .ParametersRequired()
                    .Optional(int.MaxValue);
            }
        }

        protected override IEnumerable<Argument> Arguments
        {
            get
            {
                yield return new Argument();
            }
        }
    }
}
