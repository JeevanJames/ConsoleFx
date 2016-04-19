﻿#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015-2016 Jeevan James

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

namespace ConsoleFx.Parser.Validators
{
    /// <summary>
    ///     Exception thrown as a result of a validation failure.
    /// </summary>
    public class ValidationException : ParserException
    {
        public ValidationException(string message, Type validatorType, string parameterValue)
            : base(Codes.ValidationFailed, message)
        {
            ValidatorType = validatorType;
            ParameterValue = parameterValue;
        }

        /// <summary>
        ///     Type of validator that caused the validation failure.
        /// </summary>
        public Type ValidatorType { get; }

        /// <summary>
        ///     The parameter value that failed the validation.
        /// </summary>
        public string ParameterValue { get; }
    }
}
