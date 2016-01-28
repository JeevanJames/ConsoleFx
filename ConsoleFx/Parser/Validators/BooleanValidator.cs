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

namespace ConsoleFx.Parser.Validators
{
    /// <summary>
    /// Checks whether the parameter value is 'True' or 'False'. The check is not case sensitive.
    /// </summary>
    public class BooleanValidator : Validator<string>
    {
        private readonly List<string> _trueStrings = new List<string>();
        private readonly List<string> _falseStrings = new List<string>();
        private readonly StringComparison _comparison;

        public BooleanValidator(string trueString = "true", string falseString = "false", bool caseSensitive = false)
        {
            _trueStrings.Add(trueString);
            _falseStrings.Add(falseString);
            _comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        public BooleanValidator(IEnumerable<string> trueStrings, IEnumerable<string> falseStrings, bool caseSensitive = false)
        {
            _trueStrings.AddRange(trueStrings);
            _falseStrings.AddRange(falseStrings);
            _comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        public string Message { get; set; } = Messages.Boolean;

        protected override string PrimaryChecks(string parameterValue)
        {
            if (!_trueStrings.Any(str => str.Equals(parameterValue, _comparison)) && !_falseStrings.Any(str => str.Equals(parameterValue, _comparison)))
                ValidationFailed(parameterValue, Message);
            return parameterValue;
        }
    }
}