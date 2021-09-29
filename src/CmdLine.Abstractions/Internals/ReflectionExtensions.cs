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
    }
}
