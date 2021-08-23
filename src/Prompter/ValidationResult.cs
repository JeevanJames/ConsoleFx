// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.Prompter
{
    /// <summary>
    ///     Represents the result of a validation on a question's answer.
    /// </summary>
    public readonly struct ValidationResult
    {
        /// <summary>
        ///     Gets a value indicating whether the answer is valid.
        /// </summary>
        internal bool IsValid { get; }

        /// <summary>
        ///     Gets the error message describing the reason why the validation failed.
        /// </summary>
        internal string ErrorMessage { get; }

        internal ValidationResult(bool isValid)
        {
            IsValid = isValid;
            ErrorMessage = null;
        }

        internal ValidationResult(string errorMessage)
        {
            if (errorMessage is null)
                throw new ArgumentNullException(nameof(errorMessage));
            IsValid = false;
            ErrorMessage = errorMessage;
        }

        public static implicit operator ValidationResult(bool isValid) => new(isValid);

        public static implicit operator ValidationResult(string errorMessage) => new(errorMessage);

        public static readonly ValidationResult Valid = new(true);

        public static readonly ValidationResult Invalid = new(false);
    }
}
