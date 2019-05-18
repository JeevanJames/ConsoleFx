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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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
        private byte _lastArgumentRepeat = 1;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Command" /> class.
        ///     <para />
        ///     This constructor is used internally to create root commands.
        /// </summary>
        public Command()
        {
            CommandAttribute commandAttribute = GetType().GetCustomAttribute<CommandAttribute>(true);
            if (commandAttribute != null)
            {
                foreach (string name in commandAttribute.Names)
                    AddName(name);
            }
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
                if (_arguments is null)
                {
                    _arguments = new Arguments();
                    IEnumerable<Argument> arguments = GetArgs().OfType<Argument>();
                    foreach (Argument argument in arguments)
                        _arguments.Add(argument);
                }

                return _arguments;
            }
        }

        /// <summary>
        ///     Gets the collection of <see cref="Option" /> objects for this command.
        /// </summary>
        public Options Options
        {
            get
            {
                if (_options is null)
                {
                    _options = new Options();
                    IEnumerable<Option> options = GetArgs().OfType<Option>();
                    foreach (Option option in options)
                        _options.Add(option);
                }

                return _options;
            }
        }

        /// <summary>
        ///     Gets the collection of <see cref="Command" /> sub-command objects for this command.
        /// </summary>
        public Commands Commands
        {
            get
            {
                if (_commands is null)
                {
                    _commands = new Commands();
                    IEnumerable<Command> commands = GetArgs().OfType<Command>();
                    foreach (Command command in commands)
                        _commands.Add(command);
                }

                return _commands;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating how many times the last argument can repeat.
        /// </summary>
        public byte LastArgumentRepeat
        {
            get => _lastArgumentRepeat;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _lastArgumentRepeat = value;
            }
        }

        protected virtual IEnumerable<Arg> GetArgs()
        {
            yield break;
        }

        public CommandCustomValidator CustomValidator
        {
            get => _customValidator ?? PerformCustomValidation;
            set => _customValidator = value;
        }

        protected virtual string PerformCustomValidation(IReadOnlyList<object> arguments,
            IReadOnlyDictionary<string, object> options)
        {
            return null;
        }

        public Argument AddArgument(string name, bool isOptional = false)
        {
            var argument = new Argument(name, isOptional);
            Arguments.Add(argument);
            return argument;
        }

        public Command AddCommand(params string[] names)
        {
            var command = new Command(names);
            Commands.Add(command);
            return command;
        }

        public Command AddCommand(bool caseSensitive, params string[] names)
        {
            var command = new Command(caseSensitive, names);
            Commands.Add(command);
            return command;
        }

        public Option AddOption(params string[] names)
        {
            var option = new Option(names);
            Options.Add(option);
            return option;
        }

        public Option AddOption(bool caseSensitive, params string[] names)
        {
            var option = new Option(caseSensitive, names);
            Options.Add(option);
            return option;
        }

        protected sealed override Regex NamePattern => base.NamePattern;
    }

    public delegate string CommandCustomValidator(IReadOnlyList<object> arguments,
        IReadOnlyDictionary<string, object> options);

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
        /// <inheritdoc />
        /// <summary>
        ///     While a <see cref="Command"/> does not need to have a name, a command added to the
        ///     <see cref="Commands"/> collection needs to have a name.
        /// </summary>
        protected override void InsertItem(int index, Command item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                throw new ArgumentException("Sub-commands must have a name.", nameof(item));

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

            base.SetItem(index, item);
        }

        protected override string GetDuplicateErrorMessage(string name) =>
            $"Command named '{name}' already exists in the command collection.";
    }
}
