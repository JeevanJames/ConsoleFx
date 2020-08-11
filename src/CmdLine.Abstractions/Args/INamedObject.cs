#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

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
    }
}
