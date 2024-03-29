﻿// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleFx.CmdLine
{
    // Dynamically discover commands in assemblies.
    public partial class Command
    {
        /// <summary>
        ///     Gets the commands that are discovered by calling one of the <c>ScanAssemblies</c>
        ///     methods.
        /// </summary>
        internal Dictionary<Type, Type> DiscoveredCommands { get; } = new();

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
        ///     Scans all assemblies in the current app domain and locates any command objects.
        /// </summary>
        /// <param name="typePredicate">
        ///     Optional predicate that can be used to filter the discovered commands.
        /// </param>
        public void ScanAllAssembliesForCommands(Func<Type, bool> typePredicate = null)
        {
            ScanAssembliesForCommands(AppDomain.CurrentDomain.GetAssemblies(), typePredicate);
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

            var discoveredCommands = new List<(Type CommandType, Type ParentType)>();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly is null)
                    throw new ArgumentException("Assembly should not be null.", nameof(assemblies));

                // Look for any types that derive from Command and have a parameterless constructor.
                // Get the command type and any parent command type specified by the Command attribute.
                IEnumerable<(Type Type, Type ParentType)> assemblyCommands = assembly.GetExportedTypes()
                    .Where(type => (typePredicate is null || typePredicate(type))
                        && typeof(Command).IsAssignableFrom(type)
                        && type.GetConstructor(Type.EmptyTypes) is not null
                        && type != GetType())
                    .Select(type => (type, type.GetCustomAttribute<CommandAttribute>(true)?.ParentType));
                discoveredCommands.AddRange(assemblyCommands);
            }

            // Add all discovered commands with a non-null parent type to the DiscoveredCommands
            // dictionary. These will be used when their corresponding parent commands are instantiated.
            IEnumerable<(Type CommandType, Type ParentType)> nonRootCommands = discoveredCommands
                .Where(c => c.ParentType is not null);
            foreach ((Type commandType, Type parentType) in nonRootCommands)
            {
                DebugOutput.Write($"Discovered child: {commandType} of {parentType}");
                DiscoveredCommands.Add(commandType, parentType);
            }

            // Since we are scanning from this instance, then this command is a root command.
            // Any discovered commands that do not have a parent type will be children of this
            // command.
            try
            {
                IEnumerable<(Type CommandType, Type ParentType)> rootCommands = discoveredCommands
                    .Where(c => c.ParentType is null);
                foreach ((Type commandType, _) in rootCommands)
                {
                    DebugOutput.Write($"Discovered root child: {commandType}");
                    var command = (Command)Activator.CreateInstance(commandType);
                    if (command.Name is not null)
                        Commands.Add(command);
                }
            }
            catch (TargetInvocationException ex) when (ex.InnerException is not null)
            {
                // TargetInvocationException can happen when using Activator.CreateInstance.
                // Catch them and throw the inner exception instead, as that's the true exception.
                throw ex.InnerException;
            }
        }
    }
}
