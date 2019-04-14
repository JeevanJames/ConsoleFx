using System;
using System.Collections.Generic;

using ConsoleFx.CmdLineParser;
using ConsoleFx.CmdLineParser.Validators;
using ConsoleFx.CmdLineParser.WindowsStyle;

namespace TestHarness
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var parser = new Parser(new WindowsParserStyle());
            parser.Commands.Add(new InstallCommand());
            ParseResult result = parser.Parse("install", "swagen", "-s=http://myget.com");
            Console.WriteLine(result);
        }
    }

    public sealed class InstallCommand : Command
    {
        /// <inheritdoc />
        public InstallCommand()
            : base("install", "i")
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<Argument> GetArguments()
        {
            yield return new Argument("packageName");
        }

        /// <inheritdoc />
        protected override IEnumerable<Option> GetOptions()
        {
            yield return new Option("source", "s")
                .UsedAsSingleParameter()
                .ValidateAsUri(UriKind.Absolute);
        }
    }
}
