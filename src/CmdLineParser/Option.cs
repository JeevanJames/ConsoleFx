﻿#region --- License & Copyright Notice ---
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

using ConsoleFx.CmdLineParser.Validators;

namespace ConsoleFx.CmdLineParser
{
    /// <summary>
    ///     Represents an options arg.
    /// </summary>
    [DebuggerDisplay("Option: {Name}")]
    public sealed class Option : Arg
    {
        public Option(params string[] names)
        {
            foreach (string name in names)
                AddName(name);
            Validators = new OptionParameterValidators(this);
        }

        public Option(bool caseSensitive, params string[] names)
        {
            foreach (string name in names)
                AddName(name, caseSensitive);
            Validators = new OptionParameterValidators(this);
        }

        /// <summary>
        ///     Various usage options for the option and its parameters, including the minimum and maximum allowed occurences of
        ///     the option itself, and also the minimum and maximum allowed number of parameters that can be specified for each
        ///     occurence.
        /// </summary>
        public OptionUsage Usage { get; } = new OptionUsage();

        /// <summary>
        ///     Collection of validators that can validate some or all of the option's parameters.
        /// </summary>
        public OptionParameterValidators Validators { get; }

        /// <summary>
        ///     Optional delegate that allows a option parameter value to be custom formatted.
        ///     During parsing, the formatting is performed before any type conversion.
        /// </summary>
        public OptionParameterFormatter Formatter { get; set; }

        /// <summary>
        ///     The type that the option parameters should be converted to. If a <see cref="TypeConverter" /> is specified, then it
        ///     is used to perform the type conversion, otherwise the framework looks for a default string-to-type type converter
        ///     for the expected type. If a type converter is not found, an exception is thrown during the parsing.
        /// </summary>
        /// <remarks>
        ///     This type applies to all parameters. In case different parameters are to be converted to different types, the
        ///     conversion must happen outside the ConsoleFx framework.
        /// </remarks>
        public Type Type { get; set; }

        /// <summary>
        ///     Optional converter to convert a string parameter value to the actual <see cref="Type" />.
        /// </summary>
        public Converter<string, object> TypeConverter { get; set; }

        /// <summary>
        ///     Assigns a parameter formatter delegate to the option, which can be used to format the
        ///     option's parameter value.
        /// </summary>
        /// <param name="formatter">Parameter formatter delegate</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the parameter formatter delegate is null.</exception>
        public Option FormatParamsAs(OptionParameterFormatter formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));
            Formatter = formatter;
            return this;
        }

        /// <summary>
        ///     Assigns a format string to the option, which will be used to format the option's
        ///     parameter value.
        /// </summary>
        /// <param name="formatStr">The format string, where the first format placeholder ({0}) represents the parameter value.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the format string is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the format string does not contain a format placeholder ({0}).</exception>
        public Option FormatParamsAs(string formatStr)
        {
            if (formatStr == null)
                throw new ArgumentNullException(nameof(formatStr));
            if (!formatStr.Contains("{0}"))
                throw new ArgumentException("The specified format string should contain a format placeholder ({0}) to specify where the parameter value is inserted.", nameof(formatStr));
            Formatter = value => string.Format(formatStr, value);
            return this;
        }

        /// <summary>
        ///     Specifies the type to convert the option parameters, with an optional custom converter.
        ///     If a custom converter is not specified, the type's type converter will be used.
        /// </summary>
        /// <typeparam name="T">The type to convert the option parameters to.</typeparam>
        /// <param name="converter">Optional custom converter.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        public Option ParamsOfType<T>(Converter<string, T> converter = null)
        {
            Type = typeof(T);
            if (converter != null)
                TypeConverter = value => converter(value);
            return this;
        }

        /// <summary>
        ///     Specifies the rules of how the option is to be used - minimum and maximum number of
        ///     occurences, minimum and maximum number of parameters allowed, etc.
        /// </summary>
        /// <param name="usageSetter">Delegate that is used to specify the option usage rules.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the specified delegate is null.</exception>
        public Option UsedAs(Action<OptionUsage> usageSetter)
        {
            if (usageSetter == null)
                throw new ArgumentNullException(nameof(usageSetter));
            usageSetter(Usage);
            return this;
        }

        /// <summary>
        ///     Specifies that the option is to be used as a flag. If the option is specified, then
        ///     its value is <c>true</c>, otherwise it is <c>false</c>.
        /// </summary>
        /// <param name="optional"></param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        public Option UsedAsFlag(bool optional = true)
        {
            Usage.SetParametersNotAllowed();
            Usage.MinOccurences = optional ? 0 : 1;
            Usage.MaxOccurences = 1;
            return this;
        }

        /// <summary>
        ///     Specifies that the option is to have only a single parameter. This means that not more
        ///     than one occurence of the option and only one parameter for the option.
        /// </summary>
        /// <param name="optional">If <c>true</c>, then the option does not need to be specified.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        public Option UsedAsSingleParameter(bool optional = true)
        {
            Usage.SetParametersRequired();
            Usage.MinOccurences = optional ? 0 : 1;
            Usage.MaxOccurences = 1;
            return this;
        }

        /// <summary>
        /// Specifies one or more validators that will be used to validate the option's parameter values.
        /// </summary>
        /// <param name="validators">One or more validators.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the specified validators array is null.</exception>
        /// <exception cref="ArgumentException">Thrown if no validators are specified or if any of the specified validators is null.</exception>
        public Option ValidateWith(params Validator[] validators)
        {
            if (validators == null)
                throw new ArgumentNullException(nameof(validators));
            if (validators.Length == 0)
                throw new ArgumentException("Specify at least one validator.", nameof(validators));
            for (int i = 0; i < validators.Length; i++)
            {
                Validator validator = validators[i];
                if (validator == null)
                    throw new ArgumentException($"Validator at index {i} is null.", nameof(validators));
                Validators.Add(validator);
            }
            return this;
        }

        /// <summary>
        ///     Specifies one or more validators that will be used to validate the option's parameter
        ///     at the specified index.
        /// </summary>
        /// <param name="parameterIndex">The index of the option parameter to validate.</param>
        /// <param name="validators">One or more validators.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the specified validators array is null.</exception>
        /// <exception cref="ArgumentException">Thrown if no validators are specified or if any of the specified validators is null.</exception>
        public Option ValidateWith(int parameterIndex, params Validator[] validators)
        {
            if (validators == null)
                throw new ArgumentNullException(nameof(validators));
            if (validators.Length == 0)
                throw new ArgumentException("Specify at least one validator.", nameof(validators));
            for (int i = 0; i < validators.Length; i++)
            {
                Validator validator = validators[i];
                if (validator == null)
                    throw new ArgumentException($"Validator at index {i} is null.", nameof(validators));
                Validators.Add(parameterIndex, validator);
            }
            return this;
        }
    }

    public delegate string OptionParameterFormatter(string value);

    /// <summary>
    ///     Represents a collection of options. Note: This is not a keyed collection because the key
    ///     can be either the name or short name.
    /// </summary>
    public sealed class Options : Args<Option>
    {
        /// <summary>
        ///     Performs all combinations of comparisons between the <see cref="Arg.Name"/>
        ///     and <see cref="Option.ShortName"/>.
        /// </summary>
        /// <inheritdoc/>
        protected override bool ObjectsMatch(Option obj1, Option obj2) =>
            obj1.HasName(obj2.Name);

        protected override bool NamesMatch(string name, Option item) =>
            item.HasName(name);

        protected override string GetDuplicateErrorMessage(string name) =>
            string.Format(CultureInfo.CurrentCulture, Messages.OptionAlreadyExists, name);
    }
}