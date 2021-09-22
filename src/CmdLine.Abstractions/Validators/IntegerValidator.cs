// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    public class IntegerValidator : Validator
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly long _minimumValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly long _maximumValue;

        public IntegerValidator(long minimumValue = long.MinValue, long maximumValue = long.MaxValue)
            : base(typeof(long))
        {
            if (minimumValue > maximumValue)
                throw new ArgumentException("Minimum value cannot be greater than the maximum value.", nameof(minimumValue));
            _minimumValue = minimumValue;
            _maximumValue = maximumValue;
        }

        public string NotAnIntegerMessage { get; set; } = Messages.Integer_NotAnInteger;

        public string OutOfRangeMessage { get; set; } = Messages.Integer_OutOfRange;

        protected override object ValidateAsString(string parameterValue)
        {
            if (!long.TryParse(parameterValue, out long value))
                ValidationFailed(NotAnIntegerMessage, parameterValue);
            if (value < _minimumValue || value > _maximumValue)
                ValidationFailed(OutOfRangeMessage, parameterValue);
            return value;
        }
    }

    public static class IntegerValidatorExtensions
    {
        public static Argument ValidateAsInteger(this Argument argument, long minimumValue = long.MinValue,
            long maximumValue = long.MaxValue, string notAnIntegerMessage = null, string outOfRangeMessage = null)
        {
            var validator = new IntegerValidator(minimumValue, maximumValue);
            if (notAnIntegerMessage is not null)
                validator.NotAnIntegerMessage = notAnIntegerMessage;
            if (outOfRangeMessage is not null)
                validator.OutOfRangeMessage = outOfRangeMessage;
            return argument.ValidateWith(validator);
        }

        public static Option ValidateAsInteger(this Option option, long minimumValue = long.MinValue,
            long maximumValue = long.MaxValue, int parameterIndex = -1, string notAnIntegerMessage = null,
            string outOfRangeMessage = null)
        {
            var validator = new IntegerValidator(minimumValue, maximumValue);
            if (notAnIntegerMessage is not null)
                validator.NotAnIntegerMessage = notAnIntegerMessage;
            if (outOfRangeMessage is not null)
                validator.OutOfRangeMessage = outOfRangeMessage;
            return option.ValidateWith(parameterIndex, validator).TypeAs<long>();
        }
    }
}
