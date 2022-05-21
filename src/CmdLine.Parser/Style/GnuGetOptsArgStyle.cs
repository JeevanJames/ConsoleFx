// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using ConsoleFx.CmdLine.Parser.Runs;

namespace ConsoleFx.CmdLine.Parser.Style
{
    public sealed class GnuGetOptsArgStyle : ArgStyle
    {
        /// <inheritdoc/>
        public override ArgGrouping GetGrouping(ArgGrouping specifiedGrouping, IReadOnlyList<Option> options,
            IReadOnlyList<Argument> arguments)
        {
            // If any option has variable number of parameters (i.e. ExpectedParameters = null),
            // then the groupings is changed to DoesNotMatter.
            if (specifiedGrouping == ArgGrouping.OptionsBeforeArguments)
            {
                bool optionsHaveVariableParameters = options.Any(option => !option.Usage.ExpectedParameters.HasValue);
                if (optionsHaveVariableParameters)
                    specifiedGrouping = ArgGrouping.DoesNotMatter;
            }

            // If any option has unlimited parameters, then options must appear after arguments.
            bool optionsHaveUnlimitedParameters =
                options.Any(option => option.Usage.MaxParameters == OptionUsage.Unlimited);
            if (specifiedGrouping != ArgGrouping.OptionsAfterArguments && optionsHaveUnlimitedParameters)
                specifiedGrouping = ArgGrouping.OptionsAfterArguments;

            return specifiedGrouping;
        }

        //TODO: Use explicit capture
        private static readonly Regex OptionPattern = new(@"^(--?)(\w[\w-_]*)(?:=(.+))?$", RegexOptions.Compiled,
            TimeSpan.FromSeconds(1));

        /// <inheritdoc/>
        public override IEnumerable<string> IdentifyTokens(IEnumerable<string> tokens, IReadOnlyList<OptionRun> options,
            ArgGrouping grouping)
        {
            // Tracks whether we're currently processing an option. Depending on the option's usage
            // rules, one or more subsequent token may be considered as option parameters.
            OptionRun currentOption = null;
            int currentOptionParameterCount = 0;

            bool forceArguments = false;

            foreach (string token in tokens)
            {
                // A standalone -- token indicates that all subsequent tokens are to be considered
                // arguments even if they look like options.
                if (token == "--")
                {
                    forceArguments = true;
                    continue;
                }

                if (forceArguments)
                {
                    yield return token;
                    continue;
                }

                Match optionMatch = OptionPattern.Match(token);

                // If the token is not an option and we are not iterating over the parameters of an
                // option, then the token is an argument.
                if (!optionMatch.Success)
                {
                    // If we're processing an option (currentOption != null), then the token is a
                    // parameter on the option.
                    // Otherwise, the token is considered to be an argument.
                    if (currentOption is null)
                        yield return token;
                    else
                    {
                        currentOption.Parameters.Add(token);
                        currentOptionParameterCount++;
                    }
                }
                else
                {
                    bool isShortOption = optionMatch.Groups[1].Value.Length == 1;
                    string optionName = optionMatch.Groups[2].Value;
                    string parameterValue = optionMatch.Groups[3].Value;
                    bool isParameterSpecified = !string.IsNullOrEmpty(parameterValue);

                    // If multiple short options are specified as a single combined option, then none
                    // of them can have parameters.
                    if (isShortOption && optionName.Length > 1 && isParameterSpecified)
                        throw new ParserException(-1, $"Cannot specify a parameter for the combined option {optionName}.");

                    // Get the specified option names.
                    // If a short option name is specified and it has multiple characters, each
                    // character is a short name.
                    string[] optionNames = isShortOption && optionName.Length > 1
                        ? optionName.Select(c => c.ToString()).ToArray()
                        : new[] { optionName };

                    // Store the first option's run, as we may need to process its parameters later.
                    OptionRun firstOptionRun = null;

                    // Add each option to its corresponding option run.
                    foreach (string name in optionNames)
                    {
                        OptionRun matchingOption = options.SingleOrDefault(or => or.Option.HasName(name));
                        if (matchingOption is null)
                        {
                            throw new ParserException(ParserException.Codes.InvalidOptionSpecified,
                                string.Format(Messages.InvalidOptionSpecified, optionName));
                        }

                        // Have we reached the maximum number of occurrences of this option? If so,
                        // we can't allow this occurrence.
                        if (matchingOption.Occurrences >= matchingOption.Option.Usage.MaxOccurrences)
                        {
                            throw new ParserException(-1,
                                $"Maximum number of occurrences of option {optionName} have been specified.");
                        }

                        // Increase the number of occurrences of the option
                        matchingOption.Occurrences++;

                        firstOptionRun ??= matchingOption;
                    }

                    // If only one option was specified (and not a combined short option set), we deal
                    // with the parameters now.
                    if (optionNames.Length == 1 && firstOptionRun is not null)
                    {
                        if (isParameterSpecified)
                        {
                            // If the parameter was specified in the same token with a "=" symbol, then
                            // that is the only parameter we can allow.
                            // Add it and proceed with the next token.
                            firstOptionRun.Parameters.Add(parameterValue);
                            currentOption = null;
                            currentOptionParameterCount = 0;
                        }
                        else
                        {
                            // Otherwise, mark the current option as this one and process all subsequent
                            // tokens as parameters until we encounter another option or we've reached the
                            // max limit of parameters for this option.
                            currentOption = firstOptionRun;
                            currentOptionParameterCount = 0;
                        }
                    }
                    else
                    {
                        currentOption = null;
                        currentOptionParameterCount = 0;
                    }
                }

                // If we're on an option (currentOption != null) and the number of parameters has
                // reached the maximum allowed, then we can stop handling that option by setting
                // currentOption to null so that the next arg will be treated as a new option or
                // argument.
                if (currentOption is not null)
                {
                    // Have we reached or exceeded the maximum number of parameters for this occurrence
                    // of the option or the maximum number of parameters across all occurrences of
                    // the option?
                    int maxParameters = currentOption.Option.Usage.MaxOccurrences * currentOption.Option.Usage.MaxParameters;
                    if (currentOptionParameterCount >= currentOption.Option.Usage.MaxParameters
                        || currentOption.Parameters.Count >= maxParameters)
                    {
                        currentOption = null;
                        currentOptionParameterCount = 0;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<string> GetDefaultHelpOptionNames()
        {
            yield return "help";
            yield return "h";
        }
    }
}
