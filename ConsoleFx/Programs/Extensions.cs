#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015 Jeevan James

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

using ConsoleFx.Parser;
using ConsoleFx.Parser.Validators;

namespace ConsoleFx.Programs
{
    public static class Extensions
    {
        public static Option Optional(this Option option, int max = 1)
        {
            option.Usage.Requirement = max == int.MaxValue ? OptionRequirement.OptionalUnlimited : OptionRequirement.Optional;
            return option;
        }

        public static Option Required(this Option option, int max = 1)
        {
            option.Usage.Requirement = max == int.MaxValue ? OptionRequirement.RequiredUnlimited : OptionRequirement.Required;
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
            option.Usage.ParameterRequirement = max == int.MaxValue ? OptionParameterRequirement.OptionalUnlimited : OptionParameterRequirement.Optional;
            return option;
        }

        public static Option ParametersRequired(this Option option, int max = 1)
        {
            option.Usage.ParameterRequirement = max == int.MaxValue ? OptionParameterRequirement.RequiredUnlimited : OptionParameterRequirement.Required;
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

        public static Option ValidateWith(this Option option, params Validator[] validators)
        {
            foreach (Validator validator in validators)
                option.Validators.Add(validator);
            return option;
        }

        public static Option ValidateWith(this Option option, int parameterIndex, params Validator[] validators)
        {
            foreach (Validator validator in validators)
                option.Validators.Add(parameterIndex, validator);
            return option;
        }
    }
}
