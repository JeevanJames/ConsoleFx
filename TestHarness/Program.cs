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
            int exitCode = new NpmProgram(new UnixParserStyle()).Run();
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
            Command command = result.Command;
            if (command == null)
                WriteLine(@"Root Command");
            else
                WriteLine($"Command: {command.Name}");
            foreach (KeyValuePair<string, object> kvp in result.Options)
            {
                Write($"Option {kvp.Key}: ");
                var list = kvp.Value as IList;
                if (list != null)
                {
                    foreach (object item in list)
                        Write($"{item?.ToString() ?? "(null)"}, ");
                    WriteLine();
                } else
                    WriteLine(kvp.Value);
            }
            return 0;
        }

        protected override IEnumerable<Option> GetOptions()
        {
            yield return new Option("verbose") {ShortName = "v"};
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
                yield return new Option("force") { ShortName = "f" };
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
                yield return new Option("version") {ShortName = "ver"}
                    .ParamsOfType<int>()
                    .UsedAs(usage => {
                        usage.ParameterRequirement = OptionParameterRequirement.Required;
                    });
                yield return new Option("force") { ShortName = "f" };
                yield return new Option("format")
                    .FormatParamsAs(v => v.ToUpperInvariant())
                    .UsedAs(usage => {
                        usage.Requirement = OptionRequirement.OptionalUnlimited;
                        usage.ParameterRequirement = OptionParameterRequirement.Required;
                    });
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
