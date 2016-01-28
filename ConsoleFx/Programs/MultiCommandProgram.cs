#region --- License & Copyright Notice ---
/*
ConsoleFx CommandLine Processing Library
Copyright 2015 Jeevan James

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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using ConsoleFx.Parser;
using ConsoleFx.Parser.Styles;

namespace ConsoleFx.Programs
{
    public sealed class MultiCommandProgram : ConsoleProgram
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
            command.ParserStyle = ParserStyle;
            Commands.Add(command);
            return command;
        }

        public TCommand RegisterCommand<TCommand>()
            where TCommand : Command, new() => RegisterCommand(new TCommand());

        public Command RegisterCommand(ExecuteHandler handler, IEnumerable<string> names)
        {
            var command = new DelegateCommand(handler, names);
            return RegisterCommand(command);
        }

        public Command RegisterCommand(ExecuteHandler handler, params string[] names)
            => RegisterCommand(handler, (IEnumerable<string>)names);

        protected override int InternalRun()
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

    public abstract class Command
    {
        public IReadOnlyList<string> Names { get; }
        internal ParserStyle ParserStyle { private get; set; }

        protected Command(IEnumerable<string> names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));
            CheckDuplicateNames(names);
            Names = new List<string>(names);
        }

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

        public Argument AddArgument()
        {
            throw new NotImplementedException();
        }

        public Option AddOption()
        {
            throw new NotImplementedException();
        }

        public int Run(IEnumerable<string> args)
        {
            var parser = new Parser.Parser(ParserStyle);
            parser.Parse(args);
            return 0;
        }

        public static Command Create(ExecuteHandler handler, params string[] names)
            => new DelegateCommand(handler, names);
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

        private void EnsureCommandNamesAreUnique(Command command, int? index = null)
        {
            //TODO: Implement this
        }
    }

    public sealed class DelegateCommand : Command
    {
        public DelegateCommand(ExecuteHandler handler, IEnumerable<string> names) : base(names)
        {
        }
    }

    public sealed class HelpCommand : Command
    {
        public HelpCommand() : base(new[] { "help", "h", "?" })
        {
        }
    }
}