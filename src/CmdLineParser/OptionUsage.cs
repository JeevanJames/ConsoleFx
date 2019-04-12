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
    ///     Additional shortcut properties (<see cref="ExpectedOccurences" />,
    ///     <see cref="ExpectedParameters" /> and <see cref="Requirement" /> allow both min and max
    ///     values to be set for common scenarios.
    /// </summary>
    [DebuggerDisplay("{Requirement} | Parameters: {MinParameters} - {MaxParameters}")]
    public sealed partial class OptionUsage
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _minOccurences = Defaults.MinOccurences;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _maxOccurences = Defaults.MaxOccurences;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _minParameters = Defaults.MinParameters;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _maxParameters = Defaults.MaxParameters;

        /// <summary>
        ///     Specifies the maximum allowed occurences of the option.
        /// </summary>
        public int MaxOccurences
        {
            get => _maxOccurences;
            set
            {
                //An option should allow at least one occurence.
                //Allowing a max of 0 occurences is the same as saying that the option should never be used.
                //TODO: Change the exception message to something more appropriate.
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
                _maxOccurences = value;
            }
        }

        /// <summary>
        ///     Specifies the minimum allowed occurences of the option.
        /// </summary>
        public int MinOccurences
        {
            get => _minOccurences;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
                _minOccurences = value;
            }
        }

        /// <summary>
        ///     Shortcut to get/set both min and max occurence values.
        ///     If min and max values are different, returns null.
        ///     If set to null, then the defaults of 0 (min) and 1 (max) are set.
        /// </summary>
        public int? ExpectedOccurences
        {
            get => MinOccurences == MaxOccurences ? MinOccurences : (int?)null;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
                _minOccurences = value.GetValueOrDefault(Defaults.MinOccurences);
                _maxOccurences = value.GetValueOrDefault(Defaults.MaxOccurences);
            }
        }

        /// <summary>
        ///     Shortcut to set the option's occurences based on its requirement.
        /// </summary>
        public OptionRequirement Requirement
        {
            get
            {
                OptionRequirement requirement = MinOccurences > 0
                    ? OptionRequirement.Required
                    : OptionRequirement.Optional;
                if (MaxOccurences == Unlimited)
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
                        MinOccurences = 0;
                        //Don't change MaxOccurences. Let it remain at it's existing value.
                        break;
                    case OptionRequirement.OptionalUnlimited:
                        MinOccurences = 0;
                        MaxOccurences = Unlimited;
                        break;
                    case OptionRequirement.Required:
                        MinOccurences = 1;
                        break;
                    case OptionRequirement.RequiredUnlimited:
                        MinOccurences = 1;
                        MaxOccurences = Unlimited;
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
        ///     Shortcut to set the option's parameter occurences based on its requirement.
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
        ///     <para>Shortcut to get/set both min and max parameter values.</para>
        ///     <para>If min and max values are different, returns null.</para>
        ///     <para>If set to null, then the defaults of 0 (min) and 0 (max) are set.</para>
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
        ///         the <see cref="min"/> and <see cref="max"/> parameters.
        ///     </para>
        /// </summary>
        /// <param name="max">The maximum number of option parameters allowed.</param>
        /// <param name="min">The minimum number of option parameters required.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <see cref="min"/> parameter is less than zero.</exception>
        /// <exception cref="ArgumentException">Thrown if the <see cref="min"/> parameter is greater than the <see cref="max"/> parameter.</exception>
        public void SetParametersRequired(int max = 1, int min = 1)
        {
            if (min < 1)
                throw new ArgumentOutOfRangeException(nameof(min),
                    "Minimum parameters has to be one or more if they are required.");
            if (min > max)
                throw new ArgumentException(
                    $"Minimum parameter usage ({min}) cannot be larger than the maximum ({max}).", nameof(min));
            _minParameters = min;
            _maxParameters = max;
        }

        /// <summary>
        ///     Specifies whether the parameters of the option are repeating (all have the same meaning)
        ///     or individual (each is different and can have separate validators).
        /// </summary>
        public OptionParameterType ParameterType { get; set; }
    }

    public sealed partial class OptionUsage
    {
        public static class Defaults
        {
            public const int MinOccurences = 0;
            public const int MaxOccurences = 1;

            public const int MinParameters = 0;
            public const int MaxParameters = 0;
        }

        public const int Unlimited = int.MaxValue;
    }
}
