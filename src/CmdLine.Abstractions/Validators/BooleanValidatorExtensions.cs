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
