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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Base class for command-line args, such as commands, arguments and options.
    ///     <para />
    ///     Base class for <see cref="Option" />, <see cref="Argument" /> and <see cref="Command" />.
    ///     <para />
    ///     This class provides two capabilities. One is the collection of names that identify the arg and the other is
    ///     additional metadata that can be used by certain frameworks to add more details for an arg.
    /// </summary>
    public abstract partial class Arg
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, bool> _names = new Dictionary<string, bool>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="Arg" /> class.
        /// </summary>
        protected Arg()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Arg" /> class with one or more names.
        /// </summary>
        /// <param name="names">One or more names for the arg. The first name is considered primary.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="names" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="names" /> collection is empty.</exception>
        protected Arg(IDictionary<string, bool> names)
        {
            if (names is null)
                throw new ArgumentNullException(nameof(names));
            if (names.Count == 0)
                throw new ArgumentException(Errors.Arg_No_names_specified, nameof(names));
            foreach (KeyValuePair<string, bool> kvp in names)
                AddName(kvp.Key, kvp.Value);

            _names = new Dictionary<string, bool>(names);
        }

        /// <summary>
        ///     Adds a new name for the arg.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="caseSensitive">Indicates whether the name is case-sensitive.</param>
        /// <returns>An instance to the same <see cref="Arg" />, to allow for a fluent syntax.</returns>
        public Arg AddName(string name, bool caseSensitive = false)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (!NamePattern.IsMatch(name))
                throw new ArgumentException(string.Format(Errors.Arg_Invalid_name, name), nameof(name));
            _names.Add(name, caseSensitive);
            return this;
        }

        /// <summary>
        ///     Checks whether any of the args' names matches the specified name.
        /// </summary>
        /// <param name="name">The name to check against.</param>
        /// <returns><c>true</c>, if the specified name matches any of the args' names, otherwise <c>false</c>.</returns>
        public bool HasName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            return _names.Any(kvp =>
            {
                var comparison = kvp.Value
                    ? StringComparison.Ordinal
                    : StringComparison.OrdinalIgnoreCase;
                return name.Equals(kvp.Key, comparison);
            });
        }

        /// <summary>
        ///     Gets the first name from all the assigned names for this arg. This represents the primary name of the arg.
        /// </summary>
        public string Name => _names.Count == 0 ? null : _names.First().Key;

        /// <summary>
        ///     Gets all the secondary names for the arg.
        /// </summary>
        public IEnumerable<string> AlternateNames => _names.Skip(count: 1).Select(kvp => kvp.Key);

        /// <summary>
        ///     Gets the list of all names assigned to the arg.
        /// </summary>
        protected IDictionary<string, bool> Names => _names;

        /// <summary>
        ///     Gets all the names of the arg.
        /// </summary>
        internal IEnumerable<string> AllNames => _names.Select(kvp => kvp.Key);

        /// <summary>
        ///     Gets a valid name pattern to verify names against.
        ///     <para />
        ///     Derived classes can override this to specify different naming rules for certain types of args.
        /// </summary>
        protected virtual Regex NamePattern { get; } = new Regex(@"^\w[\w_-]*$");
    }

    // Metadata handling.
    public abstract partial class Arg
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<string, object> _metadata;

        /// <summary>
        ///     Gets or sets a string metadata value.
        /// </summary>
        /// <param name="name">The name of the metadata value.</param>
        /// <returns>The string value of the metadata.</returns>
        public string this[string name]
        {
            get => Get<string>(name);
            set => Set(name, value);
        }

        /// <summary>
        ///     Gets a metadata value by name.
        /// </summary>
        /// <typeparam name="T">The type of the metadata value.</typeparam>
        /// <param name="name">The name of the metadata value.</param>
        /// <returns>The metadata value or the default of T if the value does not exist.</returns>
        public T Get<T>(string name)
        {
            if (_metadata is null)
                return default;
            return _metadata.TryGetValue(name, out var result) ? (T)result : default;
        }

        /// <summary>
        ///     Sets a metadata value by name.
        /// </summary>
        /// <typeparam name="T">The type of the metadata value.</typeparam>
        /// <param name="name">The name of the metadata value.</param>
        /// <param name="value">The value of the metadata to set.</param>
        public void Set<T>(string name, T value)
        {
            if (_metadata is null)
                _metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (_metadata.ContainsKey(name))
                _metadata[name] = value;
            else
                _metadata.Add(name, value);
        }
    }
}
