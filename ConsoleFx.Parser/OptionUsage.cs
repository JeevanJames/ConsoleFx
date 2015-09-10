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
    /// Rules for specifying an option on the command line. This includes number of occurences of
    /// the option (defaults: 0 min and 1 max) and number of parameters (default: 0).
    /// Additional shortcut properties (<see cref="ExpectedOccurences"/>, 
    /// <see cref="ExpectedParameters"/> and <see cref="Requirement"/> allow both min and max
    /// values to be set for common scenarios.
    /// </summary>
    [DebuggerDisplay("{Requirement} | Parameters: {MinParameters} - {MaxParameters}")]
    public sealed class OptionUsage
    {
        private int _minOccurences;
        private int _maxOccurences = 1;
        private int _minParameters;
        private int _maxParameters;

        public OptionUsage()
        {
            //By default, options are optional... can be omitted or appear exactly once
            Requirement = OptionRequirement.Optional;

            //By default, parameters are not allowed on an option
            MinParameters = 0;
            MaxParameters = 0;
        }

        public int MaxOccurences
        {
            get { return _maxOccurences; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
                _maxOccurences = value;
            }
        }

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
        /// Shortcut to get/set both min and max occurence values.
        /// If min and max values are different, returns null.
        /// If set to null, then the defaults of 0 (min) and 1 (max) are set.
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
        /// Shortcut to get/set both min and max parameter values.
        /// If min and max values are different, returns null.
        /// If set to null, then the defaults of 0 (min) and 0 (max) are set.
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
        /// Shortcut to set the option's occurences based on its requirement.
        /// </summary>
        public OptionRequirement Requirement
        {
            get
            {
                if (MaxOccurences > 0)
                    return MinOccurences > 0 ? OptionRequirement.Required : OptionRequirement.Optional;
                return OptionRequirement.NotAllowed;
            }
            set
            {
                switch (value)
                {
                    case OptionRequirement.Optional:
                        MinOccurences = 0;
                        if (MaxOccurences == 0)
                            MaxOccurences = 1;
                        break;
                    case OptionRequirement.Required:
                        MinOccurences = 1;
                        if (MaxOccurences == 0)
                            MaxOccurences = 1;
                        break;
                    case OptionRequirement.NotAllowed:
                        MinOccurences = MaxOccurences = 0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        public OptionParameterType ParameterType { get; set; }
    }
}
