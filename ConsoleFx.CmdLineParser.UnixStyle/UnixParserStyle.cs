﻿#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015-2018 Jeevan James

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
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleFx.CmdLineParser.UnixStyle
{
    public sealed class UnixParserStyle : ParserStyle
    {
        /// <summary>
        ///     <para>
        ///         Single character short names allow certain behaviors like combining multiple parameterless options
        ///         into a single option.
        ///     </para>
        ///     <para>For example, "-w -v -d" can be specified as "-wvd"</para>
        ///     <para>If this is true, then it will be validated in the <see cref="ValidateDefinedOptions"/> method.</para>
        /// </summary>
        public bool EnforceSingleCharacterShortNames { get; set; } = true;

        /// <inheritdoc/>
        public override ArgGrouping GetGrouping(ArgGrouping specifiedGrouping, IReadOnlyList<Option> options,
            IReadOnlyList<Argument> arguments)
        {
            //If any option has variable number of parameters (i.e. ExpectedParameters = null),
            //then the groupings is changed to DoesNotMatter.
            if (specifiedGrouping == ArgGrouping.OptionsBeforeArguments)
            {
                bool optionsHaveVariableParameters = options.Any(option => !option.Usage.ExpectedParameters.HasValue);
                if (optionsHaveVariableParameters)
                    specifiedGrouping = ArgGrouping.DoesNotMatter;
            }

            //If any option has unlimited parameters, then options must appear after arguments.
            bool optionsHaveUnlimitedParameters =
                options.Any(option => option.Usage.MaxParameters == OptionUsage.Unlimited);
            if (specifiedGrouping != ArgGrouping.OptionsAfterArguments && optionsHaveUnlimitedParameters)
                specifiedGrouping = ArgGrouping.OptionsAfterArguments;

            return specifiedGrouping;
        }

        //public override void ValidateDefinedOptions(IEnumerable<Option> options)
        //{
        //    if (EnforceSingleCharacterShortNames)
        //    {
        //        Option invalidOption = options.FirstOrDefault(option => option.ShortName?.Length != 1);
        //        if (invalidOption != null)
        //            throw new ParserException(1000, $"Option '{invalidOption.Name}' has an invalid short name. Short names for the UNIX style parser should be a single character only. Alternatively, set the {nameof(EnforceSingleCharacterShortNames)} property to force to bypass this check and allow multi-character short names.");
        //    }
        //}

        private static readonly Regex OptionPattern = new Regex(@"(--?)(\w[\w-_]+)(?:=(.+))?");

        /// <inheritdoc/>
        public override IEnumerable<string> IdentifyTokens(IEnumerable<string> tokens, IReadOnlyList<OptionRun> options,
            ArgGrouping grouping)
        {
            OptionRun currentOption = null;

            foreach (string token in tokens)
            {
                Match optionMatch = OptionPattern.Match(token);

                //If the token is not an option and we are not iterating over the parameters of an
                //option, then the token is an argument.
                if (!optionMatch.Success && currentOption == null)
                {
                    yield return token;
                    continue;
                }

                if (optionMatch.Success)
                {
                    bool isShortOption = optionMatch.Groups[1].Value.Length == 1;
                    string optionName = optionMatch.Groups[2].Value;
                    string parameterValue = optionMatch.Groups[3].Value;
                    bool isParameterSpecified = !string.IsNullOrEmpty(parameterValue);

                    Func<OptionRun, bool> predicate = isShortOption
                        ? (Func<OptionRun, bool>)
                            (or => or.Option.ShortName != null && or.Option.ShortName.Equals(optionName, StringComparison.OrdinalIgnoreCase))
                        : or => or.Option.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase);
                    OptionRun option = options.FirstOrDefault(predicate);
                    if (option == null)
                    {
                        throw new ParserException(ParserException.Codes.InvalidOptionSpecified,
                            string.Format(Messages.InvalidOptionSpecified, optionName));
                    }

                    if (option.Option.CaseSensitive)
                    {
                        if (isShortOption && !option.Option.ShortName.Equals(optionName, StringComparison.Ordinal))
                            throw new ParserException(ParserException.Codes.InvalidOptionSpecified,
                                string.Format(Messages.InvalidOptionSpecified, optionName));
                        if (!option.Option.Name.Equals(optionName, StringComparison.Ordinal))
                            throw new ParserException(ParserException.Codes.InvalidOptionSpecified,
                                string.Format(Messages.InvalidOptionSpecified, optionName));
                    }

                    option.Occurences += 1;
                    if (isParameterSpecified)
                    {
                        option.Parameters.Add(parameterValue);
                        currentOption = null;
                    }
                    else
                        currentOption = option;
                }
                else
                    currentOption.Parameters.Add(token);

                //If we're on an option (currentOption != null) and the number of parameters has
                //reached the maximum allowed, then we can stop handling that option by setting
                //currentOption to null so that the next arg  will be treated as a new option or
                //argument.
                if (currentOption != null && currentOption.Parameters.Count > currentOption.Option.Usage.MaxParameters)
                    currentOption = null;
            }
        }
    }
}
