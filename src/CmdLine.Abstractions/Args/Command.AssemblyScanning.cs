#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

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
using System.Linq;
using System.Reflection;

namespace ConsoleFx.CmdLine
{
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
                IEnumerable<(Type type, Type ParentType)> assemblyCommands = assembly.GetExportedTypes()
                    .Where(type => typePredicate is null || typePredicate(type))
                    .Where(type => typeof(Command).IsAssignableFrom(type))
                    .Where(type => type.GetConstructor(Type.EmptyTypes) != null)
                    .Select(type => (type, type.GetCustomAttribute<CommandAttribute>(true)?.ParentType));
                discoveredCommands.AddRange(assemblyCommands);
            }

            // Add all discovered commands with a non-null parent type to the DiscoveredCommands
            // dictionary. These will be used when their corresponding parent commands are instantiated.
            IEnumerable<(Type commandType, Type parentType)> nonRootCommands = discoveredCommands
                .Where(c => c.parentType != null);
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
                IEnumerable<(Type commandType, Type parentType)> rootCommands = discoveredCommands
                    .Where(c => c.parentType is null);
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
}
