﻿#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2019 Jeevan James

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
using System.Linq;

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineArgs.Validators;
using ConsoleFx.CmdLineArgs.Validators.Bases;
using ConsoleFx.CmdLineParser.Config;
using ConsoleFx.CmdLineParser.Runs;
using ConsoleFx.CmdLineParser.Style;

namespace ConsoleFx.CmdLineParser
{
    public sealed class Parser
    {
        public Parser(Command command, ArgStyle argStyle, ArgGrouping grouping = ArgGrouping.DoesNotMatter)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));
            if (argStyle is null)
                throw new ArgumentNullException(nameof(argStyle));

            Command = command;
            ArgStyle = argStyle;
            Grouping = grouping;
        }

        /// <summary>
        ///     Gets the <see cref="Command"/> instance that specifies the parsing details and rules.
        /// </summary>
        public Command Command { get; }

        public ArgStyle ArgStyle { get; }

        /// <summary>
        ///     Gets or sets how the args should be grouped.
        ///     <para/>
        ///     Note: This can be overridden by the parser style at runtime.
        /// </summary>
        public ArgGrouping Grouping { get; set; }

        public ConfigReader ConfigReader { get; set; }

        public ParseResult Parse(IEnumerable<string> tokens) =>
            Parse(tokens.ToArray());

        /// <summary>
        ///     Parses the given set of tokens based on the rules specified by the <see cref="Arguments" />,
        ///     <see cref="Options" /> and <see cref="Commands" /> properties.
        /// </summary>
        /// <param name="tokens">Token strings to parse.</param>
        /// <returns>A <see cref="ParseResult" /> instance.</returns>
        public ParseResult Parse(params string[] tokens)
        {
            IReadOnlyList<string> tokenList = tokens.ToList();

            // Creates a ParseRun instance, which specifies the sequence of commands specified and
            // the tokens and any options and arguments that apply to the specified commands.
            ParseRun run = CreateRun(tokenList);

            // Extract out just the option and argument objects from their respective run collections.
            // We want to pass these to parser style methods that only need to deal with the Option
            // and Argument objects and not have to deal with the 'run' aspects. See some of the calls
            // to protected methods from this method.
            IReadOnlyList<Option> justOptions = run.Options.Select(o => o.Option).ToList();
            IReadOnlyList<Argument> justArguments = run.Arguments.Select(a => a.Argument).ToList();

            // Even though the caller can define the grouping, the parser style can override it based
            // on the available options and arguments. See the UnixArgStyle class for an example.
            Grouping = ArgStyle.GetGrouping(Grouping, justOptions, justArguments);

            // Validate all the available options based on the parser style rules.
            // See the UnixArgStyle for an example.
            ArgStyle.ValidateDefinedOptions(justOptions);

            // Identify all tokens as options or arguments. Identified option details are stored in
            // the Option instance itself. Identified arguments are returned from the method.
            List<string> specifiedArguments =
                ArgStyle.IdentifyTokens(run.Tokens, run.Options, Grouping).ToList();

            //TODO: Come back to this later
            //if (ConfigReader != null)
            //    specifiedArguments = ConfigReader.Run(specifiedArguments, Options);

            // Process the specified options and arguments, and resolve their values.
            ProcessOptions(run.Options);
            ProcessArguments(specifiedArguments, run.Arguments, run.Commands[run.Commands.Count - 1]);

            var parseResult = new ParseResult(run);

            CommandCustomValidator customCommandValidator = parseResult.Command.CustomValidator;
            string validationError = customCommandValidator?.Invoke(parseResult.Arguments, parseResult.Options);
            if (validationError != null)
                throw new ValidationException(validationError, null, null);

            return parseResult;
        }

        private ParseRun CreateRun(IReadOnlyList<string> tokens)
        {
            var run = new ParseRun();

            run.Commands.Add(Command);
            Command currentCommand = Command;

            // Go through the tokens and find all the subcommands and their arguments and options.
            for (int i = 0; i < tokens.Count; i++)
            {
                string token = tokens[i];

                // All options for the current command are considered. So, add them all.
                // This is not the case for arguments. Only the arguments for the innermost command are considered.
                //run.Options.AddRange(currentCommand.Options.Select(o => new OptionRun(o, currentCommand)));

                // Check if subcommand exists under the current command with the token as a name.
                Command subcommand = currentCommand.Commands[token];

                if (subcommand != null)
                {
                    // Add the command and move to the next command level.
                    run.Commands.Add(subcommand);
                    currentCommand = subcommand;
                }
                else
                {
                    // This is the innermost command. Add any arguments from this command to the run
                    // and then add all the remaining tokens to the run's Token collection and exit
                    // the loop.

                    // Add all the options for the current command.
                    run.Options.AddRange(currentCommand.Options.Select(o => new OptionRun(o)));

                    // Add all the arguments for the current command.
                    run.Arguments.AddRange(currentCommand.Arguments.Select(a => new ArgumentRun(a)));

                    // All tokens from the current token are considered the args for the command.
                    run.Tokens = new List<string>(tokens.Count - i + 1);
                    for (int j = i; j < tokens.Count; j++)
                        run.Tokens.Add(tokens[j]);

                    // We're done with subcommands. Break out of this loop.
                    break;
                }
            }

            // To avoid null-ref exceptions in case no tokens are specified, assign run.Tokens if it
            // is null.
            if (run.Tokens is null)
                run.Tokens = new List<string>(0);

            return run;
        }

        /// <summary>
        ///     Process the specified options by verifying their usage, validating them and executing
        ///     their handler.
        /// </summary>
        /// <param name="optionRuns">The option run details.</param>
        /// <exception cref="ParserException">
        ///     Thrown if any of the validation or usage checks on the <see cref="OptionRun"/> objects
        ///     fails.
        /// </exception>
        private static void ProcessOptions(IReadOnlyList<OptionRun> optionRuns)
        {
            foreach (OptionRun or in optionRuns)
            {
                // If the option is required, but is not specified at all, then assign a default value,
                // if available.
                if (or.Option.Usage.MinOccurrences > 0 && or.Occurrences == 0)
                {
                    if (or.Option.DefaultSetter is null)
                    {
                        throw new ParserException(ParserException.Codes.RequiredOptionAbsent,
                            string.Format(Messages.RequiredOptionAbsent, or.Option.Name));
                    }

                    object defaultValue = or.Option.DefaultSetter();
                    if (or.Option.Usage.MaxOccurrences > 1 || or.Option.Usage.MaxParameters > 1)
                        or.Value = new List<object>(1) { defaultValue };
                }

                // If the option is specified less times than the minimum expected number of times.
                if (or.Occurrences < or.Option.Usage.MinOccurrences)
                {
                    throw new ParserException(ParserException.Codes.TooFewOptions,
                        string.Format(Messages.TooFewOptions, or.Option.Name, or.Option.Usage.MinOccurrences));
                }

                // If the option is specified more times than the maximum allowed number of times.
                if (or.Occurrences > or.Option.Usage.MaxOccurrences)
                {
                    throw new ParserException(ParserException.Codes.TooManyOptions,
                        string.Format(Messages.TooManyOptions, or.Option.Name, or.Option.Usage.MaxOccurrences));
                }

                // If the option with required parameters is specified, but no parameters are.
                if (or.Occurrences > 0 && or.Option.Usage.MinParameters > 0 && or.Parameters.Count == 0)
                {
                    throw new ParserException(ParserException.Codes.RequiredParametersAbsent,
                        string.Format(Messages.RequiredParametersAbsent, or.Option.Name));
                }

                // If the option does not specify parameters by some are specified.
                if (or.Occurrences > 0 && or.Option.Usage.MinParameters == 0 && or.Option.Usage.MaxParameters == 0 &&
                    or.Parameters.Count > 0)
                {
                    throw new ParserException(ParserException.Codes.InvalidParametersSpecified,
                        string.Format(Messages.InvalidParametersSpecified, or.Option.Name));
                }

                //TODO: Check against MinParameters and MaxParameters

                // If the option has been used and it has parameters and validators, then validate
                // all parameters.
                if (or.Occurrences > 0 && or.Parameters.Count > 0 && or.Option.Validators.Count > 0)
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

                if (or.Occurrences > 0)
                    or.Value = ResolveOptionParameterValues(or);

                // Option values can only null for objects. For all other value types, assign a default
                // value.
                if (or.Value is null)
                {
                    switch (or.ValueType)
                    {
                        case OptionValueType.List:
                            or.Value = or.CreateCollection(0);
                            break;
                        case OptionValueType.Count:
                            or.Value = 0;
                            break;
                        case OptionValueType.Flag:
                            or.Value = false;
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Resolves an <see cref="Option" />'s value based on it's usage details. See the
        ///     <see cref="OptionRun.GetOptionValueType(Option)" /> method for details on how the
        ///     resolution is done.
        /// </summary>
        /// <param name="optionRun">The <see cref="OptionRun" /> instance, whose option to resolve.</param>
        /// <returns>The value of the option.</returns>
        private static object ResolveOptionParameterValues(OptionRun optionRun)
        {
            switch (optionRun.ValueType)
            {
                case OptionValueType.Count:
                    return optionRun.Occurrences;

                case OptionValueType.Flag:
                    return optionRun.Occurrences > 0;

                case OptionValueType.List:
                    IList list = optionRun.CreateCollection(optionRun.Parameters.Count);
                    foreach (string parameter in optionRun.Parameters)
                        list.Add(optionRun.Convert(parameter));
                    return list;

                case OptionValueType.Object:
                    return optionRun.Convert(optionRun.Parameters[0]);
            }

            //TODO: Change this to an internal parser exception.
            throw new InvalidOperationException("Should never reach here");
        }

        /// <summary>
        ///     Process the specified arguments by verifying their usage, validating them and executing
        ///     their handlers.
        /// </summary>
        /// <param name="specifiedArguments">The list of specified arguments.</param>
        /// <param name="argumentRuns">The argument run details.</param>
        /// <param name="command">The <see cref="Command"/> that the arguments belong to.</param>
        /// <exception cref="ParserException">
        ///     Thrown if any of the validation or usage checks on the <see cref="ArgumentRun"/> objects
        ///     fails.
        /// </exception>
        private static void ProcessArguments(IReadOnlyList<string> specifiedArguments, IReadOnlyList<ArgumentRun> argumentRuns, Command command)
        {
            // Throw exception if number of specified arguments is greater than number of defined
            // arguments. Account for repeated last arguments.
            if (specifiedArguments.Count > argumentRuns.Count + command.LastArgumentRepeat - 1)
            {
                //TODO: The error message is too generic. Change to mention the extra arguments.
                throw new ParserException(ParserException.Codes.InvalidNumberOfArguments,
                    Messages.InvalidNumberOfArguments);
            }

            // If there are no argument runs, there is nothing to process, so exit.
            if (argumentRuns.Count == 0)
                return;

            // Find the number of arguments that are required.
            int requiredArgumentCount = 0;
            while (requiredArgumentCount < argumentRuns.Count &&
                !argumentRuns[requiredArgumentCount].Argument.IsOptional)
                requiredArgumentCount++;

            // Throw exception if not enough required arguments are specified.
            if (specifiedArguments.Count < requiredArgumentCount)
            {
                //TODO: The error message is too generic. Change to specify the first argument that's missing.
                throw new ParserException(ParserException.Codes.InvalidNumberOfArguments,
                    Messages.InvalidNumberOfArguments);
            }

            // Iterate through all specified arguments and validate.
            // If validated, assign the value.
            for (int i = 0; i < argumentRuns.Count; i++)
            {
                ArgumentRun argumentRun = argumentRuns[i];
                Argument argument = argumentRuns[i].Argument;

                // If there is a specified argument for this run.
                if (i < specifiedArguments.Count)
                {
                    string argumentValue = specifiedArguments[i];
                    foreach (Validator validator in argument.Validators)
                        validator.Validate(argumentValue);

                    argumentRun.Assigned = true;
                    argumentRun.Value = i == argumentRuns.Count - 1 && command.LastArgumentRepeat > 1
                        ? ResolveArgumentValue(argumentRun, specifiedArguments, i, specifiedArguments.Count - 1)
                        : ResolveArgumentValue(argumentRun, specifiedArguments, i, i);
                }

                // No specified argument, but there is a default value.
                else if (argument.DefaultSetter != null)
                {
                    // Note: For default values, none of the validators are run. This enables special
                    // default values to be assigned that are outside the rules of validation.
                    argumentRun.Assigned = true;
                    object value = argument.DefaultSetter();
                    if (command.LastArgumentRepeat > 1)
                    {
                        IList list = argumentRun.CreateCollection(1);
                        list.Add(value);
                        argumentRun.Value = list;
                    }
                    else
                        argumentRun.Value = value;
                }
            }
        }

        private static object ResolveArgumentValue(ArgumentRun argumentRun, IReadOnlyList<string> specifiedArguments,
            int startIndex, int endIndex)
        {
            if (startIndex == endIndex)
                return argumentRun.Convert(specifiedArguments[startIndex]);

            IList list = argumentRun.CreateCollection(endIndex - startIndex + 1);
            for (var j = startIndex; j <= endIndex; j++)
                list.Add(argumentRun.Convert(specifiedArguments[j]));
            return list;
        }
    }
}
