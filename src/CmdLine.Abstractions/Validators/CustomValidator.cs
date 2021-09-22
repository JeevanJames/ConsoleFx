// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    public sealed class CustomValidator : SingleMessageValidator
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Func<string, bool> _validator;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomValidator"/> class.
        /// </summary>
        /// <param name="validator">A delegate that represents the custom validator logic.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="validator"/> is <c>null</c>.</exception>
        public CustomValidator(Func<string, bool> validator)
            : base(Messages.Custom)
        {
            if (validator is null)
                throw new ArgumentNullException(nameof(validator));
            _validator = validator;
        }

        protected sealed override object ValidateAsString(string parameterValue)
        {
            if (!_validator(parameterValue))
                ValidationFailed(parameterValue, Message);
            return parameterValue;
        }
    }

    public static class CustomValidatorExtensions
    {
        public static Argument ValidateCustom(this Argument argument, Func<string, bool> validator,
            string message = null)
        {
            var val = new CustomValidator(validator);
            if (message is not null)
                val.Message = message;
            return argument.ValidateWith(val);
        }

        public static Option ValidateCustom(this Option option, Func<string, bool> validator,
            int parameterIndex = -1, string message = null)
        {
            var val = new CustomValidator(validator);
            if (message is not null)
                val.Message = message;
            return option.ValidateWith(parameterIndex, val);
        }
    }
}
