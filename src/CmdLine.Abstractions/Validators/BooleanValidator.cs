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

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    /// <summary>
    ///     Checks whether the parameter value is 'True' or 'False'. The check is not case sensitive.
    /// </summary>
    public class BooleanValidator : SingleMessageValidator
    {
        private readonly List<string> _trueStrings;
        private readonly List<string> _falseStrings;
        private readonly StringComparison _comparison;

        public BooleanValidator(string message)
            : base(typeof(bool), message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BooleanValidator"/> class.
        /// </summary>
        /// <param name="trueString">
        ///     The string that represents a true value. Defaults to 'true'.
        /// </param>
        /// <param name="falseString">
        ///     The string that represents a false value. Defaults to 'false'.
        /// </param>
        /// <param name="caseSensitive">
        ///     Indicates whether the validation checks are case sensitive.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if either the <paramref name="trueString"/> or <paramref name="falseString"/> is
        ///     <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if either thr <paramref name="trueString"/> or <paramref name="falseString"/>
        ///     is invalid, or both are the same based on the case sensitivity rules.
        /// </exception>
        public BooleanValidator(string trueString = "true", string falseString = "false", bool caseSensitive = false)
            : base(typeof(bool), Messages.Boolean)
        {
            if (trueString is null)
                throw new ArgumentNullException(nameof(trueString));
            if (falseString is null)
                throw new ArgumentNullException(nameof(falseString));
            if (trueString.Trim().Length == 0)
                throw new ArgumentException("Specify a valid true string.", nameof(trueString));
            if (falseString.Trim().Length == 0)
                throw new ArgumentException("Specify a valid false string.", nameof(falseString));

            _comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            if (string.Equals(trueString, falseString, _comparison))
                throw new ArgumentException("true string and false string cannot be the same");

            _trueStrings = new List<string>(1)
            {
                trueString,
            };

            _falseStrings = new List<string>(1)
            {
                falseString,
            };
        }

        public BooleanValidator(IEnumerable<string> trueStrings, IEnumerable<string> falseStrings, bool caseSensitive = false)
            : base(typeof(bool), Messages.Boolean)
        {
            if (trueStrings is null)
                throw new ArgumentNullException(nameof(trueStrings));
            if (falseStrings is null)
                throw new ArgumentNullException(nameof(falseStrings));
            if (!trueStrings.Any())
                throw new ArgumentException("Must specify at least one true string", nameof(trueStrings));
            if (!falseStrings.Any())
                throw new ArgumentException("Must specify at least one false string", nameof(falseStrings));
            if (trueStrings.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("The true strings contains one or more invalid values.", nameof(trueStrings));
            if (falseStrings.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("The false strings contains one or more invalid values.", nameof(falseStrings));

            IEnumerable<string> commonStrings = trueStrings.Intersect(falseStrings,
                caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
            if (commonStrings.Any())
                throw new ArgumentException($"The string '{commonStrings.First()}' is specified as both a true string and a false string.");

            _trueStrings = new List<string>(trueStrings);
            _falseStrings = new List<string>(falseStrings);
            _comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        protected override object ValidateAsString(string parameterValue)
        {
            bool isTrue = _trueStrings.Any(str => str.Equals(parameterValue, _comparison));
            if (isTrue)
                return true;
            bool isFalse = _falseStrings.Any(str => str.Equals(parameterValue, _comparison));
            if (isFalse)
                return false;
            ValidationFailed(Message, parameterValue);
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
