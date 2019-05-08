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
            command.Arguments.Add(new Argument("source"));
            command.Arguments.Add(new Argument("destination")
                .ValidateAsString(5));
            command.Options.Add(new Option("v")
                .UsedAsFlag());
            command.Options.Add(new Option("y")
                .UsedAsFlag());
            command.Options.Add(new Option("trace", "t")
                .UsedAsSingleParameter()
                .ValidateAsEnum<TraceLevel>().ParamsOfType<string>());
            command.Options.Add(new Option("repeat", "r")
                .UsedAsSingleParameter()
                .ValidateAsInteger(0, 3)
                .ParamsOfType<int>());
            command.Options.Add(new Option("log", "l")
                .UsedAsSingleParameter()
                .FormatParamsAs(s => $"Log{s}"));
            command.Options.Add(new Option("url", "u", "web")
                .UsedAsSingleParameter()
                .ValidateAsUri()
                .ParamsOfType<Uri>());
            command.Options.Add(new Option("id")
                .UsedAsSingleParameter()
                .ValidateAsGuid());
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
