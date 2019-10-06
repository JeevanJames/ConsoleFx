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

namespace ConsoleFx.CmdLine.Validators.Bases
{
    /// <summary>
    ///     Base class for typed validators; i.e. validators that perform additional validations once the parameter value has
    ///     been converted to its native type.
    ///     This class breaks the validation into 2 parts - validation of the string parameter value and validation of the
    ///     converted typed value.
    /// </summary>
    //TODO: The conversion performed by this class seems redundant considering the type conversion
    //details specified by the option itself (using its Type and TypeConverter properties). Can we
    //perform the conversion once and then pass the converted value into this class for the typed
    //validation. Also, consider that the conversion logic for validators and that for the option
    //could be different, leading to inconsistencies.
    public abstract class TypedValidator : Validator
    {
        protected TypedValidator()
        {
            ExpectedType = typeof(string);
        }

        protected TypedValidator(Type expectedType)
        {
            if (expectedType is null)
                throw new ArgumentNullException(nameof(expectedType));
            ExpectedType = expectedType;
        }

        public Type ExpectedType { get; }

        public object Value { get; private set; }

        /// <inheritdoc/>
        public sealed override void Validate(string parameterValue)
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
    }
}
