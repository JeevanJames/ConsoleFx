// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Marks a property in a <see cref="Command"/> class as an <see cref="Argument"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ArgumentAttribute : Attribute
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
    }
}
