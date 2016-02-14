using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;

namespace ConsoleFx.Programs
{
    public sealed class MultiCommandProgram
    {
        public ParserStyle ParserStyle { get; }
        private CommandCollection Commands { get; } = new CommandCollection();

        public MultiCommandProgram(ParserStyle parserStyle)
        {
            if (parserStyle == null)
                throw new ArgumentNullException(nameof(parserStyle));
            ParserStyle = parserStyle;
        }

        public TCommand RegisterCommand<TCommand>(TCommand command)
            where TCommand : Command
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            command.SetParserStyle(ParserStyle);
            Commands.Add(command);
            return command;
        }

        public TCommand RegisterCommand<TCommand>(Action<TCommand> initializer = null)
            where TCommand : Command, new()
        {
            var command = new TCommand();
            initializer?.Invoke(command);
            return RegisterCommand(command);
        }

        public int Run()
        {
            string[] allArgs = Environment.GetCommandLineArgs();
            string commandName = allArgs[1];
            Command command = Commands.FindCommand(commandName);
            if (command == null)
                throw new InvalidOperationException($"Unrecognized command: {commandName}");
            IEnumerable<string> commandArgs = allArgs.Skip(2);
            return command.Run(commandArgs);
        }
    }

    public abstract class Command : BaseCommand
    {
        protected Command(IEnumerable<string> names, ArgGrouping grouping = ArgGrouping.DoesNotMatter) : base(null)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));
            CheckDuplicateNames(names);
            Names = new List<string>(names);
            Grouping = grouping;
        }

        /// <summary>
        ///     Gets the name or names of the command.
        /// </summary>
        public IReadOnlyList<string> Names { get; }

        /// <summary>
        ///     Called by the MultiCommandProgram to set the parser style.
        /// </summary>
        /// <param name="parserStyle">The parser style to set.</param>
        internal void SetParserStyle(ParserStyle parserStyle)
        {
            ParserStyle = parserStyle;
        }

        public int Run(IEnumerable<string> args) => CoreRun(args);

        private static void CheckDuplicateNames(IEnumerable<string> names)
        {
            //Group the names, without case sensitivity
            IEnumerable<string> duplicateNames = names
                .GroupBy(name => name, name => name, StringComparer.OrdinalIgnoreCase)
                .Where(group => @group.Count() > 1)
                .Select(group => @group.Key);
            if (duplicateNames.Any())
            {
                string duplicateNamesString = duplicateNames.Aggregate(new StringBuilder(), (sb, name) => {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(name);
                    return sb;
                }).ToString();
                throw new ArgumentException($"The following command names have duplicates: {duplicateNamesString}",
                    nameof(names));
            }
        }
    }

    internal sealed class CommandCollection : Collection<Command>
    {
        internal Command FindCommand(string name)
        {
            foreach (Command command in this)
            {
                if (command.Names.Any(n => n.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    return command;
            }
            return null;
        }

        protected override void InsertItem(int index, Command command)
        {
            EnsureCommandNamesAreUnique(command);
            base.InsertItem(index, command);
        }

        protected override void SetItem(int index, Command command)
        {
            EnsureCommandNamesAreUnique(command, index);
            base.SetItem(index, command);
        }

        private void EnsureCommandNamesAreUnique(Command newCommand, int? index = null)
        {
            for (int i = 0; i < Count; i++)
            {
                if (index.HasValue && index.Value == i)
                    continue;
                Command command = this[i];
                foreach (string existingName in command.Names)
                {
                    if (newCommand.Names.Any(newName => newName.Equals(existingName, StringComparison.OrdinalIgnoreCase)))
                        throw new Exception($"A command named '{existingName}' already exists.");
                }
            }
        }
    }

    public sealed class HelpCommand : Command
    {
        public HelpCommand() : base(new[] { "help", "h", "?" })
        {
        }

        public string Command { get; set; }

        public bool Detailed { get; set; }

        protected override int Handle()
        {
            if (string.IsNullOrWhiteSpace(Command))
                Console.WriteLine(@"Displaying generic help");
            else
                Console.WriteLine($"Help for command {Command}");
            if (Detailed)
                Console.WriteLine(@"Displaying detailed help");
            return 0;
        }

        protected override IEnumerable<Argument> GetArguments()
        {
            yield return CreateArgument(true).AssignTo(() => Command);
        }

        protected override IEnumerable<Option> GetOptions()
        {
            yield return CreateOption("details", "d")
                .Flag(() => Detailed);
        }
    }
}
