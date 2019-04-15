using System;
using System.Collections.Generic;

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
                ParseResult result = parser.Parse("push", "-include:repo1", "-include:repo2,repo3", "-message:blah");
                Console.WriteLine(result.Command.Name);
                Console.WriteLine("Options");
                foreach (KeyValuePair<string, object> option in result.Options)
                    Console.WriteLine($"    {option.Key} = {option.Value}");
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
