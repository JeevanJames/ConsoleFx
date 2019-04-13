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
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleFx.CmdLineParser
{
    [DebuggerDisplay(@"Command {Name ?? ""[Root]""}")]
    public sealed class Command : Arg
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Command"/> class.
        ///     <para/>
        ///     This constructor is used internally to create root commands.
        /// </summary>
        internal Command()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="name">Name of the command.</param>
        /// <param name="caseSensitive">Indicates whether the command name is case sensitive.</param>
        /// <exception cref="T:System.ArgumentNullException">Thrown if the command name is null.</exception>
        /// <exception cref="T:System.ArgumentException">Thrown if the command name is not valid.</exception>
        public Command(string name, bool caseSensitive = false)
            : base(new Dictionary<string, bool> { [name] = caseSensitive })
        {
            if (!NamePattern.IsMatch(name))
            {
                throw new ArgumentException(
                    $"'{name}' is not a valid command name. Command names should only consist of alphanumeric characters.",
                    nameof(name));
            }

            NameComparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        /// <summary>
        ///     Gets a value indicating whether command name is case-sensitive.
        /// </summary>
        public bool CaseSensitive => NameComparison == StringComparison.Ordinal;

        /// <summary>
        ///     Gets the <see cref="StringComparison" /> value used for comparing the command name.
        /// </summary>
        internal StringComparison NameComparison { get; }

        /// <summary>
        ///     Gets the collection of <see cref="Argument" /> objects for this command.
        /// </summary>
        public Arguments Arguments { get; } = new Arguments();

        /// <summary>
        ///     Gets the collection of <see cref="Option" /> objects for this command.
        /// </summary>
        public Options Options { get; } = new Options();

        /// <summary>
        ///     Gets the collection of <see cref="Command" /> sub-command objects for this command.
        /// </summary>
        public Commands Commands { get; } = new Commands();
    }

    /// <summary>
    ///     <para>Collection of <see cref="Command" /> objects.</para>
    ///     <para>
    ///         This collection adds special behavior to prevent duplicate command names in the
    ///         collection as well as the ability to retrieve sub-commands based on the correct
    ///         case-sensitivity.
    ///     </para>
    /// </summary>
    public sealed class Commands : Args<Command>
    {
        protected override bool NamesMatch(string name, Command obj) =>
            obj.Name.Equals(name, obj.NameComparison);

        protected override string GetDuplicateErrorMessage(string name) =>
            $"Command named '{name}' already exists in the command collection.";
    }
}
