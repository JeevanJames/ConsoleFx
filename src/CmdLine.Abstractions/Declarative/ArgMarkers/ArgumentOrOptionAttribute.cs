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
using System.Reflection;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Base class for attributes that mark properties in a <see cref="Command"/> class as an
    ///     <see cref="Argument"/> or an <see cref="Option"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class ArgumentOrOptionAttribute : Attribute
    {
        /// <summary>
        ///     Helper method to return the item type from the specified collection
        ///     <paramref name="property"/> info.
        /// </summary>
        /// <param name="property">The collection property info.</param>
        /// <returns>
        ///     The item type of the collection property. If the property is not a collection (doesn't
        ///     implement <see cref="IEnumerable{T}"/>), returns <c>null</c>.
        /// </returns>
        protected static Type GetCollectionItemType(PropertyInfo property)
        {
            Type type = property.PropertyType;

            if (!type.IsGenericType)
                return null;

            Type[] genericArgs = type.GetGenericArguments();
            if (genericArgs.Length != 1)
                return null;

            Type collectionType = typeof(IEnumerable<>).MakeGenericType(genericArgs[0]);
            if (!collectionType.IsAssignableFrom(type))
                return null;

            return genericArgs[0];
        }
    }
}
