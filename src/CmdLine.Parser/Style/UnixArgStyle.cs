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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using ConsoleFx.CmdLine.Parser.Runs;

namespace ConsoleFx.CmdLine.Parser.Style
{
    public sealed class UnixArgStyle : ArgStyle
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

        private static readonly Regex OptionPattern = new Regex(@"^(--?)(\w[\w-_]*)(?:=(.+))?$");

        /// <inheritdoc/>
        public override IEnumerable<string> IdentifyTokens(IEnumerable<string> tokens, IReadOnlyList<OptionRun> options,
            ArgGrouping grouping)
        {
            OptionRun currentOption = null;

            bool forceArguments = false;

            foreach (string token in tokens)
            {
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
                    // If we're processing an option (currentOption != null), then it's an argument.
                    // Otherwise, it's a parameter on the current option.
                    if (currentOption is null)
                        yield return token;
                    else
                        currentOption.Parameters.Add(token);
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

                    // Add each option to its corresponding option run.
                    foreach (string name in optionNames)
                    {
                        OptionRun matchingOption = options.SingleOrDefault(or => or.Option.HasName(name));
                        if (matchingOption is null)
                        {
                            throw new ParserException(ParserException.Codes.InvalidOptionSpecified,
                                string.Format(Messages.InvalidOptionSpecified, optionName));
                        }

                        // Increase the number of occurrences of the option
                        matchingOption.Occurrences += 1;
                    }

                    // If only one option was specified (and not a combined short option set), we deal
                    // with the parameters now.
                    if (optionNames.Length == 1)
                    {
                        // We know its option run exists, so use Single here.
                        OptionRun matchingOption = options.Single(or => or.Option.HasName(optionNames[0]));

                        // If the parameter was specified in the same token with a "=" symbol, then
                        // that is the only parameter we can allow.
                        // Add it and proceed with the next token.
                        if (isParameterSpecified)
                        {
                            matchingOption.Parameters.Add(parameterValue);
                            currentOption = null;
                        }

                        // Otherwise, mark the current option as this one and process all subsequent
                        // tokens as parameters until we encounter another option or we've reached the
                        // max limit of parameters for this option.
                        else
                            currentOption = matchingOption;
                    }
                    else
                        currentOption = null;
                }

                // If we're on an option (currentOption != null) and the number of parameters has
                // reached the maximum allowed, then we can stop handling that option by setting
                // currentOption to null so that the next arg will be treated as a new option or
                // argument.
                if (currentOption != null && currentOption.Parameters.Count >= currentOption.Option.Usage.MaxParameters)
                    currentOption = null;
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
