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
using System.Text.RegularExpressions;

namespace ConsoleFx.Parsers.Validators
{
    public sealed class RegexValidator : SingleMessageValidator
    {
        public RegexValidator(Regex regex)
            : base(Messages.Regex)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));
            Regex = regex;
        }

        public RegexValidator(string pattern)
            : base(Messages.Regex)
        {
            Regex = new Regex(pattern);
        }

        public override void Validate(string parameterValue)
        {
            if (!Regex.IsMatch(parameterValue))
                ValidationFailed(parameterValue);
        }

        public Regex Regex { get; }
    }

    public static class RegexPattern
    {
        public const string Email = @"^[^_][a-zA-Z0-9_]+[^_]@{1}[a-z]+[.]{1}(([a-z]{2,3})|([a-z]{2,3}[.]{1}[a-z]{2,3}))$";
        public const string FileMask = @"^[\w\.\*\?][\w\s\.\*\?]*$";
        public const string Url = @"";

        private static string _path;

        public static string Path
        {
            get { return _path ?? (_path = BuildPathRegex()); }
        }

        private static string BuildPathRegex()
        {
            //TODO:
            return string.Empty;
        }
    }
}
