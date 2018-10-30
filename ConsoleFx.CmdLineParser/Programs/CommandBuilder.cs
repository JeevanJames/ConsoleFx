﻿#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2018 Jeevan James

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

namespace ConsoleFx.CmdLineParser.Programs
{
    /// <summary>
    ///     Helper class that is useful for creating <see cref="Command" /> objects using inheritance semantics.
    ///     Simply override the necessary members and then convert to a <see cref="Command" /> object using the
    ///     <see cref="ToCommand" /> method or using an implicit cast.
    /// </summary>
    public abstract class CommandBuilder
    {
        /// <summary>
        ///     Name of the command.
        /// </summary>
        protected abstract string Name { get; }

        /// <summary>
        ///     Optional description of the command. Useful for usage builders.
        /// </summary>
        protected virtual string Description => null;

        /// <summary>
        ///     Override this method to return the arguments available to this command.
        ///     Remember that optional arguments must be after required arguments.
        /// </summary>
        protected virtual IEnumerable<Argument> Arguments
        {
            get { yield break; }
        }

        /// <summary>
        ///     Override this method to return the options available to this command.
        /// </summary>
        protected virtual IEnumerable<Option> Options
        {
            get { yield break; }
        }

        /// <summary>
        ///     Override this method to return the sub-commands available to this command.
        /// </summary>
        protected virtual IEnumerable<Command> Commands
        {
            get { yield break; }
        }

        /// <summary>
        ///     Creates a <see cref="Command" /> instance from this command builder.
        /// </summary>
        /// <returns>A <see cref="Command" /> instance.</returns>
        public Command ToCommand()
        {
            var command = new Command(Name);
            foreach (Argument argument in Arguments)
                command.Arguments.Add(argument);
            foreach (Option option in Options)
                command.Options.Add(option);
            foreach (Command subcommand in Commands)
                command.Commands.Add(subcommand);
            if (!string.IsNullOrWhiteSpace(Description))
                command.Description(Description);
            return command;
        }

        /// <summary>
        ///     Implicit cast operator to convert a <see cref="CommandBuilder" /> instance to a <see cref="Command" /> instance.
        ///     This means that you can pass a <see cref="CommandBuilder" /> instance to any method that expects a
        ///     <see cref="Command" /> instance; for example when adding sub-commands to a <see cref="Command" /> object.
        ///     For languages that do not support implicit cast operators, use the <see cref="ToCommand" /> method.
        /// </summary>
        /// <param name="builder"></param>
        public static implicit operator Command(CommandBuilder builder) => builder.ToCommand();
    }
}