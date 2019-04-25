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

using System;
using System.Collections.Generic;
using System.Linq;

using ConsoleFx.CmdLineArgs.Validators.Bases;

namespace ConsoleFx.CmdLineArgs.Validators
{
    /// <summary>
    ///     Checks whether the parameter value is 'True' or 'False'. The check is not case sensitive.
    /// </summary>
    public class BooleanValidator : SingleMessageValidator<bool>
    {
        private readonly List<string> _trueStrings;
        private readonly List<string> _falseStrings;
        private readonly StringComparison _comparison;

        public BooleanValidator(string trueString = "true", string falseString = "false", bool caseSensitive = false)
            : base(Messages.Boolean)
        {
            _trueStrings = new List<string>(1)
            {
                trueString
            };

            _falseStrings = new List<string>(1)
            {
                falseString
            };

            _comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        public BooleanValidator(IEnumerable<string> trueStrings, IEnumerable<string> falseStrings, bool caseSensitive = false)
            : base(Messages.Boolean)
        {
            if (trueStrings == null)
                throw new ArgumentNullException(nameof(trueStrings));
            if (falseStrings == null)
                throw new ArgumentNullException(nameof(falseStrings));

            _trueStrings = new List<string>(trueStrings);
            _falseStrings = new List<string>(falseStrings);
            _comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        protected override bool ValidateAsString(string parameterValue)
        {
            bool isTrue = _trueStrings.Any(str => str.Equals(parameterValue, _comparison));
            if (isTrue)
                return true;
            bool isFalse = _falseStrings.Any(str => str.Equals(parameterValue, _comparison));
            if (isFalse)
                return false;
            ValidationFailed(parameterValue, Message);
            return false;
        }
    }

    public static class BooleanValidatorExtensions
    {
        public static Argument ValidateAsBoolean(this Argument argument, string trueString = "true",
            string falseString = "false", bool caseSensitive = false, string message = null)
        {
            var validator = new BooleanValidator(trueString, falseString, caseSensitive);
            if (message != null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Argument ValidateAsBoolean(this Argument argument, IEnumerable<string> trueStrings,
            IEnumerable<string> falseStrings, bool caseSensitive = false, string message = null)
        {
            var validator = new BooleanValidator(trueStrings, falseStrings, caseSensitive);
            if (message != null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Option ValidateAsBoolean(this Option option, string trueString = "true",
            string falseString = "false", bool caseSensitive = false, int parameterIndex = -1, string message = null)
        {
            var validator = new BooleanValidator(trueString, falseString, caseSensitive);
            if (message != null)
                validator.Message = message;
            return option.ValidateWith(parameterIndex, validator);
        }

        public static Option ValidateAsBoolean(this Option option, IEnumerable<string> trueStrings,
            IEnumerable<string> falseStrings, bool caseSensitive = false, int parameterIndex = -1,
            string message = null)
        {
            var validator = new BooleanValidator(trueStrings, falseStrings, caseSensitive);
            if (message != null)
                validator.Message = message;
            return option.ValidateWith(parameterIndex, validator);
        }
    }
}
