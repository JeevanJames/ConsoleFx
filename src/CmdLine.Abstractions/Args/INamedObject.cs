// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Represents an object that can have one or more names.
    /// </summary>
    public interface INamedObject
    {
        /// <summary>
        ///     Adds a new name for the arg.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="caseSensitive">Indicates whether the name is case-sensitive.</param>
        void AddName(string name, bool caseSensitive = false);

        /// <summary>
        ///     Checks whether any of the args' names matches the specified name.
        /// </summary>
        /// <param name="name">The name to check against.</param>
        /// <returns>
        ///     <c>true</c>, if the specified name matches any of the args' names, otherwise
        ///     <c>false</c>.
        /// </returns>
        bool HasName(string name);

        /// <summary>
        ///     Gets the first name from all the assigned names for this arg. This represents the
        ///     primary name of the arg.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets all the secondary names for the arg.
        /// </summary>
        IEnumerable<string> AlternateNames { get; }

        /// <summary>
        ///     Gets all the names of the arg.
        /// </summary>
        IEnumerable<string> AllNames { get; }

        //TODO: Add a method to retrieve the error messages for different types of implementors.
        //Instead of a very generic error message coming from the common NamedObjectImpl, let
        //implementers be allowed to sepcify detailed messages.
    }
}
