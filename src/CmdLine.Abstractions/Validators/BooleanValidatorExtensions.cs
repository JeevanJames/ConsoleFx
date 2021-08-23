// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace ConsoleFx.CmdLine.Validators
{
    public static class BooleanValidatorExtensions
    {
        public static Argument ValidateAsBoolean(this Argument argument, string trueString = "true",
            string falseString = "false", bool caseSensitive = false, string message = null)
        {
            var validator = new BooleanValidator(trueString, falseString, caseSensitive);
            if (message != null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Argument ValidateAsBoolean(this Argument argument, IEnumerable<string> trueStrings,
            IEnumerable<string> falseStrings, bool caseSensitive = false, string message = null)
        {
            var validator = new BooleanValidator(trueStrings, falseStrings, caseSensitive);
            if (message != null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Option ValidateAsBoolean(this Option option, string trueString = "true",
            string falseString = "false", bool caseSensitive = false, int parameterIndex = -1, string message = null)
        {
            var validator = new BooleanValidator(trueString, falseString, caseSensitive);
            if (message != null)
                validator.Message = message;
            return option.ValidateWith(parameterIndex, validator);
        }

        public static Option ValidateAsBoolean(this Option option, IEnumerable<string> trueStrings,
            IEnumerable<string> falseStrings, bool caseSensitive = false, int parameterIndex = -1,
            string message = null)
        {
            var validator = new BooleanValidator(trueStrings, falseStrings, caseSensitive);
            if (message != null)
                validator.Message = message;
            return option.ValidateWith(parameterIndex, validator);
        }
    }
}
