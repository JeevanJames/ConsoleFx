// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    public sealed class StringValidator : Validator
    {
        public StringValidator(int maxLength)
            : this(minLength: 1, maxLength)
        {
        }

        public StringValidator(int minLength, int maxLength)
        {
            if (minLength < 1)
                throw new ArgumentException("Minimum string length cannot be less than one.");
            if (minLength > maxLength)
                throw new ArgumentException("Minimum string length cannot be greater than maximum string length.");

            MinLength = minLength;
            MaxLength = maxLength;
        }

        protected override object ValidateAsString(string parameterValue)
        {
            if (parameterValue.Length < MinLength)
                ValidationFailed(MinLengthMessage, parameterValue, MinLength);
            if (parameterValue.Length > MaxLength)
                ValidationFailed(MaxLengthMessage, parameterValue, MaxLength);
            return parameterValue;
        }

        public int MaxLength { get; }

        public int MinLength { get; }

        public string MinLengthMessage { get; set; } = Messages.String_MinLength;

        public string MaxLengthMessage { get; set; } = Messages.String_MaxLength;
    }

    public static class StringValidatorExtensions
    {
        public static Argument ValidateAsString(this Argument argument, int minLength, int maxLength = int.MaxValue,
            string minLengthMessage = null, string maxLengthMessage = null)
        {
            var validator = new StringValidator(minLength, maxLength);
            if (minLengthMessage is not null)
                validator.MinLengthMessage = minLengthMessage;
            if (maxLengthMessage is not null)
                validator.MaxLengthMessage = maxLengthMessage;
            return argument.ValidateWith(validator);
        }

        public static Option ValidateAsString(this Option option, int minLength, int maxLength = int.MaxValue,
            int parameterIndex = -1, string minLengthMessage = null, string maxLengthMessage = null)
        {
            var validator = new StringValidator(minLength, maxLength);
            if (minLengthMessage is not null)
                validator.MinLengthMessage = minLengthMessage;
            if (maxLengthMessage is not null)
                validator.MaxLengthMessage = maxLengthMessage;
            return option.ValidateWith(parameterIndex, validator);
        }
    }
}
