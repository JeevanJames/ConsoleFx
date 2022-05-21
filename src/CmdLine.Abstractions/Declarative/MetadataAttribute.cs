// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Base class for any attribute that can assign metadata to an <see cref="Arg"/>.
    ///     <para/>
    ///     The framework locates such attributes decorated on args and automatically assigns them to
    ///     the metadata of the arg.
    /// </summary>
    /// <remarks>
    ///     An example of metadata attributes are the Help attributes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class MetadataAttribute : Attribute
    {
        /// <summary>
        ///     Override this method to return one or more metadata for the arg decorated by this
        ///     attribute.
        /// </summary>
        /// <returns>One or more metadata key-value pairs.</returns>
        public abstract IEnumerable<ArgMetadata> GetMetadata();

        /// <summary>
        ///     Helper method to assign the metadata values from this attribute to the specified
        ///     <paramref name="arg"/>.
        /// </summary>
        /// <typeparam name="TArg">The type of arg.</typeparam>
        /// <param name="arg">The arg to assign the metadata to.</param>
        public void AssignMetadata<TArg>(TArg arg)
            where TArg : Arg
        {
            // Validate applicable args for this attribute.
            //TODO: Simplify code
            IEnumerable<Type> applicableArgs = GetApplicableArgTypes();
            if (!applicableArgs.Any(type => typeof(TArg).IsAssignableFrom(type)))
            {
                throw new InvalidOperationException(
                    $"Cannot apply the {GetType().Name} attribute to an arg of type {typeof(TArg).Name}.");
            }

            foreach (ArgMetadata metadataItem in GetMetadata())
                arg.Set(metadataItem.Name, metadataItem.Value);
        }

        /// <summary>
        ///     Specifies the types of args that this attribute can apply to.
        /// </summary>
        /// <returns>The applicable arg types.</returns>
        protected abstract IEnumerable<Type> GetApplicableArgTypes();

        protected static string ResolveResourceString(string unlocalizedValue, Type resourceType, string resourceName,
            bool required)
        {
            if (resourceType is not null && !string.IsNullOrWhiteSpace(resourceName))
            {
                PropertyInfo resourceProperty = resourceType.GetTypeInfo().GetDeclaredProperty(resourceName);
                if (resourceProperty is null)
                {
                    throw new ParserException(-1,
                        $"Resource type {resourceType} does not contain a resource named {resourceName}.");
                }

                if (resourceProperty.PropertyType != typeof(string))
                {
                    throw new ParserException(-1,
                        $"Resource {resourceName} on the resource type {resourceType} is not a string.");
                }

                return resourceProperty.GetValue(null, null) as string;
            }

            if (unlocalizedValue is not null)
                return unlocalizedValue;

            if (required)
                throw new ParserException(-1, "Specify either a string value or a resource type/name pair.");

            return null;
        }
    }

    public static class CommonApplicableArgTypes
    {
        public static readonly Type[] Command = { typeof(Command) };
        public static readonly Type[] Argument = { typeof(Argument) };
        public static readonly Type[] Option = { typeof(Option) };

        public static readonly Type[] All = { typeof(Command), typeof(Argument), typeof(Option) };
    }
}
