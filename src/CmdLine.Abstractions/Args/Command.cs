// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using ConsoleFx.CmdLine.Internals;

namespace ConsoleFx.CmdLine
{
    [DebuggerDisplay("Command {Name}")]
    public abstract partial class Command : Arg
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<Arguments> _arguments;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<Options> _options;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<Commands> _commands;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Command" /> class.
        ///     <para />
        ///     This constructor tries to read the command details from the
        ///     <see cref="CommandAttribute"/> attribute.
        /// </summary>
        protected Command()
        {
            _arguments = new Lazy<Arguments>(InitializeArguments);
            _options = new Lazy<Options>(InitializeOptions);
            _commands = new Lazy<Commands>(InitializeCommands);

            // Command and ConsoleProgram need to process different types of attributes. Since ConsoleProgram
            // derives from Command, it will inherit the attribute processing behavior from Command,
            // which we don't want. So, we use reflection to call a predefined function (ProcessAttribute),
            // without the inheritance behavior.
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
            MethodInfo processAttributeMethod = GetType().GetMethod(nameof(ProcessAttribute),
                BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                Type.EmptyTypes,
                modifiers: null);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
            processAttributeMethod?.Invoke(this, parameters: null);
        }

        protected void ProcessAttribute()
        {
            // Read the command attribute on this class.
            CommandAttribute commandAttribute = GetType().GetCustomAttribute<CommandAttribute>(true);
            if (commandAttribute is null)
                throw new InvalidOperationException($"The command type '{GetType()}' is not decorated with the '{typeof(CommandAttribute)}' attribute.");

            // Add names from attribute
            foreach (string name in commandAttribute.Names)
                AddName(name);

            // The parent type is same as current type.
            if (commandAttribute.ParentType == GetType())
                throw new InvalidOperationException($"Parent command type of {GetType()} command cannot be the same type");
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
                while (currentCommand.ParentCommand is not null)
                    currentCommand = currentCommand.ParentCommand;
                return currentCommand;
            }
        }

        /// <summary>
        ///     Gets the collection of <see cref="Argument" /> objects for this command.
        /// </summary>
        public Arguments Arguments => _arguments.Value;

        private Arguments InitializeArguments()
        {
            Arguments arguments = new();

            // Add arguments from the GetArgs method.
            arguments.AddRange(GetArgs().OfType<Argument>());

            // Add arguments from the Argument attribute on properties in this class.
            IEnumerable<Argument> propertyArguments = GetPropertyArgs<Argument, ArgumentAttribute>(
                attr => new Argument(attr.Order, attr.Optional, attr.MaxOccurences));
            arguments.AddRange(propertyArguments);

            // Any additional setup on the arguments.
            SetupArguments(arguments);

            return arguments;
        }

        /// <summary>
        ///     Gets the collection of <see cref="Option" /> objects for this command.
        /// </summary>
        public Options Options => _options.Value;

        private Options InitializeOptions()
        {
            Options options = new();

            // Add options from the GetArgs method.
            options.AddRange(GetArgs().OfType<Option>());

            // Add options from the Option and Flag attributes on properties in this class.
            IEnumerable<Option> propertyOptions = GetPropertyArgs<Option, OptionAttribute>(
                attr => new Option(attr.Names));
            options.AddRange(propertyOptions);

            IEnumerable<Option> propertyFlags = GetPropertyArgs<Option, FlagAttribute>(
                attr => new Option(attr.Names));
            options.AddRange(propertyFlags);

            // Add any universal options.
            // Universal options that apply to all commands are only specified at the root
            // command level.
            // It wouldn't make sense for non-root commands to specify universal options.
            IEnumerable<Option> additionalOptions = RootCommand.GetUniversalOptions();
            foreach (Option option in additionalOptions)
                options.Add(option);

            // Any additional setup on the options.
            SetupOptions(options);

            return options;
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
        private IEnumerable<TArg> GetPropertyArgs<TArg, TArgAttribute>(Func<TArgAttribute, TArg> argFactory)
            where TArg : ArgumentOrOption<TArg>
            where TArgAttribute : Attribute
        {
            IEnumerable<PropertyInfo> argProperties = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.GetCustomAttribute<TArgAttribute>(true) is not null);

            foreach (PropertyInfo property in argProperties)
            {
                // Property should be read/write. Read-only is allowed if the property is a collection.
                bool isCollectionProperty = property.IsCollectionProperty();
                if (isCollectionProperty)
                {
                    if (!property.CanRead)
                        throw new ParserException(-1, $"Property {property.Name} on {property.DeclaringType} cannot be decorated with the {typeof(TArgAttribute).Name} attribute because it is not a readable property.");
                }
                else if (!property.CanRead || !property.CanWrite)
                    throw new ParserException(-1, $"Property {property.Name} on {property.DeclaringType} cannot be decorated with the {typeof(TArgAttribute).Name} attribute because it is not a read/write property.");

                // Property should not be indexed
                if (property.GetIndexParameters().Length > 0)
                    throw new ParserException(-1, $"Property {property.Name} on {property.DeclaringType} cannot be decorated with the {typeof(TArgAttribute).Name} attribute because it is an indexed property.");

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

                arg.ValidateUnderlyingProperty();

                // Apply any metadata attributes to the arg
                IEnumerable<MetadataAttribute> metadataAttrs = property.GetCustomAttributes<MetadataAttribute>(true);
                foreach (MetadataAttribute metadataAttr in metadataAttrs)
                    metadataAttr.AssignMetadata(arg);

                yield return arg;
            }
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
        public Commands Commands => _commands.Value;

        private Commands InitializeCommands()
        {
            Commands commands = new(this);

            // Add subcommands from the GetArgs method.
            commands.AddRange(GetArgs().OfType<Command>());

            // Add subcommands from any properties on this class that are derived from Command.
            commands.AddRange(GetCommandsFromProperties());

            // Add the subcommands from the DiscoveredCommands collection created from the
            // ScanAssembliesForCommands method in the root command.
            IEnumerable<Type> childCommandTypes = RootCommand.DiscoveredCommands
                .Where(kvp => kvp.Value == GetType())
                .Select(kvp => kvp.Key);
            foreach (Type childCommandType in childCommandTypes)
            {
                var command = (Command)Activator.CreateInstance(childCommandType);
                if (command.Name is not null)
                    commands.Add(command);
            }

            return commands;
        }

        private IEnumerable<Command> GetCommandsFromProperties()
        {
            // Get all properties that derive from Command. We want to get properties from base classes
            // as well, but not the Command class itself, which has a couple of matching properties
            // like RootCommand and ParentCommand, which don't want to consider.
            IEnumerable<PropertyInfo> commandProperties = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => typeof(Command).IsAssignableFrom(pi.PropertyType) && pi.DeclaringType != typeof(Command));

            foreach (PropertyInfo commandProperty in commandProperties)
            {
                // Property should be readable
                if (!commandProperty.CanRead)
                    throw new ParserException(-1, $"Property {commandProperty.Name} on {commandProperty.DeclaringType} cannot be used as a command because it cannot be read.");

                // Property should not be indexed
                if (commandProperty.GetIndexParameters().Length > 0)
                    throw new ParserException(-1, $"Property {commandProperty.Name} on {commandProperty.DeclaringType} cannot be used as a command because it is indexed.");

                var command = (Command)commandProperty.GetValue(this);

                // Apply any metadata attributes to the arg
                IEnumerable<MetadataAttribute> metadataAttrs = commandProperty
                    .GetCustomAttributes<MetadataAttribute>(true);
                foreach (MetadataAttribute metadataAttr in metadataAttrs)
                    metadataAttr.AssignMetadata(command);

                yield return command;
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
        ///     Override this method to perform any final setup on the arguments for this command.
        /// </summary>
        /// <param name="arguments">The list of arguments for this command.</param>
        protected virtual void SetupArguments(Arguments arguments)
        {
        }

        /// <summary>
        ///     Override this method to perform any final setup on the options for this command,.
        /// </summary>
        /// <param name="options">The list of options for this command.</param>
        protected virtual void SetupOptions(Options options)
        {
        }

        /// <summary>
        ///     Override this method to perform any additional custom validations after the args have
        ///     been parsed.
        /// </summary>
        /// <param name="arguments">The arguments parsed from the args.</param>
        /// <param name="options">The options parsed from the args.</param>
        /// <returns>An error message if there are validation errors, otherwise <c>null</c>.</returns>
        //TODO: Change return type to something more descriptive
        public virtual string ValidateParseResult(IReadOnlyList<object> arguments,
            IReadOnlyDictionary<string, object> options)
        {
            return null;
        }

        public virtual string Validate()
        {
            return null;
        }

        internal virtual Task<int> HandleCommandAsync(IParseResult parseResult)
        {
            return Task.FromResult(HandleCommand(parseResult));
        }

        protected virtual int HandleCommand(IParseResult parseResult)
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
    }
}
