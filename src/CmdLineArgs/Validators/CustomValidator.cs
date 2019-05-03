#region --- License & Copyright Notice ---
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
using System.Diagnostics;
using ConsoleFx.CmdLineArgs.Validators.Bases;

namespace ConsoleFx.CmdLineArgs.Validators
{
    public sealed class CustomValidator : SingleMessageValidator<string>
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
