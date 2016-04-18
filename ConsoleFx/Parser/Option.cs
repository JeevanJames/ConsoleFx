using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace ConsoleFx.Parser
{
    /// <summary>
    ///     Represents a commandline switch parameter.
    /// </summary>
    [DebuggerDisplay("Option: {Name}")]
    public sealed class Option : MetadataObject
    {
        public Option(string name)
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

        public int Order { get; set; }
        public OptionUsage Usage { get; } = new OptionUsage();
        public OptionParameterValidators Validators { get; }
        public OptionParameterFormatter Formatter { get; set; }
        public Type Type { get; set; }
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
