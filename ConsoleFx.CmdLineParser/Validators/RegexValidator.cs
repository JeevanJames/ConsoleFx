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
using System.Text.RegularExpressions;

namespace ConsoleFx.CmdLineParser.Validators
{
    public class RegexValidator : SingleMessageValidator<string>
    {
        public RegexValidator(Regex regex) : base(Messages.Regex)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));
            Regex = regex;
        }

        public RegexValidator(string pattern) : base(Messages.Regex)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentException("Value cannot be null or empty.", nameof(pattern));
            Regex = new Regex(pattern);
        }

        public Regex Regex { get; }

        protected override string ValidateAsString(string parameterValue)
        {
            if (!Regex.IsMatch(parameterValue))
                ValidationFailed(parameterValue, Message);
            return parameterValue;
        }
    }
}