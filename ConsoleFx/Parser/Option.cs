#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015-2016 Jeevan James

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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using ConsoleFx.Parser.Validators;

namespace ConsoleFx.Parser
{
    /// <summary>
    ///     Represents a commandline switch parameter.
    /// </summary>
    [DebuggerDisplay("Option: {Name}")]
    public sealed class Option : MetadataObject
    {
        public Option(string name, string shortName = null, bool caseSensitive = false, int order = 0, object @default = null)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Validators = new OptionParameterValidators(this);
        }

        /// <summary>
        ///     Name of the option.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Optional alternative name for the option, normally a shorter version.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        ///     Specifies whether the option name and short name are case sensitive.
        /// </summary>
        public bool CaseSensitive { get; set; }

        /// <summary>
        ///     Priority order for processing the option. Options with higher <see cref="Order" /> values are processed before
        ///     those with lower values.
        /// </summary>
        //TODO: Need to implement this
        public int Order { get; set; }

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
        ///     Returns whether the option's name or short name matches the specified name.
        /// </summary>
        /// <param name="name">The name to check against.</param>
        /// <returns>True, if either the name or short name matches.</returns>
        public bool HasName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            StringComparison comparison = CaseSensitive
                ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            if (name.Equals(Name, comparison))
                return true;
            if (!string.IsNullOrEmpty(ShortName) && name.Equals(ShortName, comparison))
                return true;
            return false;
        }

        public Option FormatParamsAs(OptionParameterFormatter formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));
            Formatter = formatter;
            return this;
        }

        public Option FormatParamsAs(string formatStr)
        {
            if (string.IsNullOrWhiteSpace(formatStr))
                throw new ArgumentException(@"The format string cannot be empty or blank.", nameof(formatStr));
            Formatter = value => string.Format(formatStr, value);
            return this;
        }

        public Option ParamsOfType<T>(Converter<string, T> converter = null)
        {
            Type = typeof(T);
            if (converter != null)
                TypeConverter = value => converter(value);
            return this;
        }

        public Option UsedAs(Action<OptionUsage> usageSetter)
        {
            if (usageSetter == null)
                throw new ArgumentNullException(nameof(usageSetter));
            usageSetter(Usage);
            return this;
        }

        public Option ValidateWith(params Validator[] validators)
        {
            foreach (Validator validator in validators)
                Validators.Add(validator);
            return this;
        }

        public Option ValidateWith(Func<string, bool> customValidator) =>
            ValidateWith(new CustomValidator(customValidator));

        public Option ValidateWith(int parameterIndex, params Validator[] validators)
        {
            foreach (Validator validator in validators)
                Validators.Add(parameterIndex, validator);
            return this;
        }
    }

    /// <summary>
    ///     Delegate that is executed for every option that is specified on the command line.
    /// </summary>
    /// <param name="parameters">The list of parameters specified for the option.</param>
    public delegate void OptionHandler(IReadOnlyList<string> parameters);

    public delegate string OptionParameterFormatter(string value);

    /// <summary>
    ///     Represents a collection of options. Note: This is not a keyed collection because the key
    ///     can be either the name or short name.
    /// </summary>
    public sealed class Options : Collection<Option>
    {
        public Option this[string name]
        {
            get
            {
                return this.FirstOrDefault(option => {
                    StringComparison comparison = option.CaseSensitive
                        ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                    if (name.Equals(option.Name, comparison))
                        return true;
                    if (!string.IsNullOrEmpty(option.ShortName) && name.Equals(option.ShortName, comparison))
                        return true;
                    return false;
                });
            }
        }

        /// <summary>
        ///     Prevents duplicate options from being inserted.
        /// </summary>
        /// <param name="index">Location to insert the new option.</param>
        /// <param name="item">Option to insert.</param>
        protected override void InsertItem(int index, Option item)
        {
            if (this.Any(DuplicateCheck(item)))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Messages.OptionAlreadyExists, item.Name), nameof(item));
            }
            base.InsertItem(index, item);
        }

        /// <summary>
        ///     Prevents duplicate options from being set.
        /// </summary>
        /// <param name="index">Location to set the new option.</param>
        /// <param name="item">Option to set.</param>
        protected override void SetItem(int index, Option item)
        {
            if (this.Any(DuplicateCheck(item)))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Messages.OptionAlreadyExists, item.Name), nameof(item));
            }
            base.SetItem(index, item);
        }

        /// <summary>
        ///     Returns a delegate that can check whether the passed option is already available in the
        ///     collection. Used whenever options are added or set in the collection.
        /// </summary>
        /// <param name="option">Option that is being set.</param>
        /// <returns>True if the option already exists in the collection. Otherwise false.</returns>
        //TODO: Check if this method is being used.
        private static Func<Option, bool> DuplicateCheck(Option option) => opt => opt.HasName(option.Name);
    }
}
