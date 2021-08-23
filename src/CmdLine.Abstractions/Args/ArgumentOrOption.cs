// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Base class for <see cref="Argument"/> and <see cref="Option"/>, as they share a lot of
    ///     common properties that are not present in <see cref="Command"/>.
    /// </summary>
    /// <typeparam name="TArg">
    ///     The type of <see cref="Arg"/>; can be either <see cref="Argument"/> and <see cref="Option"/>.
    /// </typeparam>
    public abstract class ArgumentOrOption<TArg> : Arg
        where TArg : Arg
    {
        private readonly List<int> _groups = new List<int> { 0 };

        /// <summary>
        ///     Gets or sets the reference to the property in the containing <see cref="Command"/>
        ///     instance, which should be set with the parsed value for this arg.
        /// </summary>
        internal PropertyInfo AssignedProperty { get; set; }

        internal string AssignedPropertyName { get; set; }

        internal IReadOnlyList<int> Groups => _groups;

        /// <summary>
        ///     Gets or sets the optional delegate to return the arg's default value, if it is not set.
        /// </summary>
        internal Func<object> DefaultSetter { get; set; }

        /// <summary>
        ///     Gets or sets the optional delegate that allows an arg value to be custom formatted.
        ///     <para />
        ///     During parsing, the formatting is performed before any type conversion.
        /// </summary>
        internal Func<string, string> Formatter { get; set; }

        /// <summary>
        ///     Gets or sets the type that the arg's values should be converted to. If a
        ///     <see cref="TypeConverter" /> is specified, then it is used to perform the type
        ///     conversion, otherwise the framework looks for a default string-to-type type converter
        ///     for the expected type. If a type converter is not found, an exception is thrown during
        ///     the parsing.
        /// </summary>
        /// <remarks>
        ///     In the case of <see cref="Option"/>, this type applies to all parameters. In case
        ///     different parameters are to be converted to different types, the conversion must happen
        ///     outside the ConsoleFx framework.
        /// </remarks>
        internal Type Type { get; set; }

        /// <summary>
        ///     Gets or sets the optional converter to convert a string arg value to the actual
        ///     <see cref="Type" />.
        /// </summary>
        internal Converter<string, object> TypeConverter { get; set; }

        public abstract TArg AssignTo(string propertyName);

        public abstract TArg UnderGroups(params int[] groups);

        public abstract TArg DefaultsTo(Func<object> setter);

        public abstract TArg DefaultsTo(object defaultValue);

        /// <summary>
        ///     Assigns the <see cref="Formatter"/> property, which can be used to format the arg's
        ///     string value.
        /// </summary>
        /// <param name="formatter">The formatter delegate.</param>
        /// <returns>The instance of the <typeparamref name="TArg"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the formatter delegate is <c>null</c>.
        /// </exception>
        public abstract TArg FormatAs(Func<string, string> formatter);

        /// <summary>
        ///     Assigns a format string to the arg, which will be used to format the arg's value.
        /// </summary>
        /// <param name="formatStr">
        ///     The format string, where the first format placeholder ({0}) represents the arg's value.
        /// </param>
        /// <returns>The instance of the <typeparamref name="TArg"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the format string is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if the format string does not contain a format placeholder ({0}).
        /// </exception>
        public abstract TArg FormatAs(string formatStr);

        public abstract TArg TypeAs(Type type, Converter<string, object> converter = null);

        /// <summary>
        ///     Specifies the type to convert the option parameters, with an optional custom converter.
        ///     If a custom converter is not specified, the type's type converter will be used.
        /// </summary>
        /// <typeparam name="T">The type to convert the option parameters to.</typeparam>
        /// <param name="converter">Optional custom converter.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        public abstract TArg TypeAs<T>(Converter<string, T> converter = null);

        /// <summary>
        ///     Specifies one or more validators that will be used to validate the arg's values.
        /// </summary>
        /// <param name="validators">One or more validators.</param>
        /// <returns>The instance of the <typeparamref name="TArg"/>.</returns>
        public abstract TArg ValidateWith(params Validator[] validators);

        protected void InternalAssignTo(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Property name cannot be null or whitespace.", nameof(propertyName));
            AssignedPropertyName = propertyName;
        }

        protected void InternalDefaultsTo(Func<object> setter)
        {
            if (setter is null)
                throw new ArgumentNullException(nameof(setter));
            DefaultSetter = setter;
        }

        protected void InternalDefaultsTo(object value)
        {
            DefaultSetter = () => value;
        }

        protected void InternalFormatAs(Func<string, string> formatter)
        {
            if (formatter is null)
                throw new ArgumentNullException(nameof(formatter));
            Formatter = formatter;
        }

        protected void InternalFormatAs(string formatStr)
        {
            if (formatStr is null)
                throw new ArgumentNullException(nameof(formatStr));
            if (!formatStr.Contains("{0}"))
                throw new ArgumentException(Errors.Option_MissingPlaceholderInFormatString, nameof(formatStr));

            Formatter = value =>
                string.Format(formatStr, value ?? string.Empty) ?? string.Empty;
        }

        protected void InternalTypeAs<T>(Type type, Converter<string, T> converter = null)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            Type = type;
            if (converter != null)
                TypeConverter = value => converter(value);
        }

        protected void InternalUnderGroups(IReadOnlyList<int> groups)
        {
            if (groups is null)
                throw new ArgumentNullException(nameof(groups));
            if (groups.Count == 0)
                return;

            _groups.Clear();
            _groups.AddRange(groups);
        }
    }
}
