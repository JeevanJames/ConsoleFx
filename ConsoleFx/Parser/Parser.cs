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

        /// <summary>
        ///     Parses the given set of tokens based on the rules specified by the <see cref="Arguments" />, <see cref="Options" />
        ///     and <see cref="Commands" /> properties.
        /// </summary>
        /// <param name="tokens">Token strings to parse.</param>
        /// <returns>A <see cref="ParseResult" /> instance.</returns>
        public ParseResult Parse(IEnumerable<string> tokens)
        {
            IReadOnlyList<string> tokenList = tokens.ToList();

            ParseRun run = CreateRun(tokenList);

            //Extract out just the option and argument objects from their respective run collections.
            //We want to pass these to parser style methods that only need to deal with the Option and
            //Argument objects and not have to deal with the 'run' aspects. See some of the calls to
            //protected methods from this method.
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

            //TODO: Come back to this later
            //if (ConfigReader != null)
            //    specifiedArguments = ConfigReader.Run(specifiedArguments, Options);

            //Process the specified options and arguments, and resolve their values.
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

            if (run.Tokens == null)
                run.Tokens = new List<string>(0);
            return run;
        }

        private static void ProcessOptions(IReadOnlyList<OptionRun> optionRuns)
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

        /// <summary>
        ///     Resolves an <see cref="Option" />'s value based on it's usage details. See the comments on the
        ///     <see cref="OptionRun.ResolvedValue" /> property for details on how the resolution is done.
        /// </summary>
        /// <param name="optionRun">The <see cref="OptionRun" /> instance, whose option to resolve.</param>
        /// <returns>The value of the option.</returns>
        private static object ResolveOptionParameterValues(OptionRun optionRun)
        {
            Option option = optionRun.Option;

            //If parameters are not allowed on the option...
            if (option.Usage.ParameterRequirement == OptionParameterRequirement.NotAllowed)
            {
                //If the option can occur more than once, it's value will be an integer specifying
                //the number of occurences.
                if (option.Usage.MaxOccurences > 1)
                    return optionRun.Occurences;

                //If the option can occur not more than once, it's value will be a bool indicating
                //whether it was specified or not.
                return optionRun.Occurences > 0;
            }

            //If no type is specified, assume string.
            Type optionType = option.Type ?? typeof(string);
            Converter<string, object> converter = option.TypeConverter;

            //If a custom type converter is not specified and the option's value type is not string,
            //then attempt to find a default type converter for that type, which can convert from string.
            if (converter == null && optionType != typeof(string))
            {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(optionType);
                //If a default converter cannot be found, throw an exception.
                if (!typeConverter.CanConvertFrom(typeof(string)))
                    throw new ParserException(-1,
                        $"Unable to find a adequate type converter to convert parameters of the {option.Name} to type {optionType.FullName}.");
                converter = value => typeConverter.ConvertFromString(value);
            }

            //If the option can have multiple parameter values (either because the MaxParameters usage
            //is greater than one or because MaxParameters is one but MaxOccurences is greater than
            //one), then the option's value is an IList<Type>.
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
                string formattedParameter = option.Formatter != null
                    ? option.Formatter(optionRun.Parameters[0]) : optionRun.Parameters[0];
                return converter == null ? formattedParameter : converter(formattedParameter);
            }

            throw new InvalidOperationException("Should never reach here");
        }

        /// <summary>
        ///     Process the specified arguments by verifying their usage, validating them and executing
        ///     their handlers.
        /// </summary>
        private static void ProcessArguments(IReadOnlyList<string> specifiedArguments,
            IReadOnlyList<ArgumentRun> argumentRuns)
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
            while (requiredArgumentCount < argumentRuns.Count &&
                !argumentRuns[requiredArgumentCount].Argument.IsOptional)
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
            Command finalCommand = run.Commands.Count > 0 ? run.Commands[run.Commands.Count - 1] : null;
            List<string> arguments = run.Arguments
                .Select(ar => ar.Value)
                .ToList();
            Dictionary<string, object> options = run.Options
                .Where(option => option.Command.Name == null)
                .ToDictionary(rootOptionRun => rootOptionRun.Option.Name, rootOptionRun => rootOptionRun.ResolvedValue);
            foreach (Command command in run.Commands)
            {
                IEnumerable<OptionRun> commandOptionRuns = run.Options.Where(option => option.Command == command);
                foreach (OptionRun commandOptionRun in commandOptionRuns)
                    options.Add(commandOptionRun.Option.Name, commandOptionRun.ResolvedValue);
            }

            return new ParseResult(finalCommand, arguments, options);
        }
    }
}
