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
    public abstract class Arg : IMetadataObject
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

        /// <inheritdoc />
        public object this[string name]
        {
            get => ((IMetadataObject)this).Get<object>(name);
            set => ((IMetadataObject)this).Set(name, value);
        }

        /// <inheritdoc />
        public T Get<T>(string name)
        {
            return _metadata.Value.TryGetValue(name, out object result) ? (T)result : default;
        }

        /// <inheritdoc />
        public void Set<T>(string name, T value)
        {
            if (_metadata.Value.ContainsKey(name))
                _metadata.Value[name] = value;
            else
                _metadata.Value.Add(name, value);
        }

        /// <summary>
        ///     Read any metadata attributes on this class and assign them to this arg.
        /// </summary>
        private void ProcessMetadataAttributes()
        {
            IEnumerable<MetadataAttribute> metadataAttributes = GetType().GetCustomAttributes<MetadataAttribute>(inherit: true);
            foreach (MetadataAttribute metadataAttribute in metadataAttributes)
                metadataAttribute.AssignMetadata(this);
        }
    }
}
