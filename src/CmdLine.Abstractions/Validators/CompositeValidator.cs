// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    /// <summary>
    ///     Checks if any one of the specified validators passes.
    /// </summary>
    public class CompositeValidator : SingleMessageValidator
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
        /// <param name="parameterValue">The parameter value as a string.</param>
        /// <returns>The parameter value converted to its actual type.</returns>
        /// <exception cref="ValidationException">Thrown if the validation fails.</exception>
        protected override object ValidateAsString(string parameterValue)
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
