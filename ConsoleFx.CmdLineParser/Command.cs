#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2018 Jeevan James

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

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ConsoleFx.CmdLineParser
{
    [DebuggerDisplay(@"Command {Name ?? ""[Root]""}")]
    public sealed class Command : MetadataObject
    {
        /// <summary>
        ///     <para>Initializes a new instance of the <see cref="Command"/> object.</para>
        ///     <para>This constructor is used internally to create root commands.</para>
        /// </summary>
        internal Command()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Command"/> object.
        /// </summary>
        /// <param name="name">Name of the command.</param>
        /// <param name="caseSensitive">Indicates whether the command name is case sensitive.</param>
        /// <exception cref="ArgumentNullException">Thrown if the command name is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the command name is not valid.</exception>
        public Command(string name, bool caseSensitive = false) : base(name)
        {
            if (!NamePattern.IsMatch(name))
            {
                throw new ArgumentException(
                    $"'{name}' is not a valid command name. Command names should only consist of alphanumeric characters.",
                    nameof(name));
            }
            NameComparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        private static readonly Regex NamePattern = new Regex(@"^\w+$", RegexOptions.Compiled);

        /// <summary>
        ///     Specifies whether the command name is case-sensitive.
        /// </summary>
        public bool CaseSensitive => NameComparison == StringComparison.Ordinal;

        /// <summary>
        ///     The <see cref="StringComparison" /> value used for comparing the command name.
        /// </summary>
        internal StringComparison NameComparison { get; }

        /// <summary>
        ///     Collection of <see cref="Argument" /> objects for this command.
        /// </summary>
        public Arguments Arguments { get; } = new Arguments();

        /// <summary>
        ///     Collection of <see cref="Option" /> objects for this command.
        /// </summary>
        public Options Options { get; } = new Options();

        /// <summary>
        ///     Collection of <see cref="Command" /> sub-command objects for this command.
        /// </summary>
        public Commands Commands { get; } = new Commands();
    }

    /// <summary>
    ///     <para>Collection of <see cref="Command" /> objects.</para>
    ///     <para>
    ///         This collection adds special behavior to prevent duplicate command names in the
    ///         collection as well as the ability to retrieve sub-commands based on the correct
    ///         case-sensivity.
    ///     </para>
    /// </summary>
    public sealed class Commands : MetadataObjects<Command>
    {
        protected override bool NamesMatch(string name, Command item) =>
            item.Name.Equals(name, item.NameComparison);

        protected override string GetDuplicateErrorMessage(string name) =>
            $"Command named '{name}' already exists in the command collection.";
    }
}
