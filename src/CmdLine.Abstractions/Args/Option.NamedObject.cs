// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleFx.CmdLine
{
    public sealed partial class Option : INamedObject
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly INamedObject _namedObject = new NamedObjectImpl(
            "The option name cannot be empty or whitespaced.",
            "'{0}' is an invalid name for an option.");

        public string Name => _namedObject.Name;

        public IEnumerable<string> AlternateNames => _namedObject.AlternateNames;

        public IEnumerable<string> AllNames => _namedObject.AllNames;

        public void AddName(string name, bool caseSensitive = false)
        {
            _namedObject.AddName(name, caseSensitive);
        }

        public bool HasName(string name)
        {
            return _namedObject.HasName(name);
        }
    }
}
