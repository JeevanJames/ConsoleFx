// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Reflection;

using ConsoleFx.CmdLine.Internals;
using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents a non-option command-line parameter.
    /// </summary>
    [DebuggerDisplay("Argument {Order} [{Validators.Count} validators]")]
    public sealed class Argument : ArgumentOrOption<Argument>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ValidatorCollection _validators;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Argument" /> class.
        /// </summary>
        /// <param name="order">The order in which the argument appears.</param>
        /// <param name="isOptional">Indicates whether the argument is optional.</param>
        /// <param name="maxOccurences">
        ///     Maximum number of occurences of the last argument. Ignored if it is not the last
        ///     argument.
        /// </param>
        public Argument(int order = 0, bool isOptional = false, byte maxOccurences = 1)
        {
            if (maxOccurences < 1)
                throw new ArgumentException("Maximum occurences for the argument should not be less than one.", nameof(maxOccurences));

            Order = order;
            IsOptional = isOptional;
            MaxOccurences = maxOccurences;
        }

        public int Order { get; }

        /// <summary>
        ///     Gets a value indicating whether the argument is optional. Optional arguments can only
        ///     be specified after all the required arguments.
        /// </summary>
        public bool IsOptional { get; }

        /// <summary>
        ///     Gets the maximum number of times the argument can be specified.
        ///     <para/>
        ///     This only applies to the last argument. All other arguments can only be specified once.
        ///     If this is not the last argument, this property is ignored and assumed to be one.
        /// </summary>
        public int MaxOccurences { get; }

        /// <summary>
        ///     Gets the validators for this argument.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        internal ValidatorCollection Validators => _validators ??= new ValidatorCollection();

        /// <inheritdoc/>
        public override Argument AssignTo(string propertyName)
        {
            InternalAssignTo(propertyName);
            return this;
        }

        /// <inheritdoc/>
        public override Argument UnderGroups(params int[] groups)
        {
            InternalUnderGroups(groups);
            return this;
        }

        /// <inheritdoc/>
        public override Argument DefaultsTo(Func<object> setter)
        {
            InternalDefaultsTo(setter);
            return this;
        }

        /// <inheritdoc/>
        public override Argument DefaultsTo(object defaultValue)
        {
            InternalDefaultsTo(defaultValue);
            return this;
        }

        /// <inheritdoc />
        public override Argument FormatAs(Func<string, string> formatter)
        {
            InternalFormatAs(formatter);
            return this;
        }

        /// <inheritdoc/>
        public override Argument FormatAs(string formatStr)
        {
            InternalFormatAs(formatStr);
            return this;
        }

        /// <inheritdoc/>
        public override Argument TypeAs(Type type, Converter<string, object> converter = null)
        {
            InternalTypeAs(type, converter);
            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        ///     Specifies the type to convert the option parameters, with an optional custom converter.
        ///     If a custom converter is not specified, the type's type converter will be used.
        /// </summary>
        /// <typeparam name="T">The type to convert the option parameters to.</typeparam>
        /// <param name="converter">Optional custom converter.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        public override Argument TypeAs<T>(Converter<string, T> converter = null)
        {
            InternalTypeAs(typeof(T), converter);
            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        ///     Specifies one or more validators to validate the argument.
        /// </summary>
        /// <param name="validators">The validators to use to validate the argument.</param>
        /// <returns>The same instance of the <see cref="Argument"/> object to allow for fluent syntax.</returns>
        public override Argument ValidateWith(params Validator[] validators)
        {
            foreach (Validator validator in validators)
                Validators.Add(validator);
            return this;
        }

        internal ArgumentValueType GetValueType()
        {
            return MaxOccurences > 1 ? ArgumentValueType.List : ArgumentValueType.Object;
        }

        /// <inheritdoc />
        internal override void ValidateUnderlyingProperty()
        {
            ArgumentValueType expectedValueType = GetValueType();
            switch (expectedValueType)
            {
                case ArgumentValueType.Object:
                    if (AssignedProperty.PropertyType != typeof(string))
                        TypeAs(AssignedProperty.PropertyType);
                    break;
                case ArgumentValueType.List:
                    Type itemType = AssignedProperty.GetCollectionItemType();
                    if (itemType is null)
                        throw new ParserException(-1, $"Type for property {AssignedProperty.Name} in command {AssignedProperty.DeclaringType} should be a generic collection type like IEnumerable<T> or List<T>.");
                    if (itemType != typeof(string))
                        TypeAs(itemType);
                    break;
                default:
                    throw new NotSupportedException($"Unexpected ArgumentValueType value of {expectedValueType}.");
            }
        }
    }

    /// <summary>
    ///     The type of the resolved value of an option.
    ///     <para/>
    ///     Decided based on the usage specs of the option.
    /// </summary>
    internal enum ArgumentValueType
    {
        /// <summary>
        ///     An object of any type.
        /// </summary>
        Object,

        /// <summary>
        ///     A list of any type. Can you be used for the last argument.
        /// </summary>
        List,
    }
}
