﻿#region --- License & Copyright Notice ---
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

using System.Collections.Generic;
using System.Diagnostics;

using ConsoleFx.CmdLineArgs.Base;

namespace ConsoleFx.CmdLineArgs
{
    [DebuggerDisplay(@"Command {Name ?? ""[Root]""}")]
    public class Command : Arg
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Arguments _arguments;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Options _options;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Commands _commands;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CommandCustomValidator _customValidator;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CommandHandler _handler;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Command" /> class.
        ///     <para />
        ///     This constructor is used internally to create root commands.
        /// </summary>
        internal Command()
        {
        }

        public Command(params string[] names)
        {
            foreach (string name in names)
                AddName(name);
        }

        public Command(bool caseSensitive, params string[] names)
        {
            foreach (string name in names)
                AddName(name, caseSensitive);
        }

        /// <summary>
        ///     Gets the collection of <see cref="Argument" /> objects for this command.
        /// </summary>
        public Arguments Arguments
        {
            get
            {
                if (_arguments == null)
                {
                    _arguments = new Arguments();
                    IEnumerable<Argument> arguments = GetArguments();
                    foreach (Argument argument in arguments)
                        _arguments.Add(argument);
                }

                return _arguments;
            }
        }

        protected virtual IEnumerable<Argument> GetArguments()
        {
            yield break;
        }

        /// <summary>
        ///     Gets the collection of <see cref="Option" /> objects for this command.
        /// </summary>
        public Options Options
        {
            get
            {
                if (_options == null)
                {
                    _options = new Options();
                    IEnumerable<Option> options = GetOptions();
                    foreach (Option option in options)
                        _options.Add(option);
                }

                return _options;
            }
        }

        protected virtual IEnumerable<Option> GetOptions()
        {
            yield break;
        }

        /// <summary>
        ///     Gets the collection of <see cref="Command" /> sub-command objects for this command.
        /// </summary>
        public Commands Commands
        {
            get
            {
                if (_commands == null)
                {
                    _commands = new Commands();
                    IEnumerable<Command> commands = GetCommands();
                    foreach (Command command in commands)
                        _commands.Add(command);
                }

                return _commands;
            }
        }

        protected virtual IEnumerable<Command> GetCommands()
        {
            yield break;
        }

        public CommandCustomValidator CustomValidator
        {
            get => _customValidator ?? PerformCustomValidation;
            set => _customValidator = value;
        }

        protected virtual string PerformCustomValidation(IReadOnlyList<string> arguments,
            IReadOnlyDictionary<string, object> options)
        {
            return null;
        }

        public CommandHandler Handler
        {
            get => _handler ?? HandleCommand;
            set => _handler = value;
        }

        protected virtual int HandleCommand(IReadOnlyList<string> arguments,
            IReadOnlyDictionary<string, object> options)
        {
            return 0;
        }
    }

    public delegate string CommandCustomValidator(IReadOnlyList<string> arguments,
        IReadOnlyDictionary<string, object> options);

    public delegate int CommandHandler(IReadOnlyList<string> arguments, IReadOnlyDictionary<string, object> options);

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
        protected override string GetDuplicateErrorMessage(string name) =>
            $"Command named '{name}' already exists in the command collection.";
    }

    public class RootCommand : Command
    {
        public RootCommand()
            : base()
        {
        }
    }
}
