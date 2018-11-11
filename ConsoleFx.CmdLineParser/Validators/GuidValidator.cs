#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2018 Jeevan James

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

namespace ConsoleFx.CmdLineParser.Validators
{
    public class GuidValidator : SingleMessageValidator<Guid>
    {
        public GuidValidator() : base(Messages.Guid)
        {
        }

        protected override Guid ValidateAsString(string parameterValue)
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
            if (message != null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Option ValidateAsGuid(this Option option, int parameterIndex = -1, string message = null)
        {
            var validator = new GuidValidator();
            if (message != null)
                validator.Message = message;
            return option.ValidateWith(parameterIndex, validator);
        }
    }
}
