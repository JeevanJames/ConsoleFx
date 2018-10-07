using System;
using System.Linq;
using System.Text;

using ConsoleFx.Parser;
using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Programs.UsageBuilders
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
            ConsoleEx.WriteIndented(GetUsage(parser.Options, parser.Arguments), 4, true);
            Console.WriteLine();

            int maxOptionNameLength = parser.Options.Max(opt => opt.Name.Length);
            foreach (Option option in parser.Options)
            {
                string fullNameSection = $"  -{option.Name.PadRight(maxOptionNameLength)}  ";
                string shortName = option.ShortName != null ? $"(short: {option.ShortName}) " : string.Empty;
                string description = option["Description"] ?? string.Empty;
                Console.Write(fullNameSection);
                ConsoleEx.WriteIndented($"{shortName}{description}", fullNameSection.Length);
            }

            Console.WriteLine();

            int maxArgumentNameLength = parser.Arguments.Max(a => (a["Name"] ?? string.Empty).Length);
            maxArgumentNameLength = Math.Max(maxArgumentNameLength, "Arg".Length + parser.Arguments.Count);
            for (var i = 0; i < parser.Arguments.Count; i++)
            {
                Argument argument = parser.Arguments[i];
                string name = argument["Name"] ?? $"Arg{i + 1}";
                string description = argument["Description"] ?? string.Empty;
                Console.WriteLine($"  <{name}>  {description}");
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