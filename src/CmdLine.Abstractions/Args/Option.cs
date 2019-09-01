#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2019 Jeevan James

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

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents an options arg.
    /// </summary>
    [DebuggerDisplay("Option: {" + nameof(Name) + "}")]
    public sealed class Option : ArgumentOrOption<Option>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Option" /> class with the specified identifying names.
        /// </summary>
        /// <param name="names">
        ///     One or more unique names to identify the option. All names added will be not be case-sensitive. In case
        ///     you require case-sensitive option names, use the overloaded constructor.
        /// </param>
        public Option(params string[] names)
        {
            foreach (string name in names)
                AddName(name);
            Validators = new OptionParameterValidators(this);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Option" /> class with the specified identifying names and
        ///     specifies whether the names are case sensitive.
        /// </summary>
        /// <param name="caseSensitive">Indicates whether the specified <paramref name="names" /> are case sensitive.</param>
        /// <param name="names">One or more unique names to identify the option.</param>
        public Option(bool caseSensitive, params string[] names)
        {
            foreach (string name in names)
                AddName(name, caseSensitive);
            Validators = new OptionParameterValidators(this);
        }

        /// <summary>
        ///     Gets the various usage options for the option and its parameters, including the minimum and maximum
        ///     allowed occurrences of the option itself, and also the minimum and maximum allowed number of parameters
        ///     that can be specified for each occurence.
        /// </summary>
        public OptionUsage Usage { get; } = new OptionUsage();

        /// <summary>
        ///     Gets the collection of validators that can validate some or all of the option's parameters.
        /// </summary>
        public OptionParameterValidators Validators { get; }

        public sealed override Option UnderGroups(params int[] groups)
        {
            InternalUnderGroups(groups);
            return this;
        }

        public sealed override Option DefaultsTo(Func<object> setter)
        {
            InternalDefaultsTo(setter);
            return this;
        }

        public sealed override Option DefaultsTo(object defaultValue)
        {
            InternalDefaultsTo(defaultValue);
            return this;
        }

        public sealed override Option FormatAs(Func<string, string> formatter)
        {
            InternalFormatAs(formatter);
            return this;
        }

        public sealed override Option FormatAs(string formatStr)
        {
            InternalFormatAs(formatStr);
            return this;
        }

        public sealed override Option TypeAs(Type type, Converter<string, object> converter = null)
        {
            InternalTypeAs(type, converter);
            return this;
        }

        /// <summary>
        ///     Specifies the type to convert the option parameters, with an optional custom converter.
        ///     If a custom converter is not specified, the type's type converter will be used.
        /// </summary>
        /// <typeparam name="T">The type to convert the option parameters to.</typeparam>
        /// <param name="converter">Optional custom converter.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        public sealed override Option TypeAs<T>(Converter<string, T> converter = null)
        {
            InternalTypeAs<T>(typeof(T), converter);
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
            if (usageSetter is null)
                throw new ArgumentNullException(nameof(usageSetter));
            usageSetter(Usage);
            return this;
        }

        /// <summary>
        ///     Specifies one or more validators that will be used to validate the option's parameter
        ///     values.
        /// </summary>
        /// <param name="validators">One or more validators.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the specified validators array is null.</exception>
        /// <exception cref="ArgumentException">Thrown if no validators are specified or if any of the specified validators is null.</exception>
        public override sealed Option ValidateWith(params Validator[] validators)
        {
            if (validators is null)
                throw new ArgumentNullException(nameof(validators));
            if (validators.Length == 0)
                throw new ArgumentException(Errors.Option_ValidatorsNotSpecified, nameof(validators));
            for (int i = 0; i < validators.Length; i++)
            {
                Validator validator = validators[i];
                if (validator is null)
                    throw new ArgumentException(string.Format(Errors.Option_ValidatorIsNull, i), nameof(validators));
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
            if (validators is null)
                throw new ArgumentNullException(nameof(validators));
            if (validators.Length == 0)
                throw new ArgumentException(Errors.Option_ValidatorsNotSpecified, nameof(validators));
            for (var i = 0; i < validators.Length; i++)
            {
                Validator validator = validators[i];
                if (validator is null)
                    throw new ArgumentException(string.Format(Errors.Option_ValidatorIsNull, i), nameof(validators));
                Validators.Add(parameterIndex, validator);
            }

            return this;
        }
    }
}
