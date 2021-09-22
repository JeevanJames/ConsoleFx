// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleFx.CmdLine.Program
{
    public abstract partial class HelpBuilder
    {
        protected HelpBuilder()
        {
        }

        protected HelpBuilder(IDictionary<string, bool> names)
        {
            if (names is null)
                throw new ArgumentNullException(nameof(names));
            if (names.Count == 0)
                throw new ArgumentException(Errors.Arg_No_names_specified, nameof(names));
            foreach (KeyValuePair<string, bool> kvp in names)
                AddName(kvp.Key, kvp.Value);
        }

        /// <summary>
        ///     Override this method to display the help for the specified <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> for which to display help.</param>
        public abstract void DisplayHelp(Command command);

        /// <summary>
        ///     Derived help builders can override this method to verify that they have all the
        ///     necessary information from the <see cref="Command"/> to display help.
        /// </summary>
        /// <param name="command">The <see cref="Command"/> to verify.</param>
        public virtual void VerifyHelp(Command command)
        {
        }
    }

    public abstract partial class HelpBuilder : INamedObject
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly NamedObjectImpl _namedObject = new();

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
