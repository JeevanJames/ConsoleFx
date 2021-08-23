// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

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
