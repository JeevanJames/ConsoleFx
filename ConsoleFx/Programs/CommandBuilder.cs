using System.Collections.Generic;

using ConsoleFx.Parser;

namespace ConsoleFx.Programs
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
