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
        public static Option Optional(this Option option, int max = 1)
        {
            option.Usage.Requirement = max == int.MaxValue
                ? OptionRequirement.OptionalUnlimited : OptionRequirement.Optional;
            return option;
        }

        public static Option Required(this Option option, int max = 1)
        {
            option.Usage.Requirement = max == int.MaxValue
                ? OptionRequirement.RequiredUnlimited : OptionRequirement.Required;
            return option;
        }

        public static Option ExpectedOccurences(this Option option, int expected)
        {
            option.Usage.ExpectedOccurences = expected;
            return option;
        }

        public static Option NoParameters(this Option option)
        {
            option.Usage.ParameterRequirement = OptionParameterRequirement.NotAllowed;
            return option;
        }

        public static Option ParametersOptional(this Option option, int max = 1)
        {
            option.Usage.ParameterRequirement = max == int.MaxValue
                ? OptionParameterRequirement.OptionalUnlimited : OptionParameterRequirement.Optional;
            return option;
        }

        public static Option ParametersRequired(this Option option, int max = 1)
        {
            option.Usage.ParameterRequirement = max == int.MaxValue
                ? OptionParameterRequirement.RequiredUnlimited : OptionParameterRequirement.Required;
            return option;
        }

        public static Option ExpectedParameters(this Option option, int expected)
        {
            option.Usage.ExpectedParameters = expected;
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