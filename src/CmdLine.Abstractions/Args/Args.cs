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
using System.Collections.ObjectModel;
using System.Linq;

namespace ConsoleFx.CmdLine
{
    /// <inheritdoc />
    /// <summary>
    ///     Base class for collections of objects derived from <see cref="Arg" />.
    ///     <para />
    ///     Collections deriving from this class provide an additional indexer that can retrieve an object my its name.
    ///     They also prevent duplicate objects from being inserted or set on the collection.
    /// </summary>
    /// <typeparam name="T">The specific type of <see cref="Arg" /> that the collection will hold.</typeparam>
    public abstract class Args<T> : Collection<T>
        where T : Arg
    {
        /// <summary>
        ///     Gets an object from the collection given either the name.
        /// </summary>
        /// <param name="name">The name of the object to find.</param>
        /// <returns>The object, if found. Otherwise <c>null</c>.</returns>
        public T this[string name] => this.FirstOrDefault(item => NamesMatch(name, item));

        /// <summary>
        ///     Helper method to add multiple args to the collection.
        /// </summary>
        /// <param name="args">The args to add.</param>
        public void AddRange(IEnumerable<T> args)
        {
            if (args is null)
                throw new ArgumentNullException(nameof(args));

            foreach (T arg in args)
                Add(arg);
        }

        /// <summary>
        ///     Compares two <see cref="Arg" /> objects for equality. The default behavior is to check if any of their
        ///     names match, but deriving classes can override this behavior.
        /// </summary>
        /// <param name="obj1">The first <see cref="Arg" /> object to compare.</param>
        /// <param name="obj2">The second <see cref="Arg" /> object to compare.</param>
        /// <returns><c>true</c>, if the objects are equal, otherwise <c>false</c>.</returns>
        protected virtual bool ObjectsMatch(T obj1, T obj2)
        {
            return obj1.AllNames.Any(name => NamesMatch(name, obj2));
        }

        /// <summary>
        ///     Checks whether the specified object can be identified by the given name.
        /// </summary>
        /// <param name="name">The name to check against.</param>
        /// <param name="obj">The object, whose identity to check against the name.</param>
        /// <returns><c>true</c>, if the object can be identified by the given name, otherwise <c>false</c>.</returns>
        protected virtual bool NamesMatch(string name, T obj)
        {
            return obj.HasName(name);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Prevents duplicate objects from being inserted.
        /// </summary>
        /// <param name="index">Index to insert the new object.</param>
        /// <param name="item">Object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            CheckDuplicates(item, index: -1);
            base.InsertItem(index, item);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Prevents duplicate objects from being set in the collection.
        /// </summary>
        /// <param name="index">index to set the new option.</param>
        /// <param name="item">Object to set.</param>
        protected override void SetItem(int index, T item)
        {
            CheckDuplicates(item, index);
            base.SetItem(index, item);
        }

        /// <summary>
        ///     Checks whether the specified object already exists in the collection.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="index">The index in the collection at which the object is being inserted.</param>
        /// <exception cref="ArgumentException">Thrown if the object is already specified in the collection.</exception>
        private void CheckDuplicates(T obj, int index)
        {
            for (var i = 0; i < Count; i++)
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
