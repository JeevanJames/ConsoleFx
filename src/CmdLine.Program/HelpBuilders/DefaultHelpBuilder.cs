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
using System.Linq;
using System.Text;

using static System.Console;

namespace ConsoleFx.CmdLine.Program.HelpBuilders
{
    public class DefaultHelpBuilder : HelpBuilder
    {
        public override void DisplayHelp(Command command)
        {
            string usage = GetSummaryUsage(command);
            WriteLine($"Usage: {usage}");

            if (command.Arguments.Count > 0)
            {
                WriteLine();
                WriteLine("Arguments:");
                PrintArgs(command.Arguments, ArgumentDescriptionPlacement);
            }

            if (command.Options.Count > 0)
            {
                WriteLine();
                WriteLine("Options:");
                PrintOptions(command.Options, OptionDescriptionPlacement);
            }

            if (command.Commands.Count > 0)
            {
                WriteLine();
                WriteLine("Commands:");
                PrintArgs(command.Commands, CommandDescriptionPlacement);
            }
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

        private void PrintArgs<TArg>(Args<TArg> args, ArgDescriptionPlacement placement)
            where TArg : Arg
        {
            if (placement == ArgDescriptionPlacement.NextLine)
                PrintArgsOnNextLine(args);
            else
                PrintArgsOnSameLine(args);
        }

        private void PrintArgsOnSameLine<TArg>(Args<TArg> args)
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
                string description = arg["Description"] ?? "<No description provided>";
                WriteLine($"{Indent}{name.PadRight(longestLength)}   {description}");
            }
        }

        private void PrintArgsOnNextLine<TArg>(Args<TArg> args)
            where TArg : Arg
        {
            foreach (TArg arg in args)
            {
                WriteLine($"{Indent}{ResolveName(arg)}");
                string description = arg["Description"] ?? "<No description provided>";
                WriteLine($"{Indent}{Indent}{description}");
                WriteLine();
            }
        }

        private void PrintOptions(Options options, ArgDescriptionPlacement placement)
        {
            if (placement == ArgDescriptionPlacement.NextLine)
                PrintOptionsOnNextLine(options);
            else
                PrintOptionsOnSameLine(options);
        }

        private void PrintOptionsOnNextLine(Options options)
        {
            foreach (Option option in options)
            {
                string names = BuildCombinedOptionsName(option);
                WriteLine($"{Indent}{names}");

                string description = option["Description"] ?? "<No description provided>";
                WriteLine($"{Indent}{Indent}{description}");

                WriteLine();
            }
        }

        private void PrintOptionsOnSameLine(Options options)
        {
            int longestLength = options.Aggregate(0, (longest, option) =>
            {
                string names = BuildCombinedOptionsName(option);
                return Math.Max(names.Length, longest);
            });

            foreach (Option option in options)
            {
                string names = BuildCombinedOptionsName(option);
                string description = option["Description"] ?? "<No description provided>";
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
            string customName = arg["Name"];
            return customName ?? arg.Name;
        }

        private const string Indent = "    ";
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
