#region --- License & Copyright Notice ---
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

using System;
using System.Linq;
using System.Text;

using static System.Console;

namespace ConsoleFx.CmdLineParser.Programs.UsageBuilders
{
    public sealed class MetadataUsageBuilder : UsageBuilder
    {
        public override void Display(Parser parser)
        {
            WriteLine(AssemblyProduct);
            WriteLine(AssemblyDescription);
            WriteLine(AssemblyCopyright);

            WriteLine();

            WriteLine(@"Usage:");
            //TODO: ConsoleEx.WriteIndented(GetUsage(parser.Options, parser.Arguments), 4, true);
            WriteLine(GetUsage(parser.Options, parser.Arguments));
            WriteLine();

            int maxOptionNameLength = parser.Options.Max(opt => opt.Name.Length);
            foreach (Option option in parser.Options)
            {
                string fullNameSection = $"  -{option.Name.PadRight(maxOptionNameLength)}  ";
                string shortName = option.ShortName != null ? $"(short: {option.ShortName}) " : string.Empty;
                string description = option["Description"] ?? string.Empty;
                Write(fullNameSection);
                //TODO: ConsoleEx.WriteIndented($"{shortName}{description}", fullNameSection.Length);
                WriteLine($"{shortName}{description}");
            }

            WriteLine();

            int maxArgumentNameLength = parser.Arguments.Max(a => (a["Name"] ?? string.Empty).Length);
            maxArgumentNameLength = Math.Max(maxArgumentNameLength, "Arg".Length + parser.Arguments.Count);
            for (var i = 0; i < parser.Arguments.Count; i++)
            {
                Argument argument = parser.Arguments[i];
                string name = argument["Name"] ?? $"Arg{i + 1}";
                string description = argument["Description"] ?? string.Empty;
                WriteLine($"  <{name}>  {description}");
            }
        }

        private string GetUsage(Options options, Arguments arguments)
        {
            var usage = new StringBuilder();
            foreach (Option option in options)
            {
                if (usage.Length > 0)
                    usage.Append(" ");
                if (option.Usage.MinOccurences == 0)
                    usage.Append("[");
                usage.Append($"-{option.Name}");
                if (option.Usage.MaxParameters > 0)
                    usage.Append(":(params)");
                if (option.Usage.MinOccurences == 0)
                    usage.Append("]");
            }
            foreach (Argument argument in arguments)
            {
                if (usage.Length > 0)
                    usage.Append(" ");
                if (argument.IsOptional)
                    usage.Append("[");
                usage.Append($"<{argument["Name"] ?? "Arg"}>");
                if (argument.IsOptional)
                    usage.Append("]");
            }
            return usage.ToString();
        }
    }
}