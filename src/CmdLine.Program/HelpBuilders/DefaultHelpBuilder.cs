#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2019 Jeevan James

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
using System.Diagnostics;
using System.Linq;
using System.Text;

using static System.Console;

namespace ConsoleFx.CmdLine.Program.HelpBuilders
{
    public class DefaultHelpBuilder : HelpBuilder
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _indent = 4;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _nameDescriptionSpacing = 3;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _newLineDescriptionIndent = 7;

        public DefaultHelpBuilder(params string[] names)
            : this(false, names)
        {
        }

        public DefaultHelpBuilder(bool caseSensitive, params string[] names)
        {
            foreach (string name in names)
                AddName(name, caseSensitive);

            Indent = 4;
            NameDescriptionSpacing = 3;
            NewLineDescriptionIndent = 7;
        }

        public override void DisplayHelp(Command command)
        {
            string usage = GetSummaryUsage(command);
            WriteLine($"Usage: {usage}");

            PrintArgs(command.Arguments, ArgumentDescriptionPlacement, "Arguments", ResolveArgumentName);
            PrintArgs(command.Options, OptionDescriptionPlacement, "Options", ResolveOptionNames);
            PrintArgs(command.Commands, CommandDescriptionPlacement, "Commands", ResolveCommandNames);
        }

        public UsageType UsageType { get; set; }

        public ArgDescriptionPlacement ArgumentDescriptionPlacement { get; set; }

        public ArgDescriptionPlacement OptionDescriptionPlacement { get; set; }

        public ArgDescriptionPlacement CommandDescriptionPlacement { get; set; }

