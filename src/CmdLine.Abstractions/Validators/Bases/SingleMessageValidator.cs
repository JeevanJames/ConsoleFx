// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.CmdLine.Validators.Bases
{
    /// <summary>
    ///     Base class for validators that only have one possible type of validation failure. In this
    ///     case, the class provides a Message property with a default value that can be changed.
    /// </summary>
    public abstract class SingleMessageValidator : Validator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleMessageValidator"/> class.
        /// </summary>
        /// <param name="message">The validation failure message.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="message"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="message"/> is empty or only white spaces.</exception>
        protected SingleMessageValidator(string message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));
            if (message.Trim().Length == 0)
                throw new ArgumentException("Specify a valid error message.", nameof(message));
            Message = message;
        }

        protected SingleMessageValidator(Type expectedType, string message)
            : base(expectedType)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));
            if (message.Trim().Length == 0)
                throw new ArgumentException("Specify a valid error message.", nameof(message));
            Message = message;
        }

        /// <summary>
        ///     Gets or sets the error message to be displayed if the validation fails.
        /// </summary>
        public string Message { get; set; }
    }
}
