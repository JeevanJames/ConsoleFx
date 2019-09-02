#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2019 Jeevan James

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
using System.Linq;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Collection of <see cref="Command" /> objects.
    ///     <para/>
    ///     This collection adds special behavior to prevent duplicate command names in the
    ///     collection as well as the ability to retrieve sub-commands based on the correct
    ///     case-sensitivity.
    /// </summary>
    public sealed class Commands : Args<Command>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Command _parentCommand;

        internal Commands(Command parentCommand)
        {
            _parentCommand = parentCommand;
        }

        public Command this[string name] => this.FirstOrDefault(item => item.HasName(name));

        /// <inheritdoc />
        /// <summary>
        ///     While a <see cref="Command"/> does not need to have a name, a command added to the
        ///     <see cref="Commands"/> collection needs to have a name.
        /// </summary>
        protected override void InsertItem(int index, Command item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                throw new ArgumentException("Sub-commands must have a name.", nameof(item));

            item.ParentCommand = _parentCommand;
            base.InsertItem(index, item);
        }

        /// <inheritdoc />
        /// <summary>
        ///     While a <see cref="Command"/> does not need to have a name, a command added to the
        ///     <see cref="Commands"/> collection needs to have a name.
        /// </summary>
        protected override void SetItem(int index, Command item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                throw new ArgumentException("Sub-commands must have a name.", nameof(item));

            item.ParentCommand = _parentCommand;
            base.SetItem(index, item);
        }

        protected override void CheckDuplicates(Command obj, int index)
        {
            for (var i = 0; i < Count; i++)
            {
                if (i == index)
                    continue;
                if (ObjectsMatch(obj, this[i]))
                    throw new ArgumentException($"Command named '{obj.Name}' already exists in the command collection.", nameof(obj));
            }
        }

        private bool ObjectsMatch(Command command1, Command command2)
        {
            return command1.AllNames.Any(name => command2.HasName(name));
        }
    }
}
