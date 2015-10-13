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
        public override CommandGrouping GetGrouping(CommandGrouping specifiedGrouping)
        {
            return base.GetGrouping(specifiedGrouping);
        }

        private static readonly Regex OptionPattern = new Regex(@"(--?)(\w+)");

        public override IEnumerable<string> IdentifyTokens(IEnumerable<string> args, Options options, Behaviors behaviors)
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

                    currentOption = option;
                    option.Run.Occurences += 1;
                } else
                    currentOption.Run.Parameters.Add(arg);
            }
        }
    }
}
