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
using System.Linq;

using ConsoleFx.CmdLineArgs.Validators.Bases;

namespace ConsoleFx.CmdLineArgs.Validators
{
    public class EnumValidator : SingleMessageValidator<string>
    {
        private Type EnumType { get; }

        private bool IgnoreCase { get; }

        public EnumValidator(Type enumType, bool ignoreCase = true)
            : base(Messages.Enum)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum)
                throw new ArgumentException("The enumType parameter should specify a enumerator type", nameof(enumType));
            EnumType = enumType;
            IgnoreCase = ignoreCase;
        }

        protected override string ValidateAsString(string parameterValue)
        {
            string[] enumNames = Enum.GetNames(EnumType);
            StringComparison comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (!enumNames.Any(enumName => parameterValue.Equals(enumName, comparison)))
                ValidationFailed(parameterValue, Message);
            return parameterValue;
        }
    }

    public class EnumValidator<TEnum> : EnumValidator
        where TEnum : struct, IComparable
    {
        public EnumValidator(bool ignoreCase = true)
            : base(typeof(TEnum), ignoreCase)
        {
        }
    }

    public static class EnumValidatorExtensions
    {
        public static Argument ValidateAsEnum(this Argument argument, Type enumType, bool ignoreCase = true,
            string message = null)
        {
            var validator = new EnumValidator(enumType, ignoreCase);
            if (message != null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Argument ValidateAsEnum<TEnum>(this Argument argument, bool ignoreCase = true, string message = null)
            where TEnum : struct, IComparable
        {
            var validator = new EnumValidator<TEnum>(ignoreCase);
            if (message != null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Option ValidateAsEnum(this Option option, Type enumType, bool ignoreCase = true,
            int parameterIndex = -1, string message = null)
        {
            var validator = new EnumValidator(enumType, ignoreCase);
            if (message != null)
                validator.Message = message;
            return option.ValidateWith(parameterIndex, validator);
        }

        public static Option ValidateAsEnum<TEnum>(this Option option, bool ignoreCase = true, int parameterIndex = -1,
            string message = null)
            where TEnum : struct, IComparable
        {
            var validator = new EnumValidator<TEnum>(ignoreCase);
            if (message != null)
                validator.Message = message;
            return option.ValidateWith(parameterIndex, validator);
        }
    }
}
