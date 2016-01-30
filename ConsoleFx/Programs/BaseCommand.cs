using System;
using System.Collections.Generic;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;
using ConsoleFx.Utilities;

namespace ConsoleFx.Programs
{
    public abstract class BaseCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ConsoleFx.Programs.BaseCommand"/> class.
        /// </summary>
        protected BaseCommand(ParserStyle parserStyle)
        {
            ParserStyle = parserStyle;
        }

        protected ParserStyle ParserStyle { get; set; }

        protected CommandGrouping Grouping { get; set; }

        protected virtual IEnumerable<Argument> GetArguments()
        {
            yield break;
        }

        protected virtual IEnumerable<Option> GetOptions()
        {
            yield break;
        }

        protected int CoreRun(IEnumerable<string> args)
        {
            try
            {
                if (ParserStyle == null)
                    throw new Exception("Cannot parse arguments without a parser style.");
                Parser.Parser parser = SetupParser();
                parser.Parse(args);
                return Handle();
            } catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        protected virtual Parser.Parser SetupParser()
        {
            var parser = new Parser.Parser(ParserStyle, Grouping, this);
            foreach (Argument argument in GetArguments())
                parser.Arguments.Add(argument);
            foreach (Option option in GetOptions())
                parser.Options.Add(option);
            return parser;
        }

        protected abstract int Handle();

        protected virtual int HandleError(Exception ex)
        {
#if DEBUG
            Console.WriteLine(ex);
#endif

            var cfxException = ex as ConsoleFxException;

#if !DEBUG
    //If the exception derives from ArgumentException or it derives from ConsoleFxException
    //and has a negative error code, treat it as an internal exception.
            if (ex is ArgumentException || (cfxException != null && cfxException.ErrorCode < 0))
                Console.WriteLine(Messages.InternalError, ex.Message);
            else
                Console.WriteLine(ex.Message);
#endif

            ConsoleEx.WriteBlankLine();

            return cfxException?.ErrorCode ?? -1;
        }

        protected Argument CreateArgument(bool optional = false) => new Argument { IsOptional = optional };

        protected Option CreateOption(string name, string shortName = null, bool caseSensitive = false,
            int order = int.MaxValue)
        {
            var option = new Option(name) {
                CaseSensitive = caseSensitive,
                Order = order
            };
            if (!string.IsNullOrWhiteSpace(shortName))
                option.ShortName = shortName;

            return option;
        }
    }
}
