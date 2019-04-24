using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ConsoleFx.CmdLineParser;
using ConsoleFx.CmdLineParser.Validators;
using ConsoleFx.CmdLineParser.WindowsStyle;
using ConsoleFx.ConsoleExtensions;
using ConsoleFx.Prompter;

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

            var prompter = new Prompter();
            prompter.List("ArgsSource", "What should be the source of the CLI args?",
                new[] { "Main method args parameter", "Args in code" });
            dynamic answers = prompter.Ask();

            try
            {
                ParseResult result = parser.Parse(answers.ArgsSource == 0 ? args : new [] {"commit", "/help"});
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
