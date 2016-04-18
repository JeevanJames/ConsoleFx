using System.Collections.Generic;

using ConsoleFx.Parser;

namespace ConsoleFx.Programs
{
    public abstract class CommandBuilder
    {
        protected abstract string Name { get; }

        protected virtual IEnumerable<Argument> Arguments
        {
            get { yield break; }
        }

        protected virtual IEnumerable<Option> Options
        {
            get { yield break; }
        }

        protected virtual IEnumerable<Command> Commands
        {
            get { yield break; }
        }

        public Command ToCommand()
        {
            var command = new Command(Name);
            foreach (Argument argument in Arguments)
                command.Arguments.Add(argument);
            foreach (Option option in Options)
                command.Options.Add(option);
            foreach (Command subcommand in Commands)
                command.Commands.Add(subcommand);
            return command;
        }

        public static implicit operator Command(CommandBuilder builder) => builder.ToCommand();
    }
}
