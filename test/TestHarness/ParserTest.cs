using System;

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineArgs.Validators;
using ConsoleFx.CmdLineParser;
using ConsoleFx.CmdLineParser.Style;

namespace TestHarness
{
    internal sealed class ParserTest : TestBase
    {
        internal override void Run()
        {
            var command = new RootCommand();
            command.AddArgument("source");
            command.AddArgument("destination")
                .ValidateAsString(5);
            command.AddOption("v")
                .UsedAsFlag();
            command.AddOption("y")
                .UsedAsFlag();
            command.AddOption("trace", "t")
                .UsedAsSingleParameter()
                .ValidateAsEnum<TraceLevel>().TypedAs<string>();
            command.AddOption("repeat", "r")
                .UsedAsSingleParameter()
                .ValidateAsInteger(0, 3)
                .TypedAs<int>();
            command.AddOption("log", "l")
                .UsedAsSingleParameter()
                .FormatAs("Log{0}");
            command.AddOption("url", "u", "web")
                .UsedAsSingleParameter()
                .ValidateAsUri();
            command.AddOption("id")
                .UsedAsSingleParameter()
                .ValidateAsGuid();
            command.Handler = (args, opts) =>
            {
                foreach (string arg in args)
                    Console.WriteLine(arg);
                foreach (System.Collections.Generic.KeyValuePair<string, object> kvp in opts)
                    Console.WriteLine($"{kvp.Key} = {kvp.Value}");
                return 0;
            };

            var parser = new Parser(command, ArgStyle.Windows);
            ParseResult result = parser.Parse("sourceFile", "destfile", "/v", "/trace:debug", "/r:2", "-log:blah",
                "/web:https://example.com", "-id:{DD45218B-CE76-4714-A3B3-7F77F4A287F1}");
            result.Command.Handler(result.Arguments, result.Options);
        }
    }

    public enum TraceLevel
    {
        Debug,
        Info,
        Warning,
        Error,
    }
}
