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

namespace ConsoleFx.Parser.Validators
{
    /// <summary>
    /// Base class for all validators
    /// </summary>
    public abstract class Validator
    {
        public abstract void Validate(string parameterValue);

        /// <summary>
        /// Shortcut method for throwing a failed validation exception. Use this from derived classes,
        /// instead of throwing the exception directly
        /// </summary>
        /// <param name="message">The validation error message</param>
        /// <param name="parameterValue">The parameter value that caused the validation to fail</param>
        /// <param name="args">Optional arguments to the message.</param>
        protected static void ValidationFailed(string message, string parameterValue, params object[] args)
        {
            object[] formatArgs = new object[] { parameterValue }.Concat(args).ToArray();
            throw new ParserException(ParserException.Codes.ValidationFailed,
                string.Format(CultureInfo.CurrentCulture, message, formatArgs));
        }
    }

    /// <summary>
    /// Base class for validators that perform multiple checks and hence can produce more than one
    /// error message.
    /// </summary>
    public abstract class Validator<T> : Validator
    {
        public sealed override void Validate(string parameterValue)
        {
            T value = PrimaryChecks(parameterValue);
            AdditionalChecks(value);
        }

        protected abstract T PrimaryChecks(string parameterValue);

        protected virtual void AdditionalChecks(T value)
        {
        }
    }

    /// <summary>
    /// Collection of validator classes
    /// </summary>
    public sealed class ValidatorCollection : Collection<Validator>
    {
        public void AddRange(IEnumerable<Validator> validators)
        {
            foreach (Validator validator in validators)
                Add(validator);
        }
    }
}