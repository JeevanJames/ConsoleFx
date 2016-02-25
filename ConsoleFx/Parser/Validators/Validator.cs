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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConsoleFx.Parser.Validators
{
    /// <summary>
    ///     Base class for all validators
    /// </summary>
    public abstract class Validator
    {
        /// <summary>
        /// Validates the specified parameter value and throws an exception if the validation fails.
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
    ///     Base class for validators that perform multiple checks and hence can produce more than one
    ///     error message.
    /// </summary>
    public abstract class Validator<T> : Validator
    {
        public sealed override void Validate(string parameterValue)
        {
            T value = ValidateAsString(parameterValue);
            ValidateAsActualType(value);
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
        /// <exception cref="ValidationException">Thrown if the validation fails.</exception>
        protected virtual void ValidateAsActualType(T value)
        {
        }
    }

    public abstract class SingleMessageValidator<T> : Validator<T>
    {
        protected SingleMessageValidator(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }

    /// <summary>
    ///     Collection of validator classes
    /// </summary>
    public sealed class ValidatorCollection : Collection<Validator>
    {
    }
}
