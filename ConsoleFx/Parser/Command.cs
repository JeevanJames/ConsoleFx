#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015-2016 Jeevan James

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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleFx.Parser
{
    [DebuggerDisplay(@"Command {Name ?? ""[Root]""}")]
    public sealed class Command : MetadataObject
    {
        private List<string> _excludedInheritingOptions;

        /// <summary>
        ///     Creates the root command, which has no name.
        /// </summary>
        internal Command()
        {
        }

        public Command(string name, bool caseSensitive = false)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (!NamePattern.IsMatch(name))
            {
                throw new ArgumentException(
                    $"'{name}' is not a valid command name. Command names should only consist of alphanumeric characters.",
                    nameof(name));
            }
            Name = name;
            NameComparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        private static readonly Regex NamePattern = new Regex(@"^\w+$", RegexOptions.Compiled);

        /// <summary>
        ///     Gets the name of the command.
        /// </summary>
        public string Name { get; }

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

        public IList<string> ExcludedInheritingOptions => _excludedInheritingOptions ?? (_excludedInheritingOptions = new List<string>());
    }

    /// <summary>
    ///     Collection of <see cref="Command" /> objects. This collection adds special behavior to prevent duplicate command
    ///     names in the collection as well as the ability to retrieve sub-commands based on the correct case-sensivity.
    /// </summary>
    public sealed class Commands : Collection<Command>
    {
        public Command this[string name] =>
            this.FirstOrDefault(command => command.Name.Equals(name, command.NameComparison));

        protected override void InsertItem(int index, Command command)
        {
            CheckDuplicates(command, -1);
            base.InsertItem(index, command);
        }

        protected override void SetItem(int index, Command command)
        {
            CheckDuplicates(command, index);
            base.SetItem(index, command);
        }

        private void CheckDuplicates(Command command, int index)
        {
            for (int i = 0; i < Count; i++)
            {
                if (i == index)
                    continue;
                if (this[i].Name.Equals(command.Name, this[i].NameComparison))
                {
                    throw new ArgumentException(
                        $"Command named '{command.Name}' already exists in the command collection.");
                }
            }
        }
    }
}
