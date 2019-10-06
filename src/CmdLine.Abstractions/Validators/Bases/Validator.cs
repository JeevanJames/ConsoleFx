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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace ConsoleFx.CmdLine.Validators.Bases
{
    /// <summary>
    ///     Base class for all validators.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class Validator : Attribute
    {
        /// <summary>
        ///     Validates the specified parameter value and throws an exception if the validation fails.
        /// </summary>
        /// <param name="parameterValue">The parameter value to validate.</param>
        /// <exception cref="ValidationException">Thrown if the validation fails.</exception>
        public abstract void Validate(string parameterValue);

        /// <summary>
        ///     Shortcut method for throwing a failed validation exception. Use this from derived classes,
        ///     instead of throwing the exception directly.
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
