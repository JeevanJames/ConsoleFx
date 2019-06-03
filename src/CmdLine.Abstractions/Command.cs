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

namespace ConsoleFx.CmdLine
{
    [DebuggerDisplay(@"Command {Name ?? ""[Root]""}")]
    public partial class Command : Arg
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Func<ParseResultBase, int> _handler;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Command" /> class.
        ///     <para />
        ///     This constructor tries to read the command details from the
        ///     <see cref="CommandAttribute"/> attribute.
        /// </summary>
        public Command()
        {
            ProcessCommandAttribute();
        }

        public Command(params string[] names)
        {
            ProcessCommandAttribute();
            foreach (string name in names)
                AddName(name);
        }

        public Command(bool caseSensitive, params string[] names)
        {
            ProcessCommandAttribute();
            foreach (string name in names)
                AddName(name, caseSensitive);
        }

        private void ProcessCommandAttribute()
        {
            // Read the command attribute on this class.
            CommandAttribute commandAttribute = GetType().GetCustomAttribute<CommandAttribute>(true);
            if (commandAttribute != null)
            {
                // Add names from attribute
                foreach (string name in commandAttribute.Names)
                    AddName(name);

                // Throw exception if parent type is same as current type.
                if (commandAttribute.ParentType == GetType())
                    throw new InvalidOperationException($"Parent command type of {GetType().FullName} command cannot be the same type");
            }
        }

        /// <summary>
        ///     Gets or sets a reference to the parent <see cref="Command"/> of this instance.
        /// </summary>
        internal Command ParentCommand { get; set; }

        /// <summary>
        ///     Gets the reference to the root <see cref="Command"/> instance.
        /// </summary>
        internal Command RootCommand
        {
            get
            {
                Command currentCommand = this;
                while (currentCommand.ParentCommand != null)
                    currentCommand = currentCommand.ParentCommand;
                return currentCommand;
            }
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
                    _commands = new Commands(this);

                    // Add the subcommands from the GetArgs method.
                    IEnumerable<Command> commands = GetArgs().OfType<Command>();
                    foreach (Command command in commands)
                        _commands.Add(command);

                    // Add the subcommands from the DiscoveredCommands collection created from the
                    // ScanAssembliesForCommands method in the root command.
                    IEnumerable<Type> childCommandTypes = RootCommand.DiscoveredCommands
                        .Where(kvp => kvp.Value == GetType())
                        .Select(kvp => kvp.Key);
                    foreach (Type childCommandType in childCommandTypes)
                    {
                        var command = (Command)Activator.CreateInstance(childCommandType);
                        if (command.Name != null)
                            _commands.Add(command);
                    }
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

        /// <summary>
        ///     Override this method to specify the arguments, options and sub-commands of this
        ///     <see cref="Command"/> instance.
        /// </summary>
        /// <returns>
        ///     A sequence of <see cref="Argument"/>, <see cref="Option"/> and <see cref="Command"/>
        ///     objects that belong to this <see cref="Command"/> instance.
        /// </returns>
        protected virtual IEnumerable<Arg> GetArgs()
        {
            yield break;
        }

        /// <summary>
        ///     Gets or sets the delegate to call to perform additional validations after the tokens
        ///     have been parsed.
        ///     <para />
        ///     If not assigned, the virtual
        ///     <see cref="PerformCustomValidation(IReadOnlyList{object}, IReadOnlyDictionary{string, object})"/>
        ///     method is called.
        /// </summary>
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

        /// <summary>
        ///     Gets or sets the delegate to call if the parsed args match this command.
        ///     <para/>
        ///     If not assigned, the virtual <see cref="HandleCommand(ParseResultBase)"/> method is
        ///     called.
        /// </summary>
        public Func<ParseResultBase, int> Handler
        {
            get => _handler ?? HandleCommand;
            set => _handler = value;
        }

        protected virtual int HandleCommand(ParseResultBase parseResult)
        {
            return HandleCommand();
        }

        protected virtual int HandleCommand()
        {
            return 0;
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

    // Dynamically discover commands in assemblies.
    public partial class Command : Arg
    {
        internal IDictionary<Type, Type> DiscoveredCommands { get; } = new Dictionary<Type, Type>();

        public void ScanEntryAssemblyForCommands(Func<Type, bool> typePredicate = null)
        {
            ScanAssembliesForCommands(new[] { Assembly.GetEntryAssembly() }, typePredicate);
        }

        public void ScanAssembliesForCommands(params Assembly[] assemblies)
        {
            ScanAssembliesForCommands((IEnumerable<Assembly>)assemblies);
        }

        public void ScanAssembliesForCommands(IEnumerable<Assembly> assemblies, Func<Type, bool> typePredicate = null)
        {
            if (assemblies is null)
                throw new ArgumentNullException(nameof(assemblies));

            var discoveredCommands = new List<(Type commandType, Type parentType)>();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly is null)
                    throw new ArgumentException("Assembly should not be null.", nameof(assemblies));

                // Look for any types that derive from Command and have a parameterless constructor.
                // Get the command type and any parent command type specified by the Command attribute.
                var assemblyCommands = assembly.GetExportedTypes()
                    .Where(type => typePredicate is null || typePredicate(type))
                    .Where(type => typeof(Command).IsAssignableFrom(type))
                    .Where(type => type.GetConstructor(Type.EmptyTypes) != null)
                    .Select(type => (type, type.GetCustomAttribute<CommandAttribute>(true)?.ParentType));
                discoveredCommands.AddRange(assemblyCommands);
            }

            // Add all discovered commands with a non-null parent type to the DiscoveredCommands
            // dictionary.
            var nonRootCommands = discoveredCommands.Where(c => c.parentType != null);
            foreach (var (commandType, parentType) in nonRootCommands)
            {
                DebugOutput.Write($"Discovered child: {commandType.FullName} of {parentType.FullName}");
                DiscoveredCommands.Add(commandType, parentType);
            }

            // Since we are scanning from this instance, then this command is a root command.
            // Any discovered commands that do not have a parent type will be children of this
            // command.
            try
            {
                var rootCommands = discoveredCommands.Where(c => c.parentType is null);
                foreach (var (commandType, _) in rootCommands)
                {
                    DebugOutput.Write($"Discovered root: {commandType.FullName}");
                    var command = (Command)Activator.CreateInstance(commandType);
                    if (command.Name != null)
                        Commands.Add(command);
                }
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                // TargetInvocationException can happen when using Activator.CreateInstance.
                // Catch them and throw the inner exception instead, as that's the true exception.
                throw ex.InnerException;
            }
        }
    }

    public delegate string CommandCustomValidator(IReadOnlyList<object> arguments,
        IReadOnlyDictionary<string, object> options);
}