        public int Indent
        {
            get => _indent;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Indent should be 0 or greater.", nameof(value));
                _indent = value;
                IndentStr = new string(' ', _indent);
            }
        }

        public int NameDescriptionSpacing
        {
            get => _nameDescriptionSpacing;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Spacing between name and description should be 0 or greater.", nameof(value));
                _nameDescriptionSpacing = value;
                NameDescriptionSpacingStr = new string(' ', _nameDescriptionSpacing);
            }
        }

        public int NewLineDescriptionIndent
        {
            get => _newLineDescriptionIndent;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Indentation for new line description should be 0 or greater.", nameof(value));
                _newLineDescriptionIndent = value;
                NewLineDescriptionIndentStr = new string(' ', _newLineDescriptionIndent);
            }
        }

        protected string IndentStr { get; set; }

        protected string NameDescriptionSpacingStr { get; set; }

        protected string NewLineDescriptionIndentStr { get; set; }

        /// <summary>
        ///     Prints an arg and its description.
        ///     <para />
        ///     Override this method to customize the way an arg and its description are printed.
        /// </summary>
        /// <param name="name">The name or names for the arg.</param>
        /// <param name="description">The description of the arg.</param>
        /// <param name="maxNameLength">Length pf the longest arg name.</param>
        /// <param name="placement">The placement of the description in relation to the name.</param>
        protected virtual void PrintArg(string name, string description, int maxNameLength, ArgDescriptionPlacement placement)
        {
            Write(IndentStr);

            if (placement == ArgDescriptionPlacement.SameLine)
            {
                Write($"{name.PadRight(maxNameLength)}{NameDescriptionSpacingStr}");
            }
            else
            {
                WriteLine(name);
                Write(NewLineDescriptionIndentStr);
            }

            WriteLine(description);
        }

        private string GetSummaryUsage(Command command)
        {
            var sb = new StringBuilder();
            BuildCommandNamesChain(command, sb);
            if (command.Commands.Count > 0)
                sb.Append("[command] ");
            if (command.Arguments.Count > 1)
                sb.Append("[arguments] ");
            else if (command.Arguments.Count == 1)
                sb.Append("[argument] ");
            if (command.Options.Count > 1)
                sb.Append("[options] ");
            else if (command.Options.Count == 1)
                sb.Append("[option] ");
            return sb.ToString().Trim();
        }

        private void BuildCommandNamesChain(Command command, StringBuilder sb)
        {
            Command currentCommand = command;
            while (currentCommand != null)
            {
                sb.Insert(0, currentCommand.Name + " ");
                currentCommand = currentCommand.ParentCommand;
            }
        }

        private void PrintArgs<TArg>(Args<TArg> args, ArgDescriptionPlacement placement, string defaultCategoryName,
            Func<TArg, string> nameResolver)
            where TArg : Arg
        {
            if (args.Count == 0)
                return;

            IEnumerable<IGrouping<string, TArg>> categories = args.GroupBy(arg => arg.Get<string>(HelpExtensions.Keys.CategoryName), StringComparer.OrdinalIgnoreCase);
            foreach (var category in categories)
            {
                List<TArg> categoryArgs = category
                    .Where(arg => !arg.Get<bool>(HelpExtensions.Keys.Hide))
                    .OrderBy(arg => arg.Get<int>(HelpExtensions.Keys.Order))
                    .ThenBy(arg => arg.Name)
                    .ToList();
                if (categoryArgs.Count == 0)
                    continue;

                WriteLine();
                WriteLine(category.Key ?? defaultCategoryName);

                int maxNameLength = placement == ArgDescriptionPlacement.NextLine ? 0 :
                    categoryArgs.Aggregate(0, (longest, arg) =>
                    {
                        string resolvedName = nameResolver(arg);
                        return Math.Max(longest, resolvedName.Length);
                    });
                foreach (TArg arg in categoryArgs)
                {
                    string resolvedName = nameResolver(arg);
                    string description = arg.Get<string>("Description") ?? "<No description provided>";
                    PrintArg(resolvedName, description, maxNameLength, placement);
                }
            }
        }

        private string ResolveCommandNames(Arg arg)
        {
            return arg.AllNames
                .Aggregate(new StringBuilder(), (sb, name) =>
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    return sb.Append(name);
                })
                .ToString();
        }

        private string ResolveOptionNames(Arg arg)
        {
            return arg.AllNames
                .Select(name => name.Length > 1 ? $"--{name}" : $"-{name}")
                .Aggregate(new StringBuilder(), (sb, name) =>
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    return sb.Append(name);
                })
                .ToString();
        }

        private string ResolveArgumentName(Arg arg)
        {
            string customName = arg.Get<string>("Name");
            return customName ?? arg.Name;
        }

        public override void VerifyHelp(Command command)
        {
            var allCommands = new List<Command> { command };
            int index = 0;
            while (index < allCommands.Count)
            {
                Command currentCommand = allCommands[index];

                Arguments arguments = currentCommand.Arguments;
                foreach (Argument argument in arguments)
                {
                    string description = argument.Get<string>("Description");
                    if (string.IsNullOrWhiteSpace(description))
                        throw new InvalidOperationException($"Argument '{argument.Name}' under command '{currentCommand.Name}' does not have a description.");
                }

                Options options = currentCommand.Options;
                foreach (Option option in options)
                {
                    string description = option.Get<string>("Description");
                    if (string.IsNullOrWhiteSpace(description))
                        throw new InvalidOperationException($"Option '{option.Name}' under command '{currentCommand.Name}' does not have a description.");
                }

                allCommands.AddRange(currentCommand.Commands);

                index++;
            }
        }
    }

    public class DefaultColorHelpBuilder : DefaultHelpBuilder
    {
        public DefaultColorHelpBuilder(params string[] names)
            : base(names)
        {
        }

        public DefaultColorHelpBuilder(bool caseSensitive, params string[] names)
            : base(caseSensitive, names)
        {
        }

        protected override void PrintArg(string name, string description, int maxNameLength, ArgDescriptionPlacement placement)
        {
            Write(IndentStr);

            ForegroundColor = ConsoleColor.Magenta;
            if (placement == ArgDescriptionPlacement.SameLine)
            {
                Write($"{name.PadRight(maxNameLength)}{NameDescriptionSpacingStr}");
            }
            else
            {
                WriteLine(name);
                Write(NewLineDescriptionIndentStr);
            }

            ResetColor();

            WriteLine(description);
        }
    }

    /// <summary>
    ///     Describes how the program usage is to be displayed.
    /// </summary>
    public enum UsageType
    {
        /// <summary>
        ///     The program usage is to be displayed as a summary, showing just the existence of
        ///     subcommands, arguments and options, but not mentioning them in detail.
        /// </summary>
        Summary,

        /// <summary>
        ///     The program usage is to be displayed in detail, with every argument and option
        ///     combinations mentioned.
        /// </summary>
        Detailed,
    }

    public enum ArgDescriptionPlacement
    {
        SameLine,
        NextLine,
    }
}
