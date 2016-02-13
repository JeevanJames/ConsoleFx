using System;
using System.Linq;
using System.Text;

using ConsoleFx.Parser;

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
            int maxOptionNameLength = parser.Options.Max(opt => opt.Name.Length);
            foreach (Option option in parser.Options)
            {
                string shortName = option.ShortName != null ? $"(short: {option.ShortName}) " : string.Empty;
                string description = option.Metadata["Description"] ?? string.Empty;
                Console.WriteLine($"  - {option.Name.PadRight(maxOptionNameLength)}  {shortName}{description}");
            }
            Console.WriteLine();
            for (int i = 0; i < parser.Arguments.Count; i++)
            {
                Argument argument = parser.Arguments[i];
                string name = argument.Metadata["Name"] ?? $"Arg{i + 1}";
                string description = argument.Metadata["Description"] ?? string.Empty;
                Console.WriteLine($"  <{name}>  {description}");
            }
        }
    }
}