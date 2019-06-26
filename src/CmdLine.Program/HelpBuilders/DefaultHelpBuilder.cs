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
using System.Linq;
using System.Text;

using static System.Console;

namespace ConsoleFx.CmdLine.Program.HelpBuilders
{
    public class DefaultHelpBuilder : HelpBuilder
    {
        public DefaultHelpBuilder(params string[] names)
        {
            foreach (string name in names)
                AddName(name);
        }

        public DefaultHelpBuilder(bool caseSensitive, params string[] names)
        {
            foreach (string name in names)
                AddName(name, caseSensitive);
        }

        public override void DisplayHelp(Command command)
        {
            string usage = GetSummaryUsage(command);
            WriteLine($"Usage: {usage}");

            PrintArgs(command.Arguments, ArgumentDescriptionPlacement, "Arguments");
            PrintOptions(command.Options, OptionDescriptionPlacement, "Options");
            PrintArgs(command.Commands, CommandDescriptionPlacement, "Commands");
        }

        public UsageType UsageType { get; set; }

        public ArgDescriptionPlacement ArgumentDescriptionPlacement { get; set; }

        public ArgDescriptionPlacement OptionDescriptionPlacement { get; set; }

        public ArgDescriptionPlacement CommandDescriptionPlacement { get; set; }

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

        private void PrintArgs<TArg>(Args<TArg> args, ArgDescriptionPlacement placement, string defaultCategoryName)
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
                if (placement == ArgDescriptionPlacement.NextLine)
                    PrintArgsOnNextLine(categoryArgs);
                else
                    PrintArgsOnSameLine(categoryArgs);
            }
        }

        private void PrintArgsOnSameLine<TArg>(IReadOnlyList<TArg> args)
            where TArg : Arg
        {
            int longestLength = args.Aggregate(0, (longest, arg) =>
            {
                string resolvedName = ResolveName(arg);
                return Math.Max(longest, resolvedName.Length);
            });

            foreach (TArg arg in args)
            {
                string name = ResolveName(arg);
                string description = arg.Get<string>("Description") ?? "<No description provided>";
                WriteLine($"{Indent}{name.PadRight(longestLength)}   {description}");
            }
        }

        private void PrintArgsOnNextLine<TArg>(IReadOnlyList<TArg> args)
            where TArg : Arg
        {
            foreach (TArg arg in args)
            {
                WriteLine($"{Indent}{ResolveName(arg)}");
                string description = arg.Get<string>("Description") ?? "<No description provided>";
                WriteLine($"{Indent}{Indent}{description}");
                WriteLine();
            }
        }

        private void PrintOptions(Options options, ArgDescriptionPlacement placement, string defaultCategoryName)
        {
            if (options.Count == 0)
                return;

            IEnumerable<IGrouping<string, Option>> categories = options.GroupBy(option => option.Get<string>(HelpExtensions.Keys.CategoryName), StringComparer.OrdinalIgnoreCase);
            foreach (IGrouping<string, Option> category in categories)
            {
                List<Option> categoryOptions = category
                    .Where(option => !option.Get<bool>(HelpExtensions.Keys.Hide))
                    .OrderBy(option => option.Get<int>(HelpExtensions.Keys.Order))
                    .ThenBy(option => option.Name)
                    .ToList();
                if (categoryOptions.Count == 0)
                    continue;

                WriteLine();
                WriteLine(category.Key ?? defaultCategoryName);
                if (placement == ArgDescriptionPlacement.NextLine)
                    PrintOptionsOnNextLine(categoryOptions);
                else
                    PrintOptionsOnSameLine(categoryOptions);
            }
        }

        private void PrintOptionsOnNextLine(List<Option> options)
        {
            foreach (Option option in options)
            {
                string names = BuildCombinedOptionsName(option);
                WriteLine($"{Indent}{names}");

                string description = option.Get<string>("Description") ?? "<No description provided>";
                WriteLine($"{Indent}{Indent}{description}");

                WriteLine();
            }
        }

        private void PrintOptionsOnSameLine(List<Option> options)
        {
            int longestLength = options.Aggregate(0, (longest, option) =>
            {
                string names = BuildCombinedOptionsName(option);
                return Math.Max(names.Length, longest);
            });

            foreach (Option option in options)
            {
                string names = BuildCombinedOptionsName(option);
                string description = option.Get<string>("Description") ?? "<No description provided>";
                WriteLine($"{Indent}{names.PadRight(longestLength)}   {description}");
            }
        }

        private string BuildCombinedOptionsName(Option option)
        {
            return option.AllNames
                .Select(name => name.Length > 1 ? $"--{name}" : $"-{name}")
                .Aggregate(new StringBuilder(), (sb, name) =>
                {
                    if (sb.Length > 0)
                        sb.Append(", ");
                    return sb.Append(name);
                })
                .ToString();
        }

        private string ResolveName(Arg arg)
        {
            string customName = arg.Get<string>("Name");
            return customName ?? arg.Name;
        }

        private const string Indent = "   ";
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
