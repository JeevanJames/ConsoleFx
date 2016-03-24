using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleFx.Parser
{
    public sealed class Command
    {
        /// <summary>
        /// Creates the root command, which has no name.
        /// </summary>
        internal Command()
        {
        }

        public Command(string name, bool caseSensitive = false)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (!NamePattern.IsMatch(name))
                throw new ArgumentException($"'{name}' is not a valid command name. Command names should only consist of alphanumeric characters.", nameof(name));
            Name = name;
            NameComparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        private static readonly Regex NamePattern = new Regex(@"^\w+$", RegexOptions.Compiled);

        public string Name { get; }

        public bool CaseSensitive => NameComparison == StringComparison.Ordinal;

        internal StringComparison NameComparison { get; }

        public Arguments Arguments { get; } = new Arguments();

        public Options Options { get; } = new Options();

        public Commands Commands { get; } = new Commands();

        public Argument AddArgument(bool optional = false)
        {
            var argument = new Argument
            {
                IsOptional = optional
            };
            Arguments.Add(argument);
            return argument;
        }

        public Option AddOption(string name, string shortName = null, bool caseSensitive = false,
            int order = int.MaxValue)
        {
            var option = new Option(name)
            {
                CaseSensitive = caseSensitive,
                Order = order
            };
            if (!string.IsNullOrWhiteSpace(shortName))
                option.ShortName = shortName;

            Options.Add(option);
            return option;
        }
    }

    public sealed class Commands : Collection<Command>
    {
        public Command this[string name] =>
            this.FirstOrDefault(command => command.Name.Equals(name, command.NameComparison));

        protected override void InsertItem(int index, Command command)
        {
            CheckDuplicates(command, -1);
            base.InsertItem(index, command);
        }

        protected override void SetItem(int index, Command command)
        {
            CheckDuplicates(command, index);
            base.SetItem(index, command);
        }

        private void CheckDuplicates(Command command, int index)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (i == index)
                    continue;
                if (this[i].Name.Equals(command.Name, this[i].NameComparison))
                    throw new ArgumentException($"Command named '{command.Name}' already exists in the command collection.");
            }
        }
    }
}
