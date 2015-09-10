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

using ConsoleFx.Parsers;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConsoleFx.Parser.Styles
{
    public sealed class WindowsParserStyle : ParserStyle
    {
        private static readonly Regex OptionPattern = new Regex(@"^[\-\/]([\w\?]+)");
        private static readonly Regex OptionParameterPattern = new Regex(@"([\s\S\w][^,]*)");

        public override SpecifiedValues IdentifyTokens(IEnumerable<string> tokens, Arguments arguments, Options options, Behaviors behaviors)
        {
            var values = new SpecifiedValues();

            ArgumentType previousType = ArgumentType.NotSet;
            ArgumentType currentType = ArgumentType.NotSet;

            foreach (string token in tokens)
            {
                VerifyCommandLineGrouping(previousType, currentType, behaviors.Grouping);

                previousType = currentType;

                Match optionMatch = OptionPattern.Match(token);
                if (!optionMatch.Success)
                {
                    currentType = ArgumentType.Argument;
                    values.Arguments.Add(token);
                }
                else
                {
                    currentType = ArgumentType.Option;

                    string specifiedOptionName = optionMatch.Groups[1].Value;

                    Option availableOption = options[specifiedOptionName];
                    if (availableOption == null)
                        throw new ParserException(ParserException.Codes.InvalidOptionSpecified, Messages.InvalidOptionSpecified, specifiedOptionName);

                    SpecifiedOptionParameters parameters;
                    if (!values.Options.TryGetValue(availableOption.Name, out parameters))
                    {
                        parameters = new SpecifiedOptionParameters();
                        values.Options.Add(availableOption.Name, parameters);
                    }

                    //If no switch parameters are specified, stop processing
                    if (token.Length == specifiedOptionName.Length + 1)
                        continue;

                    if (token[specifiedOptionName.Length + 1] != ':')
                        throw new ParserException(ParserException.Codes.InvalidOptionParameterSpecifier, Messages.InvalidOptionParameterSpecifier, specifiedOptionName);

                    MatchCollection parameterMatches = OptionParameterPattern.Matches(token, optionMatch.Length + 1);
                    foreach (Match parameterMatch in parameterMatches)
                    {
                        string value = parameterMatch.Groups[1].Value;
                        if (value.StartsWith(",", StringComparison.OrdinalIgnoreCase))
                            value = value.Remove(0, 1);
                        parameters.Add(value);
                    }
                }
            }

            VerifyCommandLineGrouping(previousType, currentType, behaviors.Grouping);

            return values;
        }

        //This method is used by the code that validates the command-line grouping. It is
        //called for every iteration of the arguments
        private void VerifyCommandLineGrouping(ArgumentType previousType, ArgumentType currentType, CommandGrouping grouping)
        {
            if (grouping == CommandGrouping.DoesNotMatter)
                return;

            if (previousType == ArgumentType.NotSet || currentType == ArgumentType.NotSet)
                return;

            if (grouping == CommandGrouping.OptionsAfterArguments && previousType == ArgumentType.Option &&
                currentType == ArgumentType.Argument)
                throw new ParserException(ParserException.Codes.OptionsAfterParameters, Messages.OptionsAfterParameters);
            if (grouping == CommandGrouping.OptionsBeforeArguments && previousType == ArgumentType.Argument &&
                currentType == ArgumentType.Option)
                throw new ParserException(ParserException.Codes.OptionsBeforeParameters, Messages.OptionsBeforeParameters);
        }

        private enum ArgumentType
        {
            NotSet,
            Option,
            Argument
        }
    }
}
