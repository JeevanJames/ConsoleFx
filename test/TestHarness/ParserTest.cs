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
            command.Handler = (args, opts) =>
            {
                foreach (string arg in args)
                    Console.WriteLine(arg);
                foreach (System.Collections.Generic.KeyValuePair<string, object> kvp in opts)
                    Console.WriteLine($"{kvp.Key} = {kvp.Value}");
                return 0;
            };

            var parser = new Parser(command, ArgStyle.Windows);
            ParseResult result = parser.Parse("sourceFile", "destfile", "/v");
            result.Command.Handler(result.Arguments, result.Options);
        }
    }
}
