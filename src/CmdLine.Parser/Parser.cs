#region --- License & Copyright Notice ---
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
using System.Text.RegularExpressions;

using ConsoleFx.CmdLine.Parser.Runs;
using ConsoleFx.CmdLine.Parser.Style;
using ConsoleFx.CmdLine.Validators;
using ConsoleFx.CmdLine.Validators.Bases;

using RegexCapture = System.Text.RegularExpressions.Capture;

namespace ConsoleFx.CmdLine.Parser
{
    public sealed partial class Parser
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

        //TODO: To be implemented later
        /* public ConfigReader ConfigReader { get; set; } */

        /// <summary>
        ///     Parses the given set of tokens based on the rules specified by the <see cref="Command"/>.
        /// </summary>
        /// <param name="tokens">Token strings to parse.</param>
        /// <returns>A <see cref="ParseResult" /> instance.</returns>
        public ParseResult Parse(params string[] tokens)
        {
            return Parse((IEnumerable<string>)tokens);
        }

        /// <summary>
        ///     Parses the given set of tokens based on the rules specified by the <see cref="Command"/>.
        /// </summary>
        /// <param name="tokens">Token strings to parse.</param>
        /// <returns>A <see cref="ParseResult" /> instance.</returns>
        public ParseResult Parse(IEnumerable<string> tokens)
        {
            DebugOutput.Write("Tokens passed", tokens);

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

            // Get the groups that match the specified options and arguments.
            IReadOnlyList<int> matchingGroups = GetMatchingGroups(run, specifiedArguments);

            // If no groups match, we cannot proceed, so throw a parser exception.
            if (matchingGroups.Count == 0)
                throw new ParserException(-1, "The specified arguments and options are invalid.");

            // Trim the argument and option runs to those that contain the matcing groups.
            TrimRunsToMatchingGroups(run, matchingGroups);

            // Process the specified options and arguments, and resolve their values.
            ProcessOptions(run.Options);
            ProcessArguments(specifiedArguments, run.Arguments);

            var parseResult = new ParseResult(run);

            // Runs the custom validator, if it is assigned.
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
                    // All tokens from the current token are considered the args for the command.
                    run.Tokens = new List<string>(tokens.Count - i + 1);
                    for (int j = i; j < tokens.Count; j++)
                        run.Tokens.Add(tokens[j]);

                    // We're done with subcommands. Break out of this loop.
                    break;
                }
            }

            // Add all the options for the current command.
            run.Options.AddRange(currentCommand.Options.Select(o => new OptionRun(o)));

            // Add all the arguments for the current command.
            run.Arguments.AddRange(currentCommand.Arguments.Select(a => new ArgumentRun(a)));

            // To avoid null-ref exceptions in case no tokens are specified, assign run.Tokens if it
            // is null.
            if (run.Tokens is null)
                run.Tokens = new List<string>(0);

            return run;
        }

        /// <summary>
        ///     Figure out the groups that match the specified options and arguments.
        ///     <para/>
        ///     Only arguments and options that have those groups will be considered for further
        ///     processing.
        /// </summary>
        /// <param name="run">
        ///     The <see cref="ParseRun"/> instance that contains the specified options.
        /// </param>
        /// <param name="specifiedArguments">The specified arguments.</param>
        /// <exception cref="ParserException">
        ///     Thrown if args from different groups are specified.
        /// </exception>
        private IReadOnlyList<int> GetMatchingGroups(ParseRun run, IList<string> specifiedArguments)
        {
            IReadOnlyList<OptionRun> specifiedOptions = run.Options.Where(or => or.Occurrences > 0).ToList();

            IEnumerable<int> groups = null;
            if (specifiedOptions.Count > 0)
            {
                groups = specifiedOptions[0].Option.Groups;
                if (specifiedOptions.Count > 1)
                {
                    for (int i = 1; i < specifiedOptions.Count; i++)
                    {
                        groups = groups.Intersect(specifiedOptions[i].Option.Groups);
                        if (!groups.Any())
                            throw new ParserException(-1, $"The '{specifiedOptions[i - 1].Option.Name}' option cannot be specified with the '{specifiedOptions[i].Option.Name}'.");
                    }
                }
            }

            if (groups is null)
                groups = run.Arguments.SelectMany(ar => ar.Argument.Groups).Distinct();

            var matchingGroups = new List<int>();
            foreach (int group in groups)
            {
                // Find all argument runs that have the group.
                IList<ArgumentRun> groupArguments = run.Arguments
                    .Where(ar => ar.Argument.Groups.Contains(group))
                    .ToList();

                // Calculate the minimum and maximum possible number of arguments that can be specified,
                // based on the selected argument groups.
                int minOccurences = groupArguments.Count(ar => !ar.Argument.IsOptional);
                int maxOccurences = groupArguments.Count == 0 ? 0
                    : groupArguments.Count + groupArguments[groupArguments.Count - 1].Argument.MaxOccurences - 1;

                // If the number of specified arguments falls into the calculated range, then this
                // group is valid, so add it to the list.
                if (specifiedArguments.Count >= minOccurences && specifiedArguments.Count <= maxOccurences)
                    matchingGroups.Add(group);
            }

            // In case no groups remain, we have two possibilities.
            if (matchingGroups.Count == 0)
            {
                // Check if all options and arguments are optional.
                bool allArgsOptional = run.Options.All(or => or.Option.Usage.MinOccurrences == 0) && run.Arguments.All(ar => ar.Argument.IsOptional);

                // If all options and arguments are optional, then no args were specified for this
                // command, so simply return a default of group 0.
                if (allArgsOptional)
                    return new[] { 0 };

                // If at least one option or argument is required, then this is an invalid scenario and we should throw an exception.
                throw new ParserException(-1, "You have specified args that do not go together.");
            }

            return matchingGroups;
        }

        /*
        /// <summary>
        ///     Figure out the groups that match the specified options and arguments.
        ///     <para/>
        ///     Only arguments and options that have those groups will be considered for further
        ///     processing.
        /// </summary>
        /// <param name="run">
        ///     The <see cref="ParseRun"/> instance that contains the specified options.
        /// </param>
        /// <param name="specifiedArguments">The specified arguments.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the <paramref name="specifiedArguments"/> is <c>null</c>.
        /// </exception>
        private IReadOnlyList<int> GetMatchingGroups(ParseRun run, IList<string> specifiedArguments)
        {
            // Get the option runs for only the specified options.
            IEnumerable<OptionRun> specifiedOptions = run.Options.Where(or => or.Occurrences > 0);

            // For the specified option runs and all the argument runs, get the distinct set of groups.
            IEnumerable<int> groups = specifiedOptions
                .SelectMany(or => or.Option.Groups)
                .Union(run.Arguments.SelectMany(ar => ar.Argument.Groups))
                .Distinct();

            // Find the common groups across all specified option runs.
            foreach (OptionRun or in specifiedOptions)
                groups = groups.Intersect(or.Option.Groups);

            // In case no groups remain, we have two possibilities.
            if (!groups.Any())
            {
                // Check if all options and arguments are optional.
                bool allArgsOptional = run.Options.All(or => or.Option.Usage.MinOccurrences == 0) && run.Arguments.All(ar => ar.Argument.IsOptional);

                // If all options and arguments are optional, then no args were specified for this
                // command, so simply return a default of group 0.
                if (allArgsOptional)
                    return new[] { 0 };

                // If at least one option or argument is required, then this is an invalid scenario and we should throw an exception.
                // Return an empty list to indicate an error.
                return new List<int>(0);
            }

            // For the remaining groups, check whether the number of specified arguments falls in the
            // range of the argument runs that have the group.
            // Add any matching groups to a list.
            var matchingArgumentGroups = new List<int>();
            foreach (int group in groups)
            {
                // Find all argument runs that have the group.
                IList<ArgumentRun> groupArguments = run.Arguments
                    .Where(ar => ar.Argument.Groups.Contains(group))
                    .ToList();

                // Calculate the minimum and maximum possible number of arguments that can be specified,
                // based on the selected argument groups.
                int minOccurences = groupArguments.Count(ar => !ar.Argument.IsOptional);
                int maxOccurences = groupArguments.Count == 0 ? 0
                    : groupArguments.Count + groupArguments[groupArguments.Count - 1].Argument.MaxOccurences - 1;

                // If the number of specified arguments falls into the calculated range, then this
                // group is valid, so add it to the list.
                if (specifiedArguments.Count >= minOccurences && specifiedArguments.Count <= maxOccurences)
                    matchingArgumentGroups.Add(group);
            }

            return matchingArgumentGroups;
        }*/

        private void TrimRunsToMatchingGroups(ParseRun run, IReadOnlyList<int> matchingGroups)
        {
            for (int i = run.Options.Count - 1; i >= 0; i--)
            {
                if (!run.Options[i].Option.Groups.Any(grp => matchingGroups.Contains(grp)))
                    run.Options.RemoveAt(i);
            }

            for (int i = run.Arguments.Count - 1; i >= 0; i--)
            {
                if (!run.Arguments[i].Argument.Groups.Any(grp => matchingGroups.Contains(grp)))
                    run.Arguments.RemoveAt(i);
            }
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

                    or.Value = GetDefaultValue(or);
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
                //TODO: Possible bug. Try specifying a flag option followed by an argument, and this fails. Somewhere, the argument is being considered a parameter of the argument.
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

                // If the option has been specified, try resolving its value.
                // If not and it has a default value, set that.
                or.Assigned = true;
                if (or.Occurrences > 0)
                    or.Value = ResolveOptionParameterValues(or);
                else if (or.Option.DefaultSetter != null)
                    or.Value = GetDefaultValue(or);
                else
                    or.Assigned = false;

                // Option values can only null for objects. For all other value types, assign a default
                // value.
                if (or.Value is null)
                {
                    or.Assigned = true;
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

            object GetDefaultValue(OptionRun run)
            {
                object defaultValue = run.Option.DefaultSetter();

                if (run.ValueType != OptionValueType.List)
                    return defaultValue;

                IList list = run.CreateCollection(1);
                list.Add(defaultValue);
                return list;
            }
        }

        /// <summary>
        ///     Resolves an <see cref="Option" />'s value based on it's usage details. See the
        ///     <see cref="Option.GetValueType" /> method for details on how the
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
                        list.Add(optionRun.ResolveValue(parameter));
                    return list;

                case OptionValueType.Object:
                    return optionRun.ResolveValue(optionRun.Parameters[0]);
            }

            throw new ParserException(-1, "Should never reach here");
        }

        /// <summary>
        ///     Process the specified arguments by verifying their usage, validating them and executing
        ///     their handlers.
        /// </summary>
        /// <param name="specifiedArguments">The list of specified arguments.</param>
        /// <param name="argumentRuns">The argument run details.</param>
        /// <exception cref="ParserException">
        ///     Thrown if any of the validation or usage checks on the <see cref="ArgumentRun"/> objects
        ///     fails.
        /// </exception>
        private static void ProcessArguments(IReadOnlyList<string> specifiedArguments, IReadOnlyList<ArgumentRun> argumentRuns)
        {
            // We already verified the number of specified arguments in the GetMatchingGroups method.

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
                    argumentRun.Value = i == argumentRuns.Count - 1 && argument.MaxOccurences > 1
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
                    if (i == argumentRuns.Count - 1 && argument.MaxOccurences > 1)
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
                return argumentRun.ResolveValue(specifiedArguments[startIndex]);

            IList list = argumentRun.CreateCollection(endIndex - startIndex + 1);
            for (var i = startIndex; i <= endIndex; i++)
                list.Add(argumentRun.ResolveValue(specifiedArguments[i]));
            return list;
        }
    }

    // Static methods
    public sealed partial class Parser
    {
        /// <summary>
        ///     Tokenizes the specified <paramref name="str"/> with space separators and &quot; delimiters.
        /// </summary>
        /// <param name="str">The string to tokenize.</param>
        /// <returns>The sequence of tokens.</returns>
        public static IEnumerable<string> Tokenize(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                yield break;

            Match match = TokenPattern.Match(str);

            if (!match.Success)
                yield break;

            foreach (RegexCapture capture in match.Groups["token"].Captures)
                yield return capture.Value;
        }

        private static readonly Regex TokenPattern = new Regex(@"((\s*""(?<token>[^""]*)(""|$)\s*)|(\s*(?<token>[^\s""]+)\s*))*",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    }
}
