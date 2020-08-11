#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

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

using ConsoleFx.CmdLine.Validators.Bases;

namespace ConsoleFx.CmdLine
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents an options arg.
    /// </summary>
    [DebuggerDisplay("Option: {" + nameof(Name) + "}")]
    public sealed partial class Option : ArgumentOrOption<Option>
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
        internal OptionUsage Usage { get; } = new OptionUsage();

        /// <summary>
        ///     Gets the collection of validators that can validate some or all of the option's parameters.
        /// </summary>
        internal OptionParameterValidators Validators { get; }

        /// <inheritdoc/>
        public override Option AssignTo(string propertyName)
        {
            InternalAssignTo(propertyName);
            return this;
        }

        /// <inheritdoc/>
        public override Option UnderGroups(params int[] groups)
        {
            InternalUnderGroups(groups);
            return this;
        }

        /// <inheritdoc/>
        public override Option DefaultsTo(Func<object> setter)
        {
            InternalDefaultsTo(setter);
            return this;
        }

        /// <inheritdoc/>
        public override Option DefaultsTo(object defaultValue)
        {
            InternalDefaultsTo(defaultValue);
            return this;
        }

        /// <inheritdoc/>
        public override Option FormatAs(Func<string, string> formatter)
        {
            InternalFormatAs(formatter);
            return this;
        }

        /// <inheritdoc/>
        public override Option FormatAs(string formatStr)
        {
            InternalFormatAs(formatStr);
            return this;
        }

        /// <inheritdoc/>
        public override Option TypeAs(Type type, Converter<string, object> converter = null)
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
        public override Option TypeAs<T>(Converter<string, T> converter = null)
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

        /// <inheritdoc/>
        /// <summary>
        ///     Specifies one or more validators that will be used to validate the option's parameter
        ///     values.
        /// </summary>
        /// <param name="validators">One or more validators.</param>
        /// <returns>The instance of the <see cref="Option"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the specified validators array is null.</exception>
        /// <exception cref="ArgumentException">Thrown if no validators are specified or if any of the specified validators is null.</exception>
        public override Option ValidateWith(params Validator[] validators)
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

        /// <summary>
        ///     Figures out the expected type of an option's value, based on the option's usage details.
        /// </summary>
        /// <returns>An <see cref="OptionValueType"/> enum specifying the expected value type.</returns>
        internal OptionValueType GetValueType()
        {
            // If parameters are not allowed on the option...
            if (Usage.ParameterRequirement == OptionParameterRequirement.NotAllowed)
            {
                // If the option can occur more than once, it's value will be an integer specifying
                // the number of occurences.
                if (Usage.MaxOccurrences > 1)
                    return OptionValueType.Count;

                // If the option can occur not more than once, it's value will be a bool indicating
                // whether it was specified or not.
                return OptionValueType.Flag;
            }

            // If the option can have multiple parameter values (either because the MaxParameters usage
            // is greater than one or because MaxParameters is one but MaxOccurences is greater than
            // one), then the option's value is an IList<Type>.
            if (Usage.MaxParameters > 1 || (Usage.MaxParameters > 0 && Usage.MaxOccurrences > 1))
                return OptionValueType.List;

            // If the option only has one parameter specified, then the option's value is a string.
            if (Usage.MaxParameters == 1 && Usage.MaxOccurrences == 1)
                return OptionValueType.Object;

            //TODO: Change this to an internal parser exception.
            throw new InvalidOperationException("Should never reach here.");
        }
    }

    // INamedObject implementation
    public sealed partial class Option : INamedObject
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly INamedObject _namedObject = new NamedObjectImpl();

        public string Name => _namedObject.Name;

        public IEnumerable<string> AlternateNames => _namedObject.AlternateNames;

        public IEnumerable<string> AllNames => _namedObject.AllNames;

        public void AddName(string name, bool caseSensitive = false)
        {
            _namedObject.AddName(name, caseSensitive);
        }

        public bool HasName(string name)
        {
            return _namedObject.HasName(name);
        }
    }

    /// <summary>
    ///     The type of the resolved value of an option.
    ///     <para/>
    ///     Decided based on the usage specs of the option.
    /// </summary>
    internal enum OptionValueType
    {
        /// <summary>
        ///     An object of any type. Used when there is a single parameter.
        /// </summary>
        Object,

        /// <summary>
        ///     A list of any type. Used when there are more than one possible parameters.
        /// </summary>
        List,

        /// <summary>
        ///     A count of the number of occurences of the option. Used when the option has no
        ///     parameters, but multiple occurences.
        /// </summary>
        Count,

        /// <summary>
        ///     A boolean flag indicating whether the option was specified. Used when the option can
        ///     occur only once and have no occurences.
        /// </summary>
        Flag,
    }
}
