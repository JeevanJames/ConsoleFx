// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Marks a property in a <see cref="Command"/> class as an <see cref="Argument"/>.
    /// </summary>
    public sealed class ArgumentAttribute : ArgumentOrOptionAttribute, IArgApplicator<Argument>
    {
        /// <summary>
        ///     Gets or sets the order of the argument in the list of arguments.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the argument is optional.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        ///     Gets or sets the maximum number of occurences allowed for the argument. This only
        ///     applies to the last argument. For other arguments, this value can only be one.
        /// </summary>
        public byte MaxOccurences { get; set; } = 1;

        void IArgApplicator<Argument>.Apply(Argument arg, PropertyInfo property)
        {
            ArgumentValueType expectedValueType = arg.GetValueType();
            switch (expectedValueType)
            {
                case ArgumentValueType.Object:
                    if (property.PropertyType != typeof(string))
                        arg.TypeAs(property.PropertyType);
                    break;
                case ArgumentValueType.List:
                    Type itemType = GetCollectionItemType(property);
                    if (itemType is null)
                        throw new ParserException(-1, $"Type for property {property.Name} in command {property.DeclaringType.FullName} should be a generic collection type like IEnumerable<T> or List<T>.");
                    if (itemType != typeof(string))
                        arg.TypeAs(itemType);
                    break;
                default:
                    throw new NotSupportedException($"Unexpected ArgumentValueType value of {expectedValueType}.");
            }
        }
    }
}
