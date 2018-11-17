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
using System.Linq;

namespace ConsoleFx.CmdLineParser
{
    /// <summary>
    ///     <para>Base class for objects that store ad-hoc metadata.</para>
    ///     <para>Base class for <see cref="T:ConsoleFx.CmdLineParser.Option" />, <see cref="T:ConsoleFx.CmdLineParser.Argument" /> and
    ///     <see cref="Command" />.</para>
    /// </summary>
    public abstract class MetadataObject
    {
        private Dictionary<string, object> _metadata;

        protected MetadataObject()
        {
            Name = null;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MetadataObject"/> object.
        /// </summary>
        /// <param name="name">Name of the object.</param>
        /// <exception cref="ArgumentNullException">Thrown if the name is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if the name is empty or has only whitespace.</exception>
        protected MetadataObject(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (name.Trim().Length == 0)
                throw new ArgumentException("Specify valid name", nameof(name));
            Name = name;
        }

        /// <summary>
        ///     Name of the metadata object.
        /// </summary>
        public string Name { get; }

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

    public abstract class MetadataObjects<T> : Collection<T>
        where T : MetadataObject
    {
        public T this[string name] =>
            this.FirstOrDefault(item => NamesMatch(name, item));

        protected abstract bool NamesMatch(string name, T item);
    }
}
