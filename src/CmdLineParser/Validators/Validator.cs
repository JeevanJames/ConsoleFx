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

using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace ConsoleFx.CmdLineParser.Validators
{
    /// <summary>
    ///     Base class for all validators
    /// </summary>
    public abstract class Validator
    {
        /// <summary>
        ///     Validates the specified parameter value and throws an exception if the validation fails.
        /// </summary>
        /// <param name="parameterValue">The parameter value to validate.</param>
        /// <exception cref="ValidationException">Thrown if the validation fails.</exception>
        public abstract void Validate(string parameterValue);

        /// <summary>
        ///     Shortcut method for throwing a failed validation exception. Use this from derived classes,
        ///     instead of throwing the exception directly
        /// </summary>
        /// <param name="message">The validation error message</param>
        /// <param name="parameterValue">The parameter value that caused the validation to fail</param>
        /// <param name="args">Optional arguments to the message.</param>
        protected void ValidationFailed(string message, string parameterValue, params object[] args)
        {
            object[] formatArgs = new object[] { parameterValue }.Concat(args).ToArray();
            throw new ValidationException(string.Format(CultureInfo.CurrentCulture, message, formatArgs),
                GetType(), parameterValue);
        }
    }

    /// <summary>
    ///     Base class for typed validators; i.e. validators that perform additional validations once the parameter value has
    ///     been converted to its native type.
    ///     This class breaks the validation into 2 parts - validation of the string parameter value and validation of the
    ///     converted typed value.
    /// </summary>
    /// <typeparam name="T">The type of the data being validated.</typeparam>
    //TODO: The conversion performed by this class seems redundant considering the type conversion
    //details specified by the option itself (using its Type and TypeConverter properties). Can we
    //perform the conversion once and then pass the converted value into this class for the typed
    //validation. Also, consider that the conversion logic for validators and that for the option
    //could be different, leading to inconsistencies.
    public abstract class Validator<T> : Validator
    {
        /// <inheritdoc/>
        public sealed override void Validate(string parameterValue)
        {
            T value = ValidateAsString(parameterValue);
            ValidateAsActualType(value, parameterValue);
        }

        /// <summary>
        ///     Validates the parameter value as a string. Converts to the actual type if the validation succeeds and returns that
        ///     value. This method must be overridden in derived classes.
        /// </summary>
        /// <param name="parameterValue">The parameter value as a string</param>
        /// <returns>The parameter value converted to its actual type.</returns>
        /// <exception cref="ValidationException">Thrown if the validation fails.</exception>
        protected abstract T ValidateAsString(string parameterValue);

        /// <summary>
        ///     Once the parameter has been validated as a string, it is converted to its actual type and passed here for
        ///     additional validations.
        ///     This is useful for additional validations that can only be performed on the actual typed value.
        /// </summary>
        /// <param name="value">The typed parameter value.</param>
        /// <param name="parameterValue">The parameter value as a string</param>
        /// <exception cref="ValidationException">Thrown if the validation fails.</exception>
        protected virtual void ValidateAsActualType(T value, string parameterValue)
        {
        }
    }

    /// <summary>
    ///     Collection of validator classes
    /// </summary>
    public sealed class ValidatorCollection : Collection<Validator>
    {
    }
}
