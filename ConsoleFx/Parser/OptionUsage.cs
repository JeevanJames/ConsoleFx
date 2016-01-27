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

using System;
using System.Diagnostics;

namespace ConsoleFx.Parser
{
    /// <summary>
    ///     Rules for specifying an option on the command line. This includes number of occurences of
    ///     the option (defaults: 0 min and 1 max) and number of parameters (default: 0).
    ///     Additional shortcut properties (<see cref="ExpectedOccurences" />,
    ///     <see cref="ExpectedParameters" /> and <see cref="Requirement" /> allow both min and max
    ///     values to be set for common scenarios.
    /// </summary>
    [DebuggerDisplay("{Requirement} | Parameters: {MinParameters} - {MaxParameters}")]
    public sealed class OptionUsage
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _minOccurences;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _maxOccurences = 1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _minParameters;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _maxParameters;

        public OptionUsage()
        {
            //By default, options are optional... can be omitted or appear exactly once
            Requirement = OptionRequirement.Optional;

            //By default, parameters are not allowed on an option
            MinParameters = 0;
            MaxParameters = 0;
        }

        /// <summary>
        ///     Specifies the maximum allowed occurences of the option.
        /// </summary>
        public int MaxOccurences
        {
            get { return _maxOccurences; }
            set
            {
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
            get { return _minOccurences; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
                _minOccurences = value;
            }
        }

        /// <summary>
        ///     Specifies the maximum allowed number of parameters for the option.
        /// </summary>
        public int MaxParameters
        {
            get { return _maxParameters; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
                _maxParameters = value;
            }
        }

        /// <summary>
        ///     Specifies the minimum allowed number of parameters for the option.
        /// </summary>
        public int MinParameters
        {
            get { return _minParameters; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
                _minParameters = value;
            }
        }

        /// <summary>
        ///     Shortcut to get/set both min and max occurence values.
        ///     If min and max values are different, returns null.
        ///     If set to null, then the defaults of 0 (min) and 1 (max) are set.
        /// </summary>
        public int? ExpectedOccurences
        {
            get { return MinOccurences == MaxOccurences ? MinOccurences : (int?)null; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
                _minOccurences = value.GetValueOrDefault(0);
                _maxOccurences = value.GetValueOrDefault(1);
            }
        }

        /// <summary>
        ///     Shortcut to get/set both min and max parameter values.
        ///     If min and max values are different, returns null.
        ///     If set to null, then the defaults of 0 (min) and 0 (max) are set.
        /// </summary>
        public int? ExpectedParameters
        {
            get { return MinParameters == MaxParameters ? MinParameters : (int?)null; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
                _minParameters = value.GetValueOrDefault(0);
                _maxParameters = value.GetValueOrDefault(0);
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
                    ? OptionRequirement.Required : OptionRequirement.Optional;
                if (MaxOccurences == int.MaxValue)
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
                        break;
                    case OptionRequirement.OptionalUnlimited:
                        MinOccurences = 0;
                        MaxOccurences = int.MaxValue;
                        break;
                    case OptionRequirement.Required:
                        MinOccurences = 1;
                        break;
                    case OptionRequirement.RequiredUnlimited:
                        MinOccurences = 1;
                        MaxOccurences = int.MaxValue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        /// <summary>
        ///     Shortcut to set the option's parameter occurences based on its requirement.
        /// </summary>
        public OptionParameterRequirement ParameterRequirement
        {
            get
            {
                if (MinParameters == 0 && MaxParameters == 0)
                    return OptionParameterRequirement.NotAllowed;
                if (MinParameters == 0)
                {
                    return MaxParameters == int.MaxValue
                        ? OptionParameterRequirement.OptionalUnlimited : OptionParameterRequirement.Optional;
                }
                return MaxParameters == int.MaxValue
                    ? OptionParameterRequirement.RequiredUnlimited : OptionParameterRequirement.Required;
            }
            set
            {
                switch (value)
                {
                    case OptionParameterRequirement.Optional:
                        MinParameters = 0;
                        if (MaxParameters == 0)
                            MaxParameters = 1;
                        break;
                    case OptionParameterRequirement.OptionalUnlimited:
                        MinParameters = 0;
                        MaxParameters = int.MaxValue;
                        break;
                    case OptionParameterRequirement.Required:
                        MinParameters = 1;
                        if (MaxParameters == 0)
                            MaxParameters = 1;
                        break;
                    case OptionParameterRequirement.RequiredUnlimited:
                        MinParameters = 1;
                        MaxParameters = int.MaxValue;
                        break;
                    case OptionParameterRequirement.NotAllowed:
                        MinParameters = MaxParameters = 0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        public OptionParameterType ParameterType { get; set; }
    }
}