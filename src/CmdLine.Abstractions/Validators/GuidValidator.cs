// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    /// <summary>
    ///     Validates whether an arg value is a valid <see cref="Guid"/>.
    /// </summary>
    public class GuidValidator : SingleMessageValidator
    {
        public GuidValidator()
            : base(typeof(Guid), Messages.Guid)
        {
        }

        protected override object ValidateAsString(string parameterValue)
        {
            if (!Guid.TryParse(parameterValue, out Guid guid))
                ValidationFailed(parameterValue, Message);
            return guid;
        }
    }

    public static class GuidValidatorExtensions
    {
        public static Argument ValidateAsGuid(this Argument argument, string message = null)
        {
            var validator = new GuidValidator();
            if (message is not null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Option ValidateAsGuid(this Option option, int parameterIndex = -1, string message = null)
        {
            var validator = new GuidValidator();
            if (message is not null)
                validator.Message = message;
            return option.ValidateWith(parameterIndex, validator).TypeAs<Guid>();
        }
    }
}
