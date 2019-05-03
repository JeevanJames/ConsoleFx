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
using System.Diagnostics;
using System.Linq;

using ConsoleFx.CmdLineArgs.Validators.Bases;

namespace ConsoleFx.CmdLineArgs.Validators
{
    /// <summary>
    ///     Checks if any one of the specified validators passes.
    /// </summary>
    public class CompositeValidator : SingleMessageValidator<string>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Validator[] _validators;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="errorMessage">The validation failure message.</param>
        /// <param name="validators">Two or more validators that form the composite validator.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="validators"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if less than 2 validators are specified.</exception>
        public CompositeValidator(string errorMessage, params Validator[] validators)
            : base(errorMessage)
        {
            if (validators is null)
                throw new ArgumentNullException(nameof(validators));
            if (validators.Length < 2)
                throw new ArgumentException("Specify at least two validators for a composite validator.", nameof(validators));
            _validators = validators;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Validates the parameter value as a string. Converts to the actual type if the validation succeeds and returns that
        ///     value. This method must be overridden in derived classes.
        /// </summary>
        /// <param name="parameterValue">The parameter value as a string</param>
        /// <returns>The parameter value converted to its actual type.</returns>
        /// <exception cref="T:ConsoleFx.CmdLineArgs.Validators.ValidationException">Thrown if the validation fails.</exception>
        protected override string ValidateAsString(string parameterValue)
        {
            bool anyValidatorPassed = _validators.Any(validator =>
            {
                try
                {
                    validator.Validate(parameterValue);
                    return true;
                }
                catch (ValidationException)
                {
                    return false;
                }
            });
            if (!anyValidatorPassed)
                ValidationFailed(Message, parameterValue);
            return parameterValue;
        }
    }

    public static class CompositeValidatorExtensions
    {
        public static Argument ValidateAnyCondition(this Argument argument, string errorMessage,
            params Validator[] validators) =>
            argument.ValidateWith(new CompositeValidator(errorMessage, validators));

        public static Option ValidateAnyCondition(this Option option, string errorMessage,
            params Validator[] validators) =>
            option.ValidateWith(new CompositeValidator(errorMessage, validators));

        public static Option ValidateAnyCondition(this Option option, int parameterIndex, string errorMessage,
            params Validator[] validators) =>
            option.ValidateWith(parameterIndex, new CompositeValidator(errorMessage, validators));
    }
}
