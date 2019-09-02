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
    ///     Base class for any type that can have one or more names, each of which can be case
    ///     sensitive or not.
    /// </summary>
    public abstract class NamedObject : INamedObject
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, bool> _names = new Dictionary<string, bool>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="NamedObject" /> class.
        /// </summary>
        protected NamedObject()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NamedObject" /> class with one or more names.
        /// </summary>
        /// <param name="names">One or more names for the arg. The first name is considered primary.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="names" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="names" /> collection is empty.</exception>
        protected NamedObject(IDictionary<string, bool> names)
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
        public void AddName(string name, bool caseSensitive = false)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (!NamePattern.IsMatch(name))
                throw new ArgumentException(string.Format(Errors.Arg_Invalid_name, name), nameof(name));
            _names.Add(name, caseSensitive);
        }

        /// <summary>
        ///     Checks whether any of the args' names matches the specified name.
        /// </summary>
        /// <param name="name">The name to check against.</param>
        /// <returns>
        ///     <c>true</c>, if the specified name matches any of the args' names, otherwise
        ///     <c>false</c>.
        /// </returns>
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
        ///     Gets the first name from all the assigned names for this arg. This represents the
        ///     primary name of the arg.
        /// </summary>
        public string Name => _names.Count == 0 ? null : _names.First().Key;

        /// <summary>
        ///     Gets all the secondary names for the arg.
        /// </summary>
        public IEnumerable<string> AlternateNames => _names.Skip(count: 1).Select(kvp => kvp.Key);

        /// <summary>
        ///     Gets all the names of the arg.
        /// </summary>
        public IEnumerable<string> AllNames => _names.Select(kvp => kvp.Key);

        /// <summary>
        ///     Gets a valid name pattern to verify names against.
        ///     <para />
        ///     Derived classes can override this to specify different naming rules for certain types of args.
        /// </summary>
        protected virtual Regex NamePattern { get; } = new Regex(@"^\w[\w_-]*$");
    }
}
