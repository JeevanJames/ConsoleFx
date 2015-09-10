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

using System.Globalization;

namespace ConsoleFx.Parsers.Validators
{
    /// <summary>
    /// Base class for validators that only do a single check and hence can only produce a single error
    /// message.
    /// For such validators, you can set the ErrorMessage property to customize the exception message
    /// in the thrown validation exception.
    /// </summary>
    public abstract class SingleMessageValidator : BaseValidator
    {
        //Derived concrete classes will pass in a default error message from the ValidationMessages.resx
        //resource file. However, these are very generic messages, and in almost cases, it is better
        //to specify a custom message using the ErrorMessage property.
        protected SingleMessageValidator(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Custom error message that is assigned to the validation failure exception
        /// </summary>
        /// <remarks>
        /// The error message can have one placeholder parameter representing the parameter value.
        /// </remarks>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Shortcut method for throwing a failed validation exception. Use this from derived classes,
        /// instead of throwing the exception directly
        /// </summary>
        /// <param name="parameterValue">The parameter value that caused the validation to fail</param>
        protected void ValidationFailed(string parameterValue)
        {
            throw new ParserException(ParserException.Codes.ValidationFailed,
                string.Format(CultureInfo.CurrentCulture, ErrorMessage, parameterValue));
        }
    }
}