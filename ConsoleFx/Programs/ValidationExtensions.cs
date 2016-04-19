using System;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Validators;

namespace ConsoleFx.Programs
{
    /// <summary>
    ///     Extensions to the Option and Argument classes to set them up for various validations.
    ///     Validations include the parameter validations as well as the arg usage validations.
    /// </summary>
    public static class ValidationExtensions
    {
        public static Option FormatParamsAs(this Option option, OptionParameterFormatter formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));
            option.Formatter = formatter;
            return option;
        }

        public static Option FormatParamsAs(this Option option, string formatStr)
        {
            if (string.IsNullOrWhiteSpace(formatStr))
                throw new ArgumentException(@"The format string cannot be empty or blank.", nameof(formatStr));
            option.Formatter = value => string.Format(formatStr, value);
            return option;
        }

        public static Option ParamsOfType<T>(this Option option, Converter<string, T> converter = null)
        {
            option.Type = typeof(T);
            if (converter != null)
                option.TypeConverter = value => converter(value);
            return option;
        }

        public static Option Usage(this Option option, Action<OptionUsage> usageSetter)
        {
            if (usageSetter == null)
                throw new ArgumentNullException(nameof(usageSetter));
            usageSetter(option.Usage);
            return option;
        }

        public static Argument ValidateWith(this Argument argument, params Validator[] validators)
        {
            foreach (Validator validator in validators)
                argument.Validators.Add(validator);
            return argument;
        }

        public static Argument ValidateWith(this Argument argument, Func<string, bool> customValidator) =>
            ValidateWith(argument, new CustomValidator(customValidator));

        public static Option ValidateWith(this Option option, params Validator[] validators)
        {
            foreach (Validator validator in validators)
                option.Validators.Add(validator);
            return option;
        }

        public static Option ValidateWith(this Option option, Func<string, bool> customValidator) =>
            ValidateWith(option, new CustomValidator(customValidator));

        public static Option ValidateWith(this Option option, int parameterIndex, params Validator[] validators)
        {
            foreach (Validator validator in validators)
                option.Validators.Add(parameterIndex, validator);
            return option;
        }
    }
}