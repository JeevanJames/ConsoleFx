﻿// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Reflection;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Rules for specifying an option on the command line. This includes number of occurences of
    ///     the option (defaults: 0 min and 1 max) and number of parameters (default: 0).
    ///     <para/>
    ///     Additional shortcut properties (<see cref="ExpectedOccurrences" />,
    ///     <see cref="ExpectedParameters" /> and <see cref="Requirement" /> allow both min and max
    ///     values to be set for common scenarios.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    [DebuggerDisplay("{Requirement} | Parameters: {MinParameters} - {MaxParameters}")]
    public sealed partial class OptionUsage : Attribute, IArgApplicator<Option>
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

            // An option should allow at least one occurence.
            // Allowing a max of 0 occurrences is the same as saying that the option should never be used.
            //TODO: Change the exception message to something more appropriate.
            set => _maxOccurrences = value >= 1 ? value
                : throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
        }

        /// <summary>
        ///     Gets or sets the minimum allowed occurrences of the option.
        /// </summary>
        public int MinOccurrences
        {
            get => _minOccurrences;
            set => _minOccurrences = value >= 0 ? value
                : throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
        }

        /// <summary>
        ///     Gets or sets the expected number of occurrences of the property.
        ///     <para/>
        ///     Shortcut to get/set both min and max occurence values.
        ///     <para/>
        ///     If min and max values are different, returns null.
        ///     <para/>
        ///     If set to null, then the defaults of 0 (min) and 1 (max) are set.
        /// </summary>
        public int? ExpectedOccurrences
        {
            get => MinOccurrences == MaxOccurrences ? MinOccurrences : (int?)null;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), Messages.OccurenceParameterValueNegative);
                _minOccurrences = value ?? Defaults.MinOccurrences;
                _maxOccurrences = value ?? Defaults.MaxOccurrences;
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
                    requirement = requirement == OptionRequirement.Required
                        ? OptionRequirement.RequiredUnlimited
                        : OptionRequirement.OptionalUnlimited;
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
        ///     Gets or sets the maximum allowed number of parameters for the option.
        /// </summary>
        public int MaxParameters
        {
            get => _maxParameters;
            set => _maxParameters = value; //TODO: Validate value
        }

        /// <summary>
        ///     Gets or sets the minimum allowed number of parameters for the option.
        /// </summary>
        public int MinParameters
        {
            get => _minParameters;
            set => _minParameters = value; //TODO: Validate value
        }

        /// <summary>
        ///     Gets or sets the parameter occurrences based on its requirement.
        /// </summary>
        public OptionParameterRequirement ParameterRequirement
        {
            get => MinParameters == 0 && MaxParameters == 0
                ? OptionParameterRequirement.NotAllowed
                : OptionParameterRequirement.Required;

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

        void IArgApplicator<Option>.Apply(Option arg, PropertyInfo propertyInfo)
        {
            arg.Usage.MinOccurrences = MinOccurrences;
            arg.Usage.MaxOccurrences = MaxOccurrences;
            arg.Usage.MinParameters = MinParameters;
            arg.Usage.MaxParameters = MaxParameters;
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
