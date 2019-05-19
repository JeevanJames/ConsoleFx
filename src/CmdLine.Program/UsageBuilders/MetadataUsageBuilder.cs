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

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Parser;

namespace ConsoleFx.CmdLine.Program.UsageBuilders
{
    public sealed class MetadataUsageBuilder : UsageBuilder
    {
        public override void Display(Parser.Parser parser)
        {
            Console.WriteLine(AssemblyProduct);
            Console.WriteLine(AssemblyDescription);
            Console.WriteLine(AssemblyCopyright);

            Console.WriteLine();

            Console.WriteLine(@"Usage:");

            //TODO: ConsoleEx.WriteIndented(GetUsage(parser.Options, parser.Arguments), 4, true);
            Console.WriteLine(GetUsage(parser.Command.Options, parser.Command.Arguments));
            Console.WriteLine();

            int maxOptionNameLength = parser.Command.Options.Max(opt => opt.Name.Length);
            foreach (Option option in parser.Command.Options)
            {
                string fullNameSection = $"  -{option.Name.PadRight(maxOptionNameLength)}  ";
                string description = option["Description"] ?? string.Empty;
                Console.Write(fullNameSection);

                //TODO: ConsoleEx.WriteIndented($"{shortName}{description}", fullNameSection.Length);
                Console.WriteLine(description);
            }

            Console.WriteLine();

            int maxArgumentNameLength = parser.Command.Arguments.Max(a => (a["Name"] ?? string.Empty).Length);
            maxArgumentNameLength = Math.Max(maxArgumentNameLength, "Arg".Length + parser.Command.Arguments.Count);
            for (var i = 0; i < parser.Command.Arguments.Count; i++)
            {
                Argument argument = parser.Command.Arguments[i];
                string name = argument["Name"] ?? $"Arg{i + 1}";
                string description = argument["Description"] ?? string.Empty;
                Console.WriteLine($"  <{name.PadRight(maxArgumentNameLength)}>  {description}");
            }
        }

        private static string GetUsage(Options options, Arguments arguments)
        {
            var usage = new StringBuilder();
            foreach (Option option in options)
            {
                if (usage.Length > 0)
                    usage.Append(" ");
                if (option.Usage.MinOccurrences == 0)
                    usage.Append("[");
                usage.Append($"-{option.Name}");
                if (option.Usage.MaxParameters > 0)
                    usage.Append(":(params)");
                if (option.Usage.MinOccurrences == 0)
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
