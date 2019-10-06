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
using System.Globalization;
using System.Linq;

namespace ConsoleFx.CmdLine.Validators.Bases
{
    /// <summary>
    ///     Base class for all validators.
    /// </summary>
    //TODO: The conversion performed by this class seems redundant considering the type conversion
    //details specified by the option itself (using its Type and TypeConverter properties). Can we
    //perform the conversion once and then pass the converted value into this class for the typed
    //validation. Also, consider that the conversion logic for validators and that for the option
    //could be different, leading to inconsistencies.
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class Validator : Attribute
    {
        protected Validator()
        {
            ExpectedType = typeof(string);
        }

        protected Validator(Type expectedType)
        {
            if (expectedType is null)
                throw new ArgumentNullException(nameof(expectedType));
            ExpectedType = expectedType;
        }

        public Type ExpectedType { get; }

        public object Value { get; private set; }

        /// <summary>
        ///     Validates the specified parameter value and throws an exception if the validation fails.
        /// </summary>
        /// <param name="parameterValue">The parameter value to validate.</param>
        /// <exception cref="ValidationException">Thrown if the validation fails.</exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if the validation returns an object whose type does not match the
        ///     <seealso cref="ExpectedType"/>.
        /// </exception>
        public virtual void Validate(string parameterValue)
        {
            Value = ValidateAsString(parameterValue);

            // If there is a value, check that it is compatible with the expected type.
#pragma warning disable S2219 // Runtime type checking should be simplified
            if (Value != null && !ExpectedType.IsAssignableFrom(Value.GetType()))
#pragma warning restore S2219 // Runtime type checking should be simplified
                throw new InvalidOperationException($"The {GetType().FullName} validator should validate an arg as a {ExpectedType.FullName} type, but is validating as a {Value.GetType().FullName} type.");

            // If the type is not a string, call the ValidateAsActualType method to perform additional
            // validations.
            if (ExpectedType != typeof(string))
                ValidateAsActualType(Value, parameterValue);
        }

        /// <summary>
        ///     Validates the parameter value as a string. Converts to the actual type if the validation
        ///     succeeds and returns that value. This method must be overridden in derived classes.
        /// </summary>
        /// <param name="parameterValue">The parameter value as a string.</param>
        /// <returns>The parameter value converted to its actual type.</returns>
        /// <exception cref="ValidationException">Thrown if the validation fails.</exception>
        protected abstract object ValidateAsString(string parameterValue);

        /// <summary>
        ///     Once the parameter has been validated as a string, it is converted to its actual type and passed here for
        ///     additional validations.
        ///     This is useful for additional validations that can only be performed on the actual typed value.
        /// </summary>
        /// <param name="value">The typed parameter value.</param>
        /// <param name="parameterValue">The parameter value as a string.</param>
        /// <exception cref="ValidationException">Thrown if the validation fails.</exception>
        protected virtual void ValidateAsActualType(object value, string parameterValue)
        {
        }

        /// <summary>
        ///     Shortcut method for throwing a failed validation exception. Use this from derived
        ///     classes, instead of throwing the exception directly.
        /// </summary>
        /// <param name="message">The validation error message.</param>
        /// <param name="parameterValue">The parameter value that caused the validation to fail.</param>
        /// <param name="args">Optional arguments to the message.</param>
        protected void ValidationFailed(string message, string parameterValue, params object[] args)
        {
            object[] formatArgs = new object[] { parameterValue }.Concat(args).ToArray();
            throw new ValidationException(string.Format(CultureInfo.CurrentCulture, message, formatArgs),
                GetType(), parameterValue);
        }
    }
}
