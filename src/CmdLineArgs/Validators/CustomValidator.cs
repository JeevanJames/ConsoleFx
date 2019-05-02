﻿#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2019 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System;

using ConsoleFx.CmdLineArgs.Validators.Bases;

namespace ConsoleFx.CmdLineArgs.Validators
{
    public sealed class CustomValidator : SingleMessageValidator<string>
    {
        private readonly Func<string, bool> _validator;

        public CustomValidator(Func<string, bool> validator)
            : base(Messages.Custom)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            _validator = validator;
        }

        protected sealed override string ValidateAsString(string parameterValue)
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
            if (message != null)
                val.Message = message;
            return argument.ValidateWith(val);
        }

        public static Option ValidateCustom(this Option option, Func<string, bool> validator,
            int parameterIndex = -1, string message = null)
        {
            var val = new CustomValidator(validator);
            if (message != null)
                val.Message = message;
            return option.ValidateWith(parameterIndex, val);
        }
    }
}