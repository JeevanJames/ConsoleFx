using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleFx.CmdLine.Internals
{
    internal static class ReflectionExtensions
    {
        internal static bool IsCollectionProperty(this PropertyInfo property)
        {
            return property.PropertyType.GetInterfaces()
                .Any(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        /// <summary>
        ///     Helper method to return the item type from the specified collection
        ///     <paramref name="property"/> info.
        /// </summary>
        /// <param name="property">The collection property info.</param>
        /// <returns>
        ///     The item type of the collection property. If the property is not a collection (doesn't
        ///     implement <see cref="IEnumerable{T}"/>), returns <c>null</c>.
        /// </returns>
        internal static Type GetCollectionItemType(this PropertyInfo property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

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
