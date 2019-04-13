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
using System.Diagnostics;

namespace ConsoleFx.CmdLineParser
{
    /// <summary>
    ///     Rules for specifying an option on the command line. This includes number of occurences of
    ///     the option (defaults: 0 min and 1 max) and number of parameters (default: 0).
    ///     Additional shortcut properties (<see cref="ExpectedOccurrences" />,
    ///     <see cref="ExpectedParameters" /> and <see cref="Requirement" /> allow both min and max
    ///     values to be set for common scenarios.
    /// </summary>
    [DebuggerDisplay("{Requirement} | Parameters: {MinParameters} - {MaxParameters}")]
    public sealed partial class OptionUsage
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _minOccurrences = Defaults.MinOccurrences;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _maxOccurrences = Defaults.MaxOccurrences;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _minParameters = Defaults.MinParameters;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _maxParameters = Defaults.MaxParameters;

        /// <summary>
        ///     Gets or sets the maximum allowed occurrences of the option.
        /// </summary>
        public int MaxOccurrences
        {
            get => _maxOccurrences;
            set
            {
                // An option should allow at least one occurence.
                // Allowing a max of 0 occurrences is the same as saying that the option should never be used.
                //TODO: Change the exception message to something more appropriate.
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
                _maxOccurrences = value;
            }
        }

        /// <summary>
        ///     Gets or sets the minimum allowed occurrences of the option.
        /// </summary>
        public int MinOccurrences
        {
            get => _minOccurrences;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
                _minOccurrences = value;
            }
        }

        /// <summary>
        ///     Gets or sets the expected number of occurrences of the property.
        ///     Shortcut to get/set both min and max occurence values.
        ///     If min and max values are different, returns null.
        ///     If set to null, then the defaults of 0 (min) and 1 (max) are set.
        /// </summary>
        public int? ExpectedOccurrences
        {
            get => MinOccurrences == MaxOccurrences ? MinOccurrences : (int?)null;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
                _minOccurrences = value.GetValueOrDefault(Defaults.MinOccurrences);
                _maxOccurrences = value.GetValueOrDefault(Defaults.MaxOccurrences);
            }
        }

        /// <summary>
        ///     Gets or sets the option's occurrences based on its requirement.
        /// </summary>
        public OptionRequirement Requirement
        {
            get
            {
                OptionRequirement requirement = MinOccurrences > 0
                    ? OptionRequirement.Required
                    : OptionRequirement.Optional;
                if (MaxOccurrences == Unlimited)
                {
                    if (requirement == OptionRequirement.Required)
                        requirement = OptionRequirement.RequiredUnlimited;
                    else if (requirement == OptionRequirement.Optional)
                        requirement = OptionRequirement.OptionalUnlimited;
                }

                return requirement;
            }

            set
            {
                switch (value)
                {
                    case OptionRequirement.Optional:
                        // Don't change MaxOccurrences. Let it remain at it's existing value.
                        MinOccurrences = 0;
                        break;
                    case OptionRequirement.OptionalUnlimited:
                        MinOccurrences = 0;
                        MaxOccurrences = Unlimited;
                        break;
                    case OptionRequirement.Required:
                        MinOccurrences = 1;
                        break;
                    case OptionRequirement.RequiredUnlimited:
                        MinOccurrences = 1;
                        MaxOccurrences = Unlimited;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        /// <summary>
        ///     Gets the maximum allowed number of parameters for the option.
        /// </summary>
        public int MaxParameters => _maxParameters;

        /// <summary>
        ///     Gets the minimum allowed number of parameters for the option.
        /// </summary>
        public int MinParameters => _minParameters;

        /// <summary>
        ///     Gets or sets the parameter occurrences based on its requirement.
        /// </summary>
        public OptionParameterRequirement ParameterRequirement
        {
            get => MinParameters == 0 && MaxParameters == 0
                ? OptionParameterRequirement.NotAllowed : OptionParameterRequirement.Required;
            set
            {
                switch (value)
                {
                    case OptionParameterRequirement.NotAllowed:
                        SetParametersNotAllowed();
                        break;
                    case OptionParameterRequirement.Required:
                        SetParametersRequired();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        /// <summary>
        ///     Gets or sets the expected number of parameters for the option.
        ///     <para/>
        ///     This is a shortcut to getting and setting the min and max properties to the same value.
        ///     <para/>
        ///     If min and max values are different, the getter returns <c>null</c>.
        ///     <para/>
        ///     If set to <c>null</c>, then the defaults of 0 (min) and 0 (max) are set.
        /// </summary>
        public int? ExpectedParameters
        {
            get => MinParameters == MaxParameters ? MinParameters : (int?)null;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
                _minParameters = value.GetValueOrDefault(Defaults.MinParameters);
                _maxParameters = value.GetValueOrDefault(Defaults.MaxParameters);
            }
        }

        /// <summary>
        ///     Disallows parameters for the option.
        /// </summary>
        public void SetParametersNotAllowed()
        {
            _minParameters = _maxParameters = 0;
        }

        /// <summary>
        ///     <para>Enforces that parameters should be specified for an option.</para>
        ///     <para>
        ///         By default, only one parameter is required, but this can be customized by specifying
        ///         the <paramref name="min"/> and <paramref name="max"/> parameters.
        ///     </para>
        /// </summary>
        /// <param name="max">The maximum number of option parameters allowed.</param>
        /// <param name="min">The minimum number of option parameters required.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="min"/> parameter is less than zero.</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="min"/> parameter is greater than the <paramref name="max"/> parameter.</exception>
        public void SetParametersRequired(int max = 1, int min = 1)
        {
            if (min < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(min),
                    "Minimum parameters has to be one or more if they are required.");
            }

            if (min > max)
            {
                throw new ArgumentException(
                    $"Minimum parameter usage ({min}) cannot be larger than the maximum ({max}).", nameof(min));
            }

            _minParameters = min;
            _maxParameters = max;
        }

        /// <summary>
        ///     Gets or sets whether the parameters of the option are repeating (all have the same meaning) or individual
        ///     (each is different and can have separate validators).
        /// </summary>
        public OptionParameterType ParameterType { get; set; }
    }

    public sealed partial class OptionUsage
    {
        public static class Defaults
        {
            public const int MinOccurrences = 0;
            public const int MaxOccurrences = 1;

            public const int MinParameters = 0;
            public const int MaxParameters = 0;
        }

        public const int Unlimited = int.MaxValue;
    }
}
