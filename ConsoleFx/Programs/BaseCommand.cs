#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015-2016 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System;
using System.Collections.Generic;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;
using ConsoleFx.Utilities;

namespace ConsoleFx.Programs
{
    /// <summary>
    ///     Represents a single command that can be run from the command line and all attributes and behavior associated with
    ///     it.
    /// </summary>
    public abstract class BaseCommand
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ConsoleFx.Programs.BaseCommand" /> class.
        /// </summary>
        protected BaseCommand(ParserStyle parserStyle)
        {
            ParserStyle = parserStyle;
        }

        /// <summary>
        ///     The parser style to use for this command.
        /// </summary>
        protected ParserStyle ParserStyle { get; set; }

        /// <summary>
        ///     The expected grouping of the args of this command.
        /// </summary>
        protected ArgGrouping Grouping { get; set; }

        protected int CoreRun(IEnumerable<string> args)
        {
            if (ParserStyle == null)
                throw new Exception("Cannot parse arguments without a parser style.");
            Parser.Parser parser = SetupParser();
            try
            {
                parser.Parse(args);
                return Handle();
            }
            catch (Exception ex)
            {
                return HandleError(ex, parser);
            }
        }

        /// <summary>
        ///     Override this method to specify the arguments supported by the command.
        /// </summary>
        /// <returns>Collection of <see cref="ConsoleFx.Parser.Argument" /> instance.</returns>
        protected virtual IEnumerable<Argument> GetArguments()
        {
            yield break;
        }

        /// <summary>
        ///     Override this method to specify the options supported by the command.
        /// </summary>
        /// <returns>Collection of <see cref="ConsoleFx.Parser.Option" /></returns>
        protected virtual IEnumerable<Option> GetOptions()
        {
            yield break;
        }

        /// <summary>
        ///     Constructs the default <see cref="ConsoleFx.Parser.Parser" /> instance for use by this command. Override this
        ///     method to customize the parser instance.
        /// </summary>
        /// <returns>The <see cref="ConsoleFx.Parser.Parser" /> instance.</returns>
        protected virtual Parser.Parser SetupParser()
        {
            var parser = new Parser.Parser(ParserStyle, Grouping, this);
            foreach (Argument argument in GetArguments())
                parser.Arguments.Add(argument);
            foreach (Option option in GetOptions())
                parser.Options.Add(option);
            return parser;
        }

        /// <summary>
        ///     Override this method to provide the default logic of the command.
        /// </summary>
        /// <returns>The exit code for the command. Zero for successful exit codes and non-zero for failures.</returns>
        protected abstract int Handle();

        /// <summary>
        ///     Handles any exceptions thrown during the setup and execution of the command.
        ///     Override this method to provide custom handling of exceptions.
        /// </summary>
        /// <param name="ex">The thrown exception.</param>
        /// <param name="parser">
        ///     The <see cref="ConsoleFx.Parser.Parser" /> instance used by the command. Useful for dynamically
        ///     constructing the usage details.
        /// </param>
        /// <returns>The exit code for the command. Zero for successful exit codes and non-zero for failures.</returns>
        protected virtual int HandleError(Exception ex, Parser.Parser parser)
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

        /// <summary>
        ///     Utility method to create an <see cref="ConsoleFx.Parser.Argument" />
        /// </summary>
        /// <param name="optional">True if the argument is optional; otherwise false.</param>
        /// <returns>An instance of the new argument.</returns>
        protected Argument CreateArgument(bool optional = false) => new Argument { IsOptional = optional };

        /// <summary>
        ///     Utility method to create an <see cref="ConsoleFx.Parser.Option" />
        /// </summary>
        /// <param name="name">Name of the option</param>
        /// <param name="shortName">Optional short name of the option</param>
        /// <param name="caseSensitive">True if the option name and short name are case sensitive; otherwise false.</param>
        /// <param name="order">The priority for processing the option. Lower priorities are process first.</param>
        /// <returns>An instance of the new option.</returns>
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