using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ConsoleFx.ConsoleExtensions;
using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;
using ConsoleFx.Programs;

using static ConsoleFx.ConsoleExtensions.ConsoleEx;

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
            WriteLineColor("[BgRed]Node Package Manager [BgGray.Black](npm) [BgYellow]by Node.js");
            WriteLineColor(new ColorString()
                .BgRed("Node Package Manager ")
                .BgGray().Black("(npm) ")
                .BgYellow("by Node.js")
            );
            WriteLineColor(@"Copyright (c) 2015-16 Jeevan James");
            string password = ReadSecret("Enter password: ", hideCursor: false, hideMask: false, needValue: true);
            string name = Prompt("[blue.bgwhite]Enter name: ");
            WriteLineColor($"{name} : {password}");
            int exitCode = new NpmProgram(new UnixParserStyle()).Run();
            if (Debugger.IsAttached)
            {
                char key = WaitForKeys(ignoreCase: true, 'Y', 'n');
                WriteLineColor(key.ToString());
                WaitForAnyKey();
            }

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
                WriteLineColor(@"Root Command");
            else
                WriteLineColor($"Command: {command.Name}");
            foreach (KeyValuePair<string, object> kvp in result.Options)
            {
                WriteColor($"Option {kvp.Key}: ");
                var list = kvp.Value as IList;
                if (list != null)
                {
                    foreach (object item in list)
                        WriteColor($"{item?.ToString() ?? "(null)"}, ");
                    WriteBlankLine();
                } else
                    WriteLineColor(kvp.Value.ToString());
            }
            return 0;
        }

        protected override IEnumerable<Option> GetOptions()
        {
            yield return new Option("verbose", "v");
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
                yield return new Option("force", "f");
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
                yield return new Option("version", "ver")
                    .ParamsOfType<int>()
                    .UsedAs(usage => {
                        usage.ParameterRequirement = OptionParameterRequirement.Required;
                    });
                yield return new Option("force", "f");
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
