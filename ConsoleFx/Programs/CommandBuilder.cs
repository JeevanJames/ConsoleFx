using System.CodeDom;
using System.Collections.Generic;

using ConsoleFx.Parser;

namespace ConsoleFx.Programs
{
    public abstract class CommandBuilder
    {
        protected abstract string Name { get; }

        protected virtual IEnumerable<Argument> Arguments
        {
            get { yield break;}
        }

        protected virtual IEnumerable<Option> Options
        {
            get { yield break; }
        }

        protected virtual IEnumerable<Command> Commands
        {
            get { yield break; }
        }

        public static implicit operator Command(CommandBuilder builder)
        {
            var command = new Command(builder.Name);
            foreach (Argument argument in builder.Arguments)
                command.Arguments.Add(argument);
            foreach (Option option in builder.Options)
                command.Options.Add(option);
            foreach (Command subcommand in builder.Commands)
                command.Commands.Add(subcommand);
            return command;
        }
    }
}