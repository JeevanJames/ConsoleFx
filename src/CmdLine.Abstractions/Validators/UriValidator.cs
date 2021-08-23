// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    public class UriValidator : SingleMessageValidator
    {
        public UriValidator(UriKind uriKind = UriKind.RelativeOrAbsolute)
            : base(typeof(Uri), Messages.Uri)
        {
            UriKind = uriKind;
        }

        public UriKind UriKind { get; }

        protected override object ValidateAsString(string parameterValue)
        {
            if (!Uri.TryCreate(parameterValue, UriKind, out Uri uri))
                ValidationFailed(Message, parameterValue);
            return uri;
        }
    }

    public static class UriValidatorExtensions
    {
        public static Argument ValidateAsUri(this Argument argument, UriKind uriKind = UriKind.RelativeOrAbsolute,
            string message = null)
        {
            var validator = new UriValidator(uriKind);
            if (message != null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Option ValidateAsUri(this Option option, UriKind uriKind = UriKind.RelativeOrAbsolute,
            int parameterIndex = -1, string message = null)
        {
            var validator = new UriValidator(uriKind);
            if (message != null)
                validator.Message = message;
            return option.ValidateWith(parameterIndex, validator).TypeAs<Uri>();
        }
    }
}
