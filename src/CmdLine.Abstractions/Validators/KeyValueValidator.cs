// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text.RegularExpressions;

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    public sealed class KeyValueValidator : SingleMessageValidator
    {
        public KeyValueValidator()
            : base(typeof(KeyValuePair<string, string>), Messages.KeyValue)
        {
        }

        protected override object ValidateAsString(string parameterValue)
        {
            Match match = KeyValuePattern.Match(parameterValue);
            if (match.Success)
                return new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[2].Value);
            ValidationFailed(Message, parameterValue);
            return default;
        }

        private static readonly Regex KeyValuePattern = new Regex(@"^(\w[\w-_]*)=(.+)$");
    }

    public static class KeyValueValidatorExtensions
    {
        public static Argument ValidateAsKeyValue(this Argument argument, string message = null)
        {
            var validator = new KeyValueValidator();
            if (message != null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Option ValidateAsKeyValue(this Option option, int parameterIndex = -1, string message = null)
        {
            var validator = new KeyValueValidator();
            if (message != null)
                validator.Message = message;
            return option.ValidateWith(parameterIndex, validator);
        }
    }
}
