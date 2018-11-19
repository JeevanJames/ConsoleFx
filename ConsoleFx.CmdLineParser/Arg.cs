#region --- License & Copyright Notice ---
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleFx.CmdLineParser
{
    /// <summary>
    ///     <para>Base class for command-line args, such as commands, arguments and options.</para>
    ///     <para>Base class for <see cref="T:ConsoleFx.CmdLineParser.Option" />, <see cref="T:ConsoleFx.CmdLineParser.Argument" /> and
    ///     <see cref="Command" />.</para>
    /// </summary>
    public abstract class Arg
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Dictionary<string, bool> _names;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Dictionary<string, object> _metadata;

        protected Arg()
        {
            _names = new Dictionary<string, bool>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Arg"/> object.
        /// </summary>
        /// <param name="name">Name of the object.</param>
        /// <exception cref="ArgumentNullException">Thrown if the name is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if the name is empty or has only whitespace.</exception>
        protected Arg(IDictionary<string, bool> names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));
            if (names.Count == 0)
                throw new ArgumentException("Specify at least one name", nameof(names));
            foreach (var kvp in names)
            {
                if (kvp.Key == null)
                    throw new ArgumentException("Name specified cannot be null", nameof(names));
                if (!NamePattern.IsMatch(kvp.Key))
                    throw new ArgumentException($"Name {kvp.Key} is not a valid name.", nameof(names));
            }
            _names = new Dictionary<string, bool>(names);
        }

        public Arg AddName(string name, bool caseSensitive = false)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (!NamePattern.IsMatch(name))
                throw new ArgumentException($"Name {name} is not a valid name.", nameof(name));
            _names.Add(name, caseSensitive);
            return this;
        }

        /// <summary>
        ///     Returns whether any of the arg's names matches the specified name.
        /// </summary>
        /// <param name="name">The name to check against.</param>
        /// <returns><c>true</c>, if the specified name matches any of the arg's names, otherwise <c>false</c>.</returns>
        public bool HasName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            return _names.Any(kvp =>
            {
                StringComparison comparison = kvp.Value
                    ? StringComparison.Ordinal
                    : StringComparison.OrdinalIgnoreCase;
                return name.Equals(kvp.Key, comparison);
            });
        }

        /// <summary>
        ///     Gets the first name from all the assigned names for this arg.
        /// </summary>
        public string Name => _names.First().Key;

        /// <summary>
        ///     Gets all the secondary names for the arg.
        /// </summary>
        public IEnumerable<string> AlternateNames => _names.Skip(1).Select(kvp => kvp.Key);

        protected IDictionary<string, bool> Names => _names;

        protected virtual Regex NamePattern { get; } = new Regex(@"^\w[\w_-]*$");

        /// <summary>
        ///     Gets or sets a string metadata value.
        /// </summary>
        /// <param name="name">The name of the metadata value.</param>
        /// <returns>The string value of the metadata.</returns>
        public string this[string name]
        {
            get { return Get<string>(name); }
            set { Set(name, value); }
        }

        /// <summary>
        ///     Gets a metadata value by name.
        /// </summary>
        /// <typeparam name="T">The type of the metadata value.</typeparam>
        /// <param name="name">The name of the metadata value.</param>
        /// <returns>The metadata value or the default of T if the value does not exist.</returns>
        public T Get<T>(string name)
        {
            if (_metadata == null)
                return default(T);
            return _metadata.TryGetValue(name, out object result) ? (T)result : default(T);
        }

        /// <summary>
        ///     Sets a metadata value by name.
        /// </summary>
        /// <typeparam name="T">The type of the metadata value.</typeparam>
        /// <param name="name">The name of the metadata value.</param>
        /// <param name="value">The value of the metadata to set.</param>
        public void Set<T>(string name, T value)
        {
            if (_metadata == null)
                _metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (_metadata.ContainsKey(name))
                _metadata[name] = value;
            else
                _metadata.Add(name, value);
        }
    }

    /// <summary>
    ///     <para>Base class for collections of objects derived from <see cref="Arg"/>.</para>
    ///     <para>
    ///         Collections deriving from this class provide an additional indexer that can retrieve
    ///         an object my its name. They also prevent duplicate objects from being inserted or
    ///         set on the collection.
    ///     </para>
    /// </summary>
    /// <typeparam name="T">The specific type of <see cref="Arg"/> that the collection will hold.</typeparam>
    public abstract class Args<T> : Collection<T>
        where T : Arg
    {
        /// <summary>
        ///     Gets an object from the collection given either the name.
        /// </summary>
        /// <param name="name">The name of the object to find.</param>
        /// <returns>The object, if found. Otherwise <c>null</c>.</returns>
        public T this[string name] =>
            this.FirstOrDefault(item => NamesMatch(name, item));

        /// <summary>
        ///     Compares two objects for equality. The default behavior is to compare their Name
        ///     properties, but deriving classes can override this behavior.
        /// </summary>
        /// <param name="obj1">The first object to compare.</param>
        /// <param name="obj2">The second object to compare.</param>
        /// <returns><c>true</c>, if the objects are equal, otherwise <c>false</c>.</returns>
        protected virtual bool ObjectsMatch(T obj1, T obj2) =>
            NamesMatch(obj1.Name, obj2);

        /// <summary>
        ///     Checks whether the specified object can be identified by the given name.
        /// </summary>
        /// <param name="name">The name to check against.</param>
        /// <param name="obj">The object, whose identity to check against the name.</param>
        /// <returns><c>true</c>, if the object can be identified by the given name, otherwise <c>false</c>.</returns>
        protected virtual bool NamesMatch(string name, T obj) =>
            name.Equals(obj.Name);

        /// <summary>
        ///     Prevents duplicate objects from being inserted.
        /// </summary>
        /// <param name="index">Index to insert the new object.</param>
        /// <param name="item">Object to insert.</param>
        protected override void InsertItem(int index, T obj)
        {
            CheckDuplicates(obj, -1);
            base.InsertItem(index, obj);
        }

        /// <summary>
        ///     Prevents duplicate objects from being set in the collection.
        /// </summary>
        /// <param name="index">index to set the new option.</param>
        /// <param name="item">Object to set.</param>
        protected override void SetItem(int index, T obj)
        {
            CheckDuplicates(obj, index);
            base.SetItem(index, obj);
        }

        /// <summary>
        ///     Checks whether the specified object already exists in the collection.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="index">The index in the collection at which the object is being inserted.</param>
        /// <exception cref="ArgumentException">Thrown if the object is already specified in the collection.</exception>
        private void CheckDuplicates(T obj, int index)
        {
            for (int i = 0; i < Count; i++)
            {
                if (i == index)
                    continue;
                if (ObjectsMatch(obj, this[i]))
                    throw new ArgumentException(GetDuplicateErrorMessage(obj.Name), nameof(obj));
            }
        }

        /// <summary>
        ///     Gets the error message of the exception that is thrown if a duplicate item is
        ///     inserted or set in the collection.
        /// </summary>
        /// <param name="name">
        ///     The name of the duplicate item that is being inserted or set in the collection,
        ///     usually the <c>Name</c> property.
        /// </param>
        /// <returns>The error message string for the exception.</returns>
        protected abstract string GetDuplicateErrorMessage(string name);
    }
}
