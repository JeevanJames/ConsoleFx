using System;
using System.Collections.Generic;
using System.Linq;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;

namespace ConsoleFx.Programs
{
    public abstract class ConsoleProgram
    {
        private readonly ParserStyle _parserStyle;

        protected ConsoleProgram(ParserStyle parserStyle)
        {
            if (parserStyle == null)
                throw new ArgumentNullException(nameof(parserStyle));
            _parserStyle = parserStyle;
        }

        protected virtual ArgGrouping Grouping => ArgGrouping.DoesNotMatter;

        protected virtual IEnumerable<Command> GetCommands()
        {
            yield break;
        }

        protected virtual IEnumerable<Option> GetOptions()
        {
            yield break;
        }

        protected virtual IEnumerable<Argument> GetArguments()
        {
            yield break;
        }

        protected abstract int Handle(ParseResult result);

        public int Run()
        {
            var parser = new Parser.Parser(_parserStyle, Grouping);
            foreach (Argument argument in GetArguments())
                parser.Arguments.Add(argument);
            foreach (Option option in GetOptions())
                parser.Options.Add(option);
            foreach (Parser.Command command in GetCommands())
                parser.Commands.Add(command);
            try
            {
                ParseResult result = parser.Parse(Environment.GetCommandLineArgs().Skip(1));
                return Handle(result);
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }
        }
    }
}