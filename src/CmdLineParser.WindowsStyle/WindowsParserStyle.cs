#region --- License & Copyright Notice ---

/*
ConsoleFx CLI Library Suite
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

namespace ConsoleFx.CmdLineParser.WindowsStyle
{
    public sealed class WindowsParserStyle : ParserStyle
    {
        // -Option or /Option
        private static readonly Regex OptionPattern = new Regex(@"^[\-\/]([\w\?][\w-_\?]*)");
        private static readonly Regex OptionParameterPattern = new Regex(@"([\s\S\w][^,]*)");

        public override IEnumerable<string> IdentifyTokens(IEnumerable<string> tokens, IReadOnlyList<OptionRun> options,
            ArgGrouping grouping)
        {
            var previousType = ArgumentType.NotSet;
            var currentType = ArgumentType.NotSet;

            foreach (string token in tokens)
            {
                VerifyCommandLineGrouping(previousType, currentType, grouping);

                previousType = currentType;

                var optionMatch = OptionPattern.Match(token);
                if (!optionMatch.Success)
                {
                    currentType = ArgumentType.Argument;
                    yield return token;
                }
                else
                {
                    currentType = ArgumentType.Option;

                    string specifiedOptionName = optionMatch.Groups[groupnum: 1].Value;

                    var availableOption = options.FirstOrDefault(or => or.Option.HasName(specifiedOptionName));
                    if (availableOption == null)
                    {
                        throw new ParserException(ParserException.Codes.InvalidOptionSpecified,
                            string.Format(Messages.InvalidOptionSpecified, specifiedOptionName));
                    }

                    availableOption.Occurrences += 1;

                    //If no switch parameters are specified, stop processing
                    if (token.Length == specifiedOptionName.Length + 1)
                        continue;

                    if (token[specifiedOptionName.Length + 1] != ':')
                    {
                        throw new ParserException(ParserException.Codes.InvalidOptionParameterSpecifier,
                            string.Format(Messages.InvalidOptionParameterSpecifier, specifiedOptionName));
                    }

                    var parameterMatches = OptionParameterPattern.Matches(token, optionMatch.Length + 1);
                    foreach (Match parameterMatch in parameterMatches)
                    {
                        string value = parameterMatch.Groups[groupnum: 1].Value;
                        if (value.StartsWith(",", StringComparison.OrdinalIgnoreCase))
                            value = value.Remove(startIndex: 0, count: 1);
                        availableOption.Parameters.Add(value);
                    }
                }
            }

            VerifyCommandLineGrouping(previousType, currentType, grouping);
        }

        /// <summary>
        ///     This method is used by the code that validates the command-line grouping. It is
        ///     called for every iteration of the arguments.
        /// </summary>
        /// <param name="previousType">The type of the previously-checked argument.</param>
        /// <param name="currentType">The type of the currently-checked argument.</param>
        /// <param name="grouping">The expected arg grouping.</param>
        private static void VerifyCommandLineGrouping(ArgumentType previousType, ArgumentType currentType,
            ArgGrouping grouping)
        {
            if (grouping == ArgGrouping.DoesNotMatter)
                return;

            if (previousType == ArgumentType.NotSet || currentType == ArgumentType.NotSet)
                return;

            if (grouping == ArgGrouping.OptionsAfterArguments && previousType == ArgumentType.Option &&
                currentType == ArgumentType.Argument)
            {
                throw new ParserException(ParserException.Codes.OptionsAfterParameters,
                    Messages.OptionsAfterParameters);
            }

            if (grouping == ArgGrouping.OptionsBeforeArguments && previousType == ArgumentType.Argument &&
                currentType == ArgumentType.Option)
            {
                throw new ParserException(ParserException.Codes.OptionsBeforeParameters,
                    Messages.OptionsBeforeParameters);
            }
        }

        private enum ArgumentType
        {
            NotSet,
            Option,
            Argument
        }
    }
}
