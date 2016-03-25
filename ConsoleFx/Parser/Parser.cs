#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015 Jeevan James

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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using ConsoleFx.Parser.Config;
using ConsoleFx.Parser.Styles;
using ConsoleFx.Parser.Validators;

namespace ConsoleFx.Parser
{
    public class Parser
    {
        public Parser(ParserStyle parserStyle, ArgGrouping grouping = ArgGrouping.DoesNotMatter)
        {
            ParserStyle = parserStyle;
            Grouping = grouping;
        }

        public ParserStyle ParserStyle { get; }

        /// <summary>
        ///     Specifies how the args should be grouped.
        ///     Note: This can be overridden by the parser style at runtime.
        /// </summary>
        public ArgGrouping Grouping { get; set; }

        public ConfigReader ConfigReader { get; set; }

        /// <summary>
        ///     The implicit <see cref="Command" /> instance that is the root of all commands and args.
        /// </summary>
        private Command RootCommand { get; } = new Command();

        public Arguments Arguments => RootCommand.Arguments;
        public Options Options => RootCommand.Options;
        public Commands Commands => RootCommand.Commands;

        public Argument AddArgument(bool optional = false) => RootCommand.AddArgument(optional);

        public Option AddOption(string name, string shortName = null, bool caseSensitive = false,
            int order = int.MaxValue) =>
                RootCommand.AddOption(name, shortName, caseSensitive, order);

        /// <summary>
        /// Parses the given set of tokens based on the rules specified by the <see cref="Arguments"/>, <see cref="Options"/> and <see cref="Commands"/> properties.
        /// </summary>
        /// <param name="tokens">Token strings to parse.</param>
        /// <returns>A <see cref="ParseResult"/> instance.</returns>
        public ParseResult Parse(IEnumerable<string> tokens)
        {
            IReadOnlyList<string> tokenList = tokens.ToList();

            ParseRun run = CreateRun(tokenList);
            IReadOnlyList<Option> justOptions = run.Options.Select(o => o.Option).ToList();
            IReadOnlyList<Argument> justArguments = run.Arguments.Select(a => a.Argument).ToList();

            //Even though the caller can define the grouping, the parser style can override it based
            //on the available options and arguments. See the UnixParserStyle class for an example.
            Grouping = ParserStyle.GetGrouping(Grouping, justOptions, justArguments);

            //Validate all the available options based on the parser style rules.
            //See the UnixParserStyle for an example.
            ParserStyle.ValidateDefinedOptions(justOptions);

            //Identify all tokens as options or arguments. Identified option details are stored in
            //the Option instance itself. Identified arguments are returned from the method.
            List<string> specifiedArguments =
                ParserStyle.IdentifyTokens(run.Tokens, run.Options, Grouping).ToList();

            //if (ConfigReader != null)
            //    specifiedArguments = ConfigReader.Run(specifiedArguments, Options);

            //Process the specified options and arguments.
            ProcessOptions(run.Options);
            ProcessArguments(specifiedArguments, run.Arguments);

            return CreateParseResult(run);
        }

        private ParseRun CreateRun(IReadOnlyList<string> tokens)
        {
            var run = new ParseRun();

            Command currentCommand = RootCommand;

            //Go through the tokens and find all the subcommands and their arguments and options.
            for (int i = 0; i < tokens.Count; i++)
            {
                string token = tokens[i];

                run.Options.AddRange(currentCommand.Options.Select(o => new OptionRun(o, currentCommand)));

                //Check if subcommand exists under the current command with the token as a name.
                Command subcommand = currentCommand.Commands[token];

                if (subcommand != null)
                {
                    //Add the command and move to the next command level.
                    run.Commands.Add(subcommand);
                    currentCommand = subcommand;
                } else
                {
                    //Only add the arguments from the current command to the run if a subcommand is not specified.
                    //Arguments from the innermost command can only be used for a run. If arguments
                    //from different levels of commands are used, then the correct order of the commands is ambiguous.
                    foreach (Argument argument in currentCommand.Arguments)
                        run.Arguments.Add(new ArgumentRun(argument));

                    //All tokens from the current token are considered the args for the command.
                    run.Tokens = new List<string>(tokens.Count - i + 1);
                    for (int j = i; j < tokens.Count; j++)
                        run.Tokens.Add(tokens[j]);

                    //We're done with subcommands. Break out of this look.
                    break;
                }
            }

            return run;
        }

