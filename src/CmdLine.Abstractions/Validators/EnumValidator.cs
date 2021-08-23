// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine.Validators
{
    /// <summary>
    ///     Checks if a value is a valid enum.
    /// </summary>
    public class EnumValidator : SingleMessageValidator
    {
        public EnumValidator(Type enumType, bool ignoreCase = true)
            : base(enumType, Messages.Enum)
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

        protected override object ValidateAsString(string parameterValue)
        {
            string[] enumNames = Enum.GetNames(EnumType);
            StringComparison comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (!enumNames.Any(enumName => parameterValue.Equals(enumName, comparison)))
                ValidationFailed(Message, parameterValue);
            return Enum.Parse(EnumType, parameterValue, IgnoreCase);
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
            var validator = new EnumValidator(typeof(TEnum), ignoreCase);
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
            var validator = new EnumValidator(typeof(TEnum), ignoreCase);
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
