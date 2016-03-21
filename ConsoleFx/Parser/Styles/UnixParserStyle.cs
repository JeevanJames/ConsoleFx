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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleFx.Parser.Styles
{
    public sealed class UnixParserStyle : ParserStyle
    {
        public override ArgGrouping GetGrouping(ArgGrouping specifiedGrouping, Options options, IList<Argument> arguments)
        {
            //Options before arguments will not work if there are any options that do not have a
            //fixed number of parameters (i.e. ExpectedParameters == null). The groupings gets
            //changed to DoesNotMatter.
            bool optionsHaveVariableParameters = options.Any(option => !option.Usage.ExpectedParameters.HasValue);
            if (specifiedGrouping == ArgGrouping.OptionsBeforeArguments && optionsHaveVariableParameters)
                specifiedGrouping = ArgGrouping.DoesNotMatter;

            //If DoesNotMatter or OptionsBeforeArguments and any option has unlimited parameters,
            //change to OptionsAfterArguments.
            bool optionsHaveUnlimitedParameters = options.Any(option => option.Usage.MaxParameters == int.MaxValue);
            if (specifiedGrouping != ArgGrouping.OptionsAfterArguments && optionsHaveUnlimitedParameters)
                specifiedGrouping = ArgGrouping.OptionsAfterArguments;

            return specifiedGrouping;
        }

        public override void ValidateDefinedOptions(Options options)
        {
            Option invalidOption = options.FirstOrDefault(option => !string.IsNullOrEmpty(option.ShortName) && option.ShortName.Length > 1);
            if (invalidOption != null)
                throw new ParserException(1000, $"Option '{invalidOption.Name}' has an invalid short name. Short names for the UNIX style parser should be a single character only.");
        }

        private static readonly Regex OptionPattern = new Regex(@"(--?)(\w+)(?:=(.+))?");

        public override IEnumerable<string> IdentifyTokens(IEnumerable<string> args, Options options, ArgGrouping grouping, object scope)
        {
            Option currentOption = null;

            foreach (string arg in args)
            {
                Match optionMatch = OptionPattern.Match(arg);

                if (!optionMatch.Success && currentOption == null)
                {
                    yield return arg;
                    continue;
                }

                if (optionMatch.Success)
                {
                    bool isShortOption = optionMatch.Groups[1].Value.Length == 1;
                    string optionName = optionMatch.Groups[2].Value;
                    string parameterValue = optionMatch.Groups[3].Value;
                    bool isParameterSpecified = !string.IsNullOrEmpty(parameterValue);

                    Func<Option, bool> predicate = isShortOption ? (Func<Option, bool>)(opt => opt.ShortName.Equals(optionName, StringComparison.OrdinalIgnoreCase)) : opt => opt.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase);
                    Option option = options.FirstOrDefault(predicate);
                    if (option == null)
                        throw new ParserException(ParserException.Codes.InvalidOptionSpecified, string.Format(Messages.InvalidOptionSpecified, optionName));

                    if (option.CaseSensitive)
                    {
                        if (isShortOption && !option.ShortName.Equals(optionName, StringComparison.Ordinal))
                            throw new ParserException(ParserException.Codes.InvalidOptionSpecified, string.Format(Messages.InvalidOptionSpecified, optionName));
                        if (!option.Name.Equals(optionName, StringComparison.Ordinal))
                            throw new ParserException(ParserException.Codes.InvalidOptionSpecified, string.Format(Messages.InvalidOptionSpecified, optionName));
                    }

                    option.Run.Occurences += 1;
                    if (isParameterSpecified)
                    {
                        option.Run.Parameters.Add(parameterValue);
                        currentOption = null;
                    }
                    else
                        currentOption = option;
                } else
                    currentOption.Run.Parameters.Add(arg);

                //If we're on an option (currentOption != null) and the number of parameters has
                //reached the maximum allowed, then we can stop handling that option by setting
                //currentOption to null so that the next arg  will be treated as a new option or
                //argument.
                if (currentOption != null && currentOption.Run.Parameters.Count > currentOption.Usage.MaxParameters)
                    currentOption = null;
            }
        }
    }
}
