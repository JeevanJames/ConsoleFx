// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ConsoleFx.CmdLine.Internals;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Marks a property in a <see cref="Command"/> class as an <see cref="Option"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OptionAttribute : ArgAttribute, IArgApplicator<Option>
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
            Names = ConstructNames(name, additionalNames);
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

        public string HelpParamName { get; set; }

        public Type HelpParamNameResourceType { get; set; }

        public string HelpParamNameResourceName { get; set; }

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

        /// <inheritdoc />
        public override IEnumerable<ArgMetadata> GetMetadata()
        {
            return base.GetMetadata().Concat(new[]
            {
                new ArgMetadata(HelpMetadataKey.OptionParameterName,
                    ResolveResourceString(HelpParamName, HelpParamNameResourceType, HelpParamNameResourceName,
                        required: false)),
            });
        }

        /// <inheritdoc />
        protected override IEnumerable<Type> GetApplicableArgTypes()
        {
            return CommonApplicableArgTypes.Option;
        }
    }
}
