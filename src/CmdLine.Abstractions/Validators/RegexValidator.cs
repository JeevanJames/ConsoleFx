// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.RegularExpressions;

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    public class RegexValidator : SingleMessageValidator
    {
        public RegexValidator(Regex regex)
            : base(Messages.Regex)
        {
            if (regex is null)
                throw new ArgumentNullException(nameof(regex));
            Regex = regex;
        }

        public RegexValidator(string pattern)
            : base(Messages.Regex)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentException("Value cannot be null or empty.", nameof(pattern));
            Regex = new Regex(pattern);
        }

        public Regex Regex { get; }

        protected override object ValidateAsString(string parameterValue)
        {
            if (!Regex.IsMatch(parameterValue))
                ValidationFailed(Message, parameterValue);
            return parameterValue;
        }
    }

    public static class RegexValidatorExtensions
    {
        public static Argument ValidateWithRegex(this Argument argument, Regex regex, string message = null)
        {
            var validator = new RegexValidator(regex);
            if (message != null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Argument ValidateWithRegex(this Argument argument, string pattern, string message = null)
        {
            var validator = new RegexValidator(pattern);
            if (message != null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Option ValidateWithRegex(this Option option, Regex regex, int parameterIndex = -1,
            string message = null)
        {
            var validator = new RegexValidator(regex);
            if (message != null)
                validator.Message = message;
            return option.ValidateWith(parameterIndex, validator);
        }

        public static Option ValidateWithRegex(this Option option, string pattern, int parameterIndex = -1, string message = null)
        {
            var validator = new RegexValidator(pattern);
            if (message != null)
                validator.Message = message;
            return option.ValidateWith(parameterIndex, validator);
        }
    }
}
