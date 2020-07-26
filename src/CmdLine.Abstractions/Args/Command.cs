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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsoleFx.CmdLine
{
    [DebuggerDisplay("Command {Name}")]
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
        private Func<IParseResult, Task<int>> _handler;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Command" /> class.
        ///     <para />
        ///     This constructor tries to read the command details from the
        ///     <see cref="CommandAttribute"/> attribute.
        /// </summary>
        public Command()
        {
            // Read the command attribute on this class.
            CommandAttribute commandAttribute = GetType().GetCustomAttribute<CommandAttribute>(true);
            if (commandAttribute is null)
                throw new InvalidOperationException($"The command type '{GetType().FullName}' is not decorated with the '{typeof(CommandAttribute).FullName}' attribute.");

            // Add names from attribute
            foreach (string name in commandAttribute.Names)
                AddName(name);

            // The parent type is same as current type.
            if (commandAttribute.ParentType == GetType())
                throw new InvalidOperationException($"Parent command type of {GetType().FullName} command cannot be the same type");

            ProcessMetadataAttributes();
        }

        public Command(bool caseSensitive, params string[] names)
        {
            ProcessMetadataAttributes();
            foreach (string name in names)
                AddName(name, caseSensitive);
        }

        public Command(params string[] names)
            : this(caseSensitive: false, names)
        {
        }

        /// <summary>
        ///     Read any metadata attributes on this class and assign them to this command.
        /// </summary>
        private void ProcessMetadataAttributes()
        {
            IEnumerable<MetadataAttribute> metadataAttributes = GetType().GetCustomAttributes<MetadataAttribute>(inherit: true);
            foreach (MetadataAttribute metadataAttribute in metadataAttributes)
                metadataAttribute.AssignMetadata(this);
        }

        /// <summary>
        ///     Gets a reference to the parent <see cref="Command"/> of this instance.
        /// </summary>
        public Command ParentCommand { get; internal set; }

        /// <summary>
        ///     Gets the reference to the root <see cref="Command"/> instance.
        /// </summary>
        public Command RootCommand
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

                    // Add arguments from the GetArgs method.
                    _arguments.AddRange(GetArgs().OfType<Argument>());

                    // Add arguments from the properties in this class.
                    IReadOnlyList<Argument> propertyArguments = GetPropertyArgs<Argument, ArgumentAttribute>(
                        attr => new Argument(attr.Order, attr.Optional, attr.MaxOccurences));
                    _arguments.AddRange(propertyArguments);

                    SetupArguments(_arguments);
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

                    // Add options from the GetArgs method.
                    _options.AddRange(GetArgs().OfType<Option>());

                    // Add options from the properties in this class.
                    IReadOnlyList<Option> propertyOptions = GetPropertyArgs<Option, OptionAttribute>(
                        attr => new Option(attr.Names));
                    _options.AddRange(propertyOptions);

                    IReadOnlyList<Option> propertyFlags = GetPropertyArgs<Option, FlagAttribute>(
                        attr => new Option(attr.Names));
                    _options.AddRange(propertyFlags);

                    // Add any universal options.
                    // Universal options that apply to all commands are only specified at the root
                    // command level.
                    // It wouldn't make sense for non-root commands to specify universal options.
                    IEnumerable<Option> additionalOptions = RootCommand.GetUniversalOptions();
                    foreach (Option option in additionalOptions)
                        _options.Add(option);

                    SetupOptions(_options);
                }

                return _options;
            }
        }

        /// <summary>
        ///     Common method to read the properties of this <see cref="Command"/> class and create
        ///     <see cref="Argument"/> and <see cref="Option"/> instances from the property metadata.
        /// </summary>
        /// <typeparam name="TArg">
        ///     The type of arg being handled, <see cref="Argument"/> or <see cref="Option"/>.
        /// </typeparam>
        /// <typeparam name="TArgAttribute">The type of sttribute identifying the arg.</typeparam>
        /// <param name="argFactory">A delegate that creates the arg from the attribute.</param>
        /// <returns>An instance of the arg, based on the property's metadata.</returns>
        private IReadOnlyList<TArg> GetPropertyArgs<TArg, TArgAttribute>(Func<TArgAttribute, TArg> argFactory)
            where TArg : ArgumentOrOption<TArg>
            where TArgAttribute : Attribute
        {
            IEnumerable<PropertyInfo> argProperties = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.GetCustomAttribute<TArgAttribute>(true) != null);

            if (!argProperties.Any())
#if NET45
                return new TArg[0];
#else
                return Array.Empty<TArg>();
#endif

            var args = new List<TArg>();

            foreach (PropertyInfo property in argProperties)
            {
                // Verify property is usable
                if (!property.CanRead || !property.CanWrite)
                    throw new ParserException(-1, $"Property {property.Name} on {property.DeclaringType.FullName} cannot be decorated with an {typeof(TArgAttribute).Name} attribute because it is not a read/write property.");

                //TODO: More checks

                // Create the arg instance
                TArgAttribute attribute = property.GetCustomAttribute<TArgAttribute>(true);
                TArg arg = argFactory(attribute);
                arg.AssignedProperty = property;

                // Apply any IArgApplicator<> attributes to the arg
                IEnumerable<IArgApplicator<TArg>> applicatorAttrs = property
                    .GetCustomAttributes()
                    .OfType<IArgApplicator<TArg>>();
                foreach (IArgApplicator<TArg> applicatorAttr in applicatorAttrs)
                    applicatorAttr.Apply(arg, property);

                // Apply any metadata attributes to the arg
                IEnumerable<MetadataAttribute> metadataAttrs = property.GetCustomAttributes<MetadataAttribute>(true);
                foreach (MetadataAttribute metadataAttr in metadataAttrs)
                    metadataAttr.AssignMetadata(arg);

                args.Add(arg);
            }

            return args;
        }

        /// <summary>
        ///     Override this method to specify options that apply to all options.
        ///     <para />
        ///     This method should only be overridden in a root command object, as that is the only
        ///     one that is considered. The method is not called for non-root commands.
        /// </summary>
        /// <returns>Options that apply to all commands.</returns>
        protected virtual IEnumerable<Option> GetUniversalOptions()
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
                if (_commands is null)
                {
                    _commands = new Commands(this);

                    // Add subcommands from the GetArgs method.
                    _commands.AddRange(GetArgs().OfType<Command>());

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

        protected virtual void SetupArguments(Arguments arguments)
        {
        }

        protected virtual void SetupOptions(Options options)
        {
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
        ///     If not assigned, the virtual <see cref="HandleCommandAsync(IParseResult)"/> method is
        ///     called.
        /// </summary>
        public Func<IParseResult, Task<int>> Handler
        {
            get => _handler ?? HandleCommandAsync;
            set => _handler = value;
        }

        protected virtual Task<int> HandleCommandAsync(IParseResult parseResult)
        {
            return HandleCommandAsync();
        }

        protected virtual Task<int> HandleCommandAsync()
        {
            return Task.FromResult(HandleCommand());
        }

        protected virtual int HandleCommand()
        {
            return 0;
        }

        public virtual void DisplayHelp(Command command = null)
        {
            RootCommand.DisplayHelp(command ?? this);
        }

        public Argument AddArgument(int order = 0, bool isOptional = false, byte maxOccurences = 1)
        {
            var argument = new Argument(order, isOptional, maxOccurences);
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
    }
}
