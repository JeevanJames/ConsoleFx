using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ConsoleFx.CmdLineParser;
using ConsoleFx.CmdLineParser.Validators;
using ConsoleFx.CmdLineParser.WindowsStyle;
using ConsoleFx.ConsoleExtensions;

using TestHarness.Commands;

namespace TestHarness
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var parser = new Parser(new WindowsParserStyle());
            parser.Commands.Add(new MultiRepoCommand("clone"));
            parser.Commands.Add(new MultiRepoCommand("pull"));
            parser.Commands.Add(new PushCommand());

            try
            {
                ParseResult result = parser.Parse("commit", "-help");
                Console.WriteLine(result.Command.Name);
                Console.WriteLine("Options");
                foreach (KeyValuePair<string, object> option in result.Options)
                {
                    if (option.Value is string str)
                        Console.WriteLine($"    {option.Key} = {str}");
                    if (option.Value is IList<string> list)
                        Console.WriteLine($"    {option.Key} = " + list.Aggregate(new StringBuilder(), (sb, s) =>
                        {
                            if (sb.Length > 0)
                                sb.Append(",");
                            sb.Append(s);
                            return sb;
                        }));
                }

                Console.WriteLine("Arguments");
                foreach (string argument in result.Arguments)
                    Console.WriteLine($"    {argument}");
            }
            catch (ParserException ex)
            {
                ConsoleEx.PrintLine(new ColorString().Red(ex.Message));
            }
            catch (Exception ex)
            {
                ConsoleEx.PrintLine(new ColorString().BgDkYellow(ex.Message));
            }

            Console.ReadLine();
        }
    }
}
