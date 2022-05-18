// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Base class for command-line args, such as <see cref="Option" />, <see cref="Argument" />
    ///     and <see cref="Command" />.
    ///     <para />
    ///     This class adds support for metadata.
    /// </summary>
    public abstract class Arg
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<Dictionary<string, object>> _metadata = new(
            () => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));

        /// <summary>
        ///     Initializes a new instance of the <see cref="Arg"/> class.
        /// </summary>
        protected Arg()
        {
            ProcessMetadataAttributes();
        }

        /// <summary>
        ///     Read any metadata attributes on this class and assign them to this arg.
        /// </summary>
        private void ProcessMetadataAttributes()
        {
            IEnumerable<MetadataAttribute> metadataAttributes = GetType()
                .GetCustomAttributes<MetadataAttribute>(inherit: true);
            foreach (MetadataAttribute metadataAttribute in metadataAttributes)
                metadataAttribute.AssignMetadata(this);
        }

        /// <summary>
        ///     Gets a metadata value by name.
        /// </summary>
        /// <typeparam name="T">The type of the metadata value.</typeparam>
        /// <param name="name">The name of the metadata value.</param>
        /// <returns>The metadata value or the default of T if the value does not exist.</returns>
        public T Get<T>(string name)
        {
            return _metadata.Value.TryGetValue(name, out object result) ? (T)result : default;
        }

        /// <summary>
        ///     Sets a metadata value by name.
        /// </summary>
        /// <typeparam name="T">The type of the metadata value.</typeparam>
        /// <param name="name">The name of the metadata value.</param>
        /// <param name="value">The value of the metadata to set.</param>
        public void Set<T>(string name, T value)
        {
            if (_metadata.Value.ContainsKey(name))
                _metadata.Value[name] = value;
            else
                _metadata.Value.Add(name, value);
        }
    }
}
