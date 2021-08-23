// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

namespace ConsoleFx.CmdLine
{
    public static class OptionExtensions
    {
        /// <summary>
        ///     Specifies that the option is to be used as a flag. If the option is specified, then
        ///     its value is <c>true</c>, otherwise it is <c>false</c>.
        /// </summary>
        /// <param name="option">The <see cref="Option"/> instance.</param>
        /// <param name="optional">Indicates whether the option can be specified.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        public static Option UsedAsFlag(this Option option, bool optional = true)
        {
            option.Usage.SetParametersNotAllowed();
            option.Usage.MinOccurrences = optional ? 0 : 1;
            option.Usage.MaxOccurrences = 1;
            return option;
        }

        /// <summary>
        ///     Specifies that the option is to have only a single parameter. This means that not more
        ///     than one occurence of the option and only one parameter for the option.
        /// </summary>
        /// <param name="option">The <see cref="Option"/> instance.</param>
        /// <param name="optional">If <c>true</c>, then the option does not need to be specified.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        public static Option UsedAsSingleParameter(this Option option, bool optional = true)
        {
            option.Usage.SetParametersRequired();
            option.Usage.MinOccurrences = optional ? 0 : 1;
            option.Usage.MaxOccurrences = 1;
            return option;
        }

        public static Option UsedAsUnlimitedOccurrencesAndParameters(this Option option, bool optional = false)
        {
            option.Usage.MinOccurrences = optional ? 0 : 1;
            option.Usage.MaxOccurrences = int.MaxValue;
            option.Usage.MinParameters = 1;
            option.Usage.MaxParameters = int.MaxValue;
            return option;
        }

        public static Option UsedAsSingleOccurrenceAndUnlimitedParameters(this Option option, bool optional = false)
        {
            option.Usage.Requirement = optional ? OptionRequirement.Optional : OptionRequirement.Required;
            option.Usage.ParameterRequirement = OptionParameterRequirement.Required;
            return option;
        }

        public static Option UsedAsUnlimitedOccurrencesAndSingleParameter(this Option option, bool optional = false)
        {
            option.Usage.MinOccurrences = optional ? 0 : 1;
            option.Usage.MaxOccurrences = int.MaxValue;
            option.Usage.ExpectedParameters = 1;
            return option;
        }
    }
}
