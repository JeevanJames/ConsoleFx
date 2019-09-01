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
        private Func<ParseResultBase, int> _handler;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Command" /> class.
        ///     <para />
        ///     This constructor tries to read the command details from the
        ///     <see cref="CommandAttribute"/> attribute.
        /// </summary>
        public Command()
        {
            ProcessCommandAttributes();
        }

        public Command(bool caseSensitive, params string[] names)
            : this()
        {
            foreach (string name in names)
                AddName(name, caseSensitive);
        }

        public Command(params string[] names)
            : this(caseSensitive: false, names)
        {
        }

        /// <summary>
        ///     Handle any attributes specified on the command.
        /// </summary>
        private void ProcessCommandAttributes()
        {
            // Read the command attribute on this class.
            CommandAttribute commandAttribute = GetType().GetCustomAttribute<CommandAttribute>(true);
            if (commandAttribute != null)
            {
                // Add names from attribute
                foreach (string name in commandAttribute.Names)
                    AddName(name);

                // The parent type is same as current type.
                if (commandAttribute.ParentType == GetType())
                    throw new InvalidOperationException($"Parent command type of {GetType().FullName} command cannot be the same type");
            }

            // Read any metadata attributes and assign them to this command.
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
                        attr => new Argument(attr.Name, attr.IsOptional, attr.MaxOccurences));
                    _arguments.AddRange(propertyArguments);
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
                        attr => new Option(attr.Name));
                    _options.AddRange(propertyOptions);

                    // Add any universal options.
                    // Universal options that apply to all commands are only specified at the root
                    // command level.
                    // It wouldn't make sense for non-root commands to specify universal options.
                    IEnumerable<Option> additionalOptions = RootCommand.GetUniversalOptions();
                    foreach (Option option in additionalOptions)
                        _options.Add(option);
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
            where TArg : Arg
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
                // Create the arg instance
                TArgAttribute attribute = property.GetCustomAttribute<TArgAttribute>(true);
                TArg arg = argFactory(attribute);

                // Apply any IArgApplicator<> attributes to the arg
                IEnumerable<IArgApplicator<TArg>> applicatorAttrs = property
                    .GetCustomAttributes()
                    .OfType<IArgApplicator<TArg>>();
                foreach (IArgApplicator<TArg> applicatorAttr in applicatorAttrs)
                    applicatorAttr.Apply(arg);

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

        public virtual void DisplayHelp(Command command = null)
        {
            RootCommand.DisplayHelp(command ?? this);
        }

        public Argument AddArgument(string name, bool isOptional = false, byte maxOccurences = 1)
        {
            var argument = new Argument(name, isOptional, maxOccurences);
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
        /// <summary>
        ///     Gets the commands that are discovered by calling one of the <c>ScanAssemblies</c>
        ///     methods.
        /// </summary>
        internal IDictionary<Type, Type> DiscoveredCommands { get; } = new Dictionary<Type, Type>();

        /// <summary>
        ///     Scans the entry assembly and locates any commands objects. Add applicable commands
        ///     as sub-commands to this command.
        /// </summary>
        /// <param name="typePredicate">
        ///     Optional predicate that can be used to filter the discovered commands.
        /// </param>
        public void ScanEntryAssemblyForCommands(Func<Type, bool> typePredicate = null)
        {
            ScanAssembliesForCommands(new[] { Assembly.GetEntryAssembly() }, typePredicate);
        }

        /// <summary>
        ///     Scans the specified <paramref name="assemblies"/> and locates any commands objects.
        ///     Add applicable commands as sub-commands to this command.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan.</param>
        public void ScanAssembliesForCommands(params Assembly[] assemblies)
        {
            ScanAssembliesForCommands((IEnumerable<Assembly>)assemblies);
        }

        /// <summary>
        ///     Scans the specified <paramref name="assemblies"/> and locates any commands objects.
        ///     Add applicable commands as sub-commands to this command.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan.</param>
        /// <param name="typePredicate">
        ///     Optional predicate that can be used to filter the discovered commands.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if any assembly in the <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
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
            // dictionary. These will be used when their corresponding parent commands are instantiated.
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
                    DebugOutput.Write($"Discovered root child: {commandType.FullName}");
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

    /// <summary>
    ///     Encapsulates a method that performs custom validation once a <see cref="Command"/> instance
    ///     has been initialized.
    /// </summary>
    /// <param name="arguments">The arguments passed to the command.</param>
    /// <param name="options">The options passed to the command.</param>
    /// <returns>A validation error message, if the validation fails; otherwise <c>null</c>.</returns>
    public delegate string CommandCustomValidator(IReadOnlyList<object> arguments,
        IReadOnlyDictionary<string, object> options);
}
