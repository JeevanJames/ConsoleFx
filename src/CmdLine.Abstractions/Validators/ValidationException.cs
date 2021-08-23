// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace ConsoleFx.CmdLine.Validators
{
    /// <inheritdoc />
    /// <summary>
    ///     Exception thrown as a result of a validation failure.
    /// </summary>
    [Serializable]
    public sealed class ValidationException : ParserException
    {
        public ValidationException(string message, Type validatorType, string parameterValue)
            : base(Codes.ValidationFailure, message)
        {
            ValidatorType = validatorType;
            ParameterValue = parameterValue;
        }

        private ValidationException(SerializationInfo info, StreamingContext context)
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
