// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleFx.CmdLine
{
    // INamedObject implementation
    public partial class Command : INamedObject
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly INamedObject _namedObject = new NamedObjectImpl(
            "The command name cannot be empty or whitespaced.",
            "'{0}' is an invalid name for a command.");

        /// <inheritdoc />
        public string Name => _namedObject.Name;

        /// <inheritdoc />
        public IEnumerable<string> AlternateNames => _namedObject.AlternateNames;

        /// <inheritdoc />
        public IEnumerable<string> AllNames => _namedObject.AllNames;

        /// <inheritdoc />
        public void AddName(string name, bool caseSensitive = false)
        {
            _namedObject.AddName(name, caseSensitive);
        }

        /// <inheritdoc />
        public bool HasName(string name)
        {
            return _namedObject.HasName(name);
        }
    }
}
