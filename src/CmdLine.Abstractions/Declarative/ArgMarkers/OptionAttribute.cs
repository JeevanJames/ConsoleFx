﻿// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Marks a property in a <see cref="Command"/> class as an <see cref="Option"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OptionAttribute : Attribute, IArgApplicator<Option>
    {
        //TODO: Add a parameterless ctor. The name of the option will be the property name.

        /// <summary>
        ///     Initializes a new instance of the <see cref="OptionAttribute"/> class with one or more
        ///     option names.
        /// </summary>
        /// <param name="name">The primary name for the option.</param>
        /// <param name="additionalNames">Optional additional names (aliases) for the option.</param>
        public OptionAttribute(string name, params string[] additionalNames)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (name.Trim().Length == 0)
                throw new ArgumentException("Option name cannot be empty or whitespaced.", nameof(name));

            if (additionalNames is null)
                throw new ArgumentNullException(nameof(additionalNames));

            for (int i = 0; i < additionalNames.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(additionalNames[i]))
                {
                    throw new ArgumentException(
                        $"Additional names for option '{name}' has an null, empty or whitespaced value at index {i}.",
                        nameof(additionalNames));
                }
            }

            string[] names = new string[additionalNames.Length + 1];
            names[0] = name;
            if (additionalNames.Length > 0)
                additionalNames.CopyTo(names, index: 1);

            Names = names;
        }

        /// <summary>
        ///     Gets the names of the option.
        /// </summary>
        public IReadOnlyList<string> Names { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the option is required. Defaults to required.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the option can be specified multiple times.
        /// </summary>
        public bool MultipleOccurrences { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the option can have multiple parameters. If
        ///     <c>false</c>, then the option can have only a single parameter.
        /// </summary>
        public bool MultipleParameters { get; set; }

        /// <inheritdoc />
        void IArgApplicator<Option>.Apply(Option arg, PropertyInfo propertyInfo)
        {
            if (MultipleOccurrences && MultipleParameters)
                arg.UsedAsUnlimitedOccurrencesAndParameters(Optional);
            else if (MultipleOccurrences)
                arg.UsedAsUnlimitedOccurrencesAndSingleParameter(Optional);
            else if (MultipleParameters)
                arg.UsedAsSingleOccurrenceAndUnlimitedParameters(Optional);
            else
                arg.UsedAsSingleParameter(Optional);
        }
    }
}
