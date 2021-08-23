// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Indicates that a class is a <see cref="Command"/> with one or more names.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class CommandAttribute : Attribute
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Type _parentType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandAttribute"/> class with the
        ///     specified <paramref name="name"/> and <paramref name="parentType"/>.
        /// </summary>
        /// <param name="name">The primary name of the command.</param>
        /// <param name="parentType">The optional type of the parent <see cref="Command"/>.</param>
        public CommandAttribute(string name, Type parentType)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            Names = new[] { name };
            ParentType = parentType;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandAttribute"/> class with the
        ///     specified <paramref name="names"/>.
        /// </summary>
        /// <param name="names">One or more names of the command.</param>
        public CommandAttribute(params string[] names)
        {
            if (names is null)
                throw new ArgumentNullException(nameof(names));
            if (names.Length == 0)
                throw new ArgumentException("Specify at least one name for the command.", nameof(names));

            Names = names.ToList();
        }

        /// <summary>
        ///     Gets the names associated with this command.
        /// </summary>
        public IReadOnlyList<string> Names { get; }

        /// <summary>
        ///     Gets or sets the optional type of the parent command.
        /// </summary>
        public Type ParentType
        {
            get => _parentType;
            set
            {
                if (value != null && !typeof(Command).IsAssignableFrom(value))
                    throw new ArgumentException($"ParentType should be type {typeof(Command).FullName} or a derived type.", nameof(value));
                _parentType = value;
            }
        }
    }
}
