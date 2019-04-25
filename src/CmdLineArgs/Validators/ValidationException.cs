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
using System.Runtime.Serialization;

namespace ConsoleFx.CmdLineArgs.Validators
{
    /// <inheritdoc />
    /// <summary>
    ///     Exception thrown as a result of a validation failure.
    /// </summary>
    [Serializable]
    public class ValidationException : Exception
    {
        public ValidationException(string message, Type validatorType, string parameterValue)
            : base(message)
        {
            ValidatorType = validatorType;
            ParameterValue = parameterValue;
        }

        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        ///     Gets type of validator that caused the validation failure.
        /// </summary>
        public Type ValidatorType { get; }

        /// <summary>
        ///     Gets the parameter value that failed the validation.
        /// </summary>
        public string ParameterValue { get; }
    }
}
