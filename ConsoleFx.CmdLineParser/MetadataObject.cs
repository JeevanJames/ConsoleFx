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
        /// <summary>
        ///     Gets an object from the collection given either the name.
        /// </summary>
        /// <param name="name">The name of the object to find.</param>
        /// <returns>The object, if found. Otherwise <c>null</c>.</returns>
        public T this[string name] =>
            this.FirstOrDefault(item => NamesMatch(name, item));

        protected virtual bool ObjectsMatch(T obj1, T obj2) =>
            NamesMatch(obj1.Name, obj2);

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
        ///     Checks whether the specified object exists in the collection.
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

        protected abstract string GetDuplicateErrorMessage(string name);
    }
}
