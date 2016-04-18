using System;
using System.Linq;

namespace ConsoleFx.Parser.Validators
{
    public class CompositeValidator : SingleMessageValidator<string>
    {
        private readonly Validator[] _validators;

        public CompositeValidator(string errorMessage, params Validator[] validators) : base(errorMessage)
        {
            if (validators == null)
                throw new ArgumentNullException(nameof(validators));
            if (validators.Length < 2)
                throw new ArgumentException("Need to specify at least two validators for a composite validator.", nameof(validators));
            _validators = validators;
        }

        /// <summary>
        ///     Validates the parameter value as a string. Converts to the actual type if the validation succeeds and returns that
        ///     value. This method must be overridden in derived classes.
        /// </summary>
        /// <param name="parameterValue">The parameter value as a string</param>
        /// <returns>The parameter value converted to its actual type.</returns>
        /// <exception cref="ValidationException">Thrown if the validation fails.</exception>
        protected override string ValidateAsString(string parameterValue)
        {
            bool anyValidatorPassed = _validators.Any(validator => {
                try
                {
                    validator.Validate(parameterValue);
                    return true;
                }
                catch (ValidationException)
                {
                }
                return false;
            });
            if (!anyValidatorPassed)
                ValidationFailed(Message, parameterValue);
            return parameterValue;
        }
    }
}