        private void ProcessOptions(IReadOnlyList<OptionRun> optionRuns)
        {
            foreach (OptionRun or in optionRuns)
            {
                //If the option is required, but is not specified.
                if (or.Option.Usage.MinOccurences > 0 && or.Occurences == 0)
                {
                    throw new ParserException(ParserException.Codes.RequiredOptionAbsent,
                        string.Format(Messages.RequiredOptionAbsent, or.Option.Name));
                }

                //If the option is specified less times than the minimum expected number of times.
                if (or.Occurences < or.Option.Usage.MinOccurences)
                {
                    throw new ParserException(ParserException.Codes.TooFewOptions,
                        string.Format(Messages.TooFewOptions, or.Option.Name, or.Option.Usage.MinOccurences));
                }

                //If the option is specified more times than the maximum allowed number of times.
                if (or.Occurences > or.Option.Usage.MaxOccurences)
                {
                    throw new ParserException(ParserException.Codes.TooManyOptions,
                        string.Format(Messages.TooManyOptions, or.Option.Name, or.Option.Usage.MaxOccurences));
                }

                //If the option with required parameters is specified, but no parameters are.
                if (or.Occurences > 0 && or.Option.Usage.MinParameters > 0 && or.Parameters.Count == 0)
                {
                    throw new ParserException(ParserException.Codes.RequiredParametersAbsent,
                        string.Format(Messages.RequiredParametersAbsent, or.Option.Name));
                }

                if (or.Occurences > 0 && or.Option.Usage.MinParameters == 0 && or.Option.Usage.MaxParameters == 0 &&
                    or.Parameters.Count > 0)
                {
                    throw new ParserException(ParserException.Codes.InvalidParametersSpecified,
                        string.Format(Messages.InvalidParametersSpecified, or.Option.Name));
                }

                //TODO: Check against MinParameters and MaxParameters

                //If the option has been used and it has parameters and validators, then validate
                //all parameters.
                if (or.Occurences > 0 && or.Parameters.Count > 0 && or.Option.Validators.Count > 0)
                {
                    for (int i = 0; i < or.Parameters.Count; i++)
                    {
                        string parameter = or.Parameters[i];

                        IEnumerable<Validator> validatorsByIndex = or.Option.Validators.GetValidators(i);
                        if (validatorsByIndex != null)
                        {
                            foreach (Validator validator in validatorsByIndex)
                                validator.Validate(parameter);
                        }
                    }
                }

                if (or.Occurences > 0)
                    or.ResolvedValue = ResolveOptionParameterValues(or);
            }
        }

        private static object ResolveOptionParameterValues(OptionRun optionRun)
        {
            Option option = optionRun.Option;

            if (option.Usage.ParameterRequirement == OptionParameterRequirement.NotAllowed)
            {
                if (option.Usage.MaxOccurences > 1)
                    return optionRun.Occurences;
                return optionRun.Occurences > 0;
            }

            Type optionType = option.Type ?? typeof(string);
            Converter<string, object> converter = option.TypeConverter;
            if (converter == null && optionType != typeof(string))
            {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(optionType);
                if (!typeConverter.CanConvertFrom(typeof(string)))
                    throw new ParserException(-1, $"Unable to find a adequate type converter to convert parameters of the {option.Name} to type {optionType.FullName}.");
                converter = value => typeConverter.ConvertFromString(value);
            }

            if (option.Usage.MaxParameters > 1 || (option.Usage.MaxParameters == 1 && option.Usage.MaxOccurences > 1))
            {
                Type listType = typeof(List<>).MakeGenericType(optionType);
                var list = (IList)Activator.CreateInstance(listType, optionRun.Parameters.Count);

                foreach (string parameter in optionRun.Parameters)
                {
                    string formattedParameter = option.Formatter != null ? option.Formatter(parameter) : parameter;
                    list.Add(converter == null ? formattedParameter : converter(formattedParameter));
                }

                return list;
            }

            if (option.Usage.MaxParameters == 1 && optionRun.Parameters.Count > 0)
            {
                string formattedParameter = option.Formatter != null ? option.Formatter(optionRun.Parameters[0]) : optionRun.Parameters[0];
                return converter == null ? formattedParameter : converter(formattedParameter);
            }

            throw new InvalidOperationException("Should never reach here");
        }

        /// <summary>
        ///     Process the specified arguments by verifying their usage, validating them and executing
        ///     their handlers.
        /// </summary>
        private void ProcessArguments(IReadOnlyList<string> specifiedArguments, IReadOnlyList<ArgumentRun> argumentRuns)
        {
            if (argumentRuns.Count == 0)
                return;

            //Throw exception of number of specified arguments is greater than number of defined arguments.
            if (specifiedArguments.Count > argumentRuns.Count)
            {
                throw new ParserException(ParserException.Codes.InvalidNumberOfArguments,
                    Messages.InvalidNumberOfArguments);
            }

            //Find the number of arguments that are required.
            int requiredArgumentCount = 0;
            while (requiredArgumentCount < argumentRuns.Count && !argumentRuns[requiredArgumentCount].Argument.IsOptional)
                requiredArgumentCount++;

            //Throw exception if not enough required arguments are specified.
            if (specifiedArguments.Count < requiredArgumentCount)
            {
                throw new ParserException(ParserException.Codes.InvalidNumberOfArguments,
                    Messages.InvalidNumberOfArguments);
            }

            //Iterate through all specified arguments and validate.
            //If validated, run the argument handler.
            for (var i = 0; i < specifiedArguments.Count; i++)
            {
                string argumentValue = specifiedArguments[i];
                Argument argument = argumentRuns[i].Argument;
                foreach (Validator validator in argument.Validators)
                    validator.Validate(argumentValue);

                argumentRuns[i].Value = argumentValue;
            }
        }

        private static ParseResult CreateParseResult(ParseRun run)
        {
            var result = new ParseResult();

            //TODO: Is there a better predicate logic here?
            IEnumerable<OptionRun> rootOptionRuns = run.Options.Where(option => option.Command.Name == null);
            foreach (OptionRun rootOptionRun in rootOptionRuns)
                result.InternalOptions.Add(rootOptionRun.Option.Name, rootOptionRun.ResolvedValue);

            BaseParseResult currentResult = result;
            foreach (Command command in run.Commands)
            {
                var commandResult = new ParseCommandResult(command.Name);
                currentResult.Command = commandResult;

                IEnumerable<OptionRun> commandOptionRuns = run.Options.Where(option => option.Command == command);
                foreach (OptionRun commandOptionRun in commandOptionRuns)
                    commandResult.InternalOptions.Add(commandOptionRun.Option.Name, commandOptionRun.ResolvedValue);
            }

            foreach (ArgumentRun argumentRun in run.Arguments)
                result.InternalArguments.Add(argumentRun.Value);

            return result;
        }
    }
}
