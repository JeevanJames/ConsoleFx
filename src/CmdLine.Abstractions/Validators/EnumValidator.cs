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

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    /// <summary>
    ///     Checks if a value is a valid enum.
    /// </summary>
    public class EnumValidator : SingleMessageValidator<string>
    {
        public EnumValidator(Type enumType, bool ignoreCase = true)
            : base(Messages.Enum)
        {
            if (enumType is null)
                throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum)
                throw new ArgumentException("The enumType parameter should specify a enumerator type", nameof(enumType));
            EnumType = enumType;
            IgnoreCase = ignoreCase;
        }

        private Type EnumType { get; }

        private bool IgnoreCase { get; }

        protected override string ValidateAsString(string parameterValue)
        {
            string[] enumNames = Enum.GetNames(EnumType);
            StringComparison comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (!enumNames.Any(enumName => parameterValue.Equals(enumName, comparison)))
                ValidationFailed(Message, parameterValue);
            return parameterValue;
        }
    }

    public class EnumValidator<TEnum> : EnumValidator
        where TEnum : Enum
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
            EnumValidator validator = CreateValidator(enumType, ignoreCase, message);
            return argument.ValidateWith(validator);
        }

        public static Argument ValidateAsEnum<TEnum>(this Argument argument, bool ignoreCase = true, string message = null)
            where TEnum : Enum
        {
            var validator = new EnumValidator<TEnum>(ignoreCase);
            if (message != null)
                validator.Message = message;
            return argument.ValidateWith(validator);
        }

        public static Option ValidateAsEnum(this Option option, Type enumType, bool ignoreCase = true,
            int parameterIndex = -1, string message = null)
        {
            EnumValidator validator = CreateValidator(enumType, ignoreCase, message);
            return option.ValidateWith(parameterIndex, validator).TypeAs(enumType);
        }

        public static Option ValidateAsEnum<TEnum>(this Option option, bool ignoreCase = true, int parameterIndex = -1,
            string message = null)
            where TEnum : Enum
        {
            var validator = new EnumValidator<TEnum>(ignoreCase);
            if (message != null)
                validator.Message = message;
            return option.ValidateWith(parameterIndex, validator).TypeAs<TEnum>();
        }

        private static EnumValidator CreateValidator(Type enumType, bool ignoreCase, string message)
        {
            if (enumType is null)
                throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum)
                throw new ArgumentException("Specified type is not an enum.", nameof(enumType));

            var validator = new EnumValidator(enumType, ignoreCase);
            if (message != null)
                validator.Message = message;

            return validator;
        }
    }
}
