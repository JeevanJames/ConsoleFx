// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleFx.CmdLine.Help
{
    public abstract partial class HelpBuilder : INamedObject
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly NamedObjectImpl _namedObject = new(
            "The help name cannot be empty or whitespaced.",
            "'{0}' is an invalid name for a help.");

        public string Name => ((INamedObject)_namedObject).Name;

        public IEnumerable<string> AlternateNames => ((INamedObject)_namedObject).AlternateNames;

        public IEnumerable<string> AllNames => ((INamedObject)_namedObject).AllNames;

        public void AddName(string name, bool caseSensitive = false)
        {
            ((INamedObject)_namedObject).AddName(name, caseSensitive);
        }

        public bool HasName(string name)
        {
            return ((INamedObject)_namedObject).HasName(name);
        }
    }
}
