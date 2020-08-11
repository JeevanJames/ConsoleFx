#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

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
