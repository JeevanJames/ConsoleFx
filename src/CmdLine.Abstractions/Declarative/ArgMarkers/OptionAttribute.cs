// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;

using ConsoleFx.CmdLine.Internals;

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

        public CommonOptionUsage Usage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the option is optional.
        /// </summary>
        public bool Optional { get; set; }

        void IArgApplicator<Option>.Apply(Option arg, PropertyInfo propertyInfo)
        {
            switch (Usage)
            {
                case CommonOptionUsage.SingleParameter:
                    arg.UsedAsSingleParameter(Optional);
                    break;
                case CommonOptionUsage.SingleOccurrenceUnlimitedParameters:
                    arg.UsedAsSingleOccurrenceAndUnlimitedParameters(Optional);
                    break;
                case CommonOptionUsage.UnlimitedOccurrencesSingleParameter:
                    arg.UsedAsUnlimitedOccurrencesAndSingleParameter(Optional);
                    break;
                case CommonOptionUsage.UnlimitedOccurrencesAndParameters:
                    arg.UsedAsUnlimitedOccurrencesAndParameters(Optional);
                    break;
            }

            OptionValueType expectedValueType = arg.GetValueType();
            switch (expectedValueType)
            {
                case OptionValueType.Count:
                    if (propertyInfo.PropertyType != typeof(int))
                        throw new ParserException(-1, $"Type for property {propertyInfo.Name} in command {propertyInfo.DeclaringType} should be an integer.");
                    break;
                case OptionValueType.Flag:
                    if (propertyInfo.PropertyType != typeof(bool))
                        throw new ParserException(-1, $"Type for property {propertyInfo.Name} in command {propertyInfo.DeclaringType} should be an boolean.");
                    break;
                case OptionValueType.Object:
                    if (propertyInfo.PropertyType != typeof(string))
                        arg.TypeAs(propertyInfo.PropertyType);
                    break;
                case OptionValueType.List:
                    Type itemType = propertyInfo.GetCollectionItemType();
                    if (itemType is null)
                        throw new ParserException(-1, $"Type for property {propertyInfo.Name} in command {propertyInfo.DeclaringType} should be a generic collection type like IEnumerable<T> or List<T>.");
                    if (itemType != typeof(string))
                        arg.TypeAs(itemType);
                    break;
                default:
                    throw new NotSupportedException($"Unexpected OptionValueType value of {expectedValueType}.");
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MultiOptionAttribute : Attribute, IArgApplicator<Option>
    {
        public bool Optional { get; set; }

        public bool MultipleOccurrences { get; set; }

        public bool MultipleParameters { get; set; }

        /// <inheritdoc />
        public void Apply(Option arg, PropertyInfo propertyInfo)
        {
            if (MultipleOccurrences && MultipleParameters)
                arg.UsedAsUnlimitedOccurrencesAndParameters(Optional);
            else if (MultipleOccurrences)
                arg.UsedAsUnlimitedOccurrencesAndSingleParameter(Optional);
            else if (MultipleParameters)
                arg.UsedAsSingleOccurrenceAndUnlimitedParameters(Optional);
        }
    }
}
