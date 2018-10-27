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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using ConsoleFx.CmdLineParser;
using ConsoleFx.CmdLineParser.Programs;
using ConsoleFx.CmdLineParser.UnixStyle;
using ConsoleFx.ConsoleExtensions;
using ConsoleFx.Prompter;

using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness
{
    public enum BackupType
    {
        Full,
        Incremental
    }

    public static class Program
    {
        private static int Main(string[] args)
        {
            int choice = PromptList(new [] {
                "Jeevan James", "Merina Mathew", "Ryan James", "Emma James"
            }, new PromptListSettings
            {
                SelectedForegroundColor = CColor.Yellow,
                SelectedBackgroundColor = CColor.BgDkGreen,
                SelectedPrefix = "> "
            });
            Console.WriteLine(choice);
            //TestPrompter();

            //TestColorOutput();

            //TestPrompts();

            //TestParser();

            if (Debugger.IsAttached)
            {
                PrintBlank(3);
                WaitForAnyKey();
            }

            return 0;
        }

        private static void TestParser()
        {
            int exitCode = new NpmProgram(new UnixParserStyle()).Run();
        }

        private static void TestPrompts()
        {
            string name = Prompt("[blue.bgwhite]Enter name starting with J: ", s => s.StartsWith("J"));
            string password = ReadSecret("Enter password: ", hideCursor: false, hideMask: false, needValue: true);
            PrintLine($"{name} : {password}");
        }

        private static void TestColorOutput()
        {
            PrintLine("[BgRed]Node Package Manager [BgGray.Black](npm) [BgYellow]by Node.js");
            PrintLine(new ColorString()
                .BgRed("Node Package Manager ")
                .BgGray().Black("(npm) ")
                .BgYellow("by Node.js")
            );
        }

        private static void TestPrompter()
        {
            dynamic answers = Prompter.Ask(
                StaticText.Text((ColorString) "Please answer the following questions [red]truthfully"),
                Question.Input("Name", "What's your name?"),
                Question.Input("Age", "What's your age?")
                    .AsInteger()
                    .Validate(value => value > 0 ? true : (ValidationResult) "Enter positive age"),
                Question.Password("Password", "Enter your password:"),
                StaticText.BlankLine(),
                StaticText.Separator(),
                StaticText.Text((ColorString) "This question is optional and reflects your opinion."),
                StaticText.Text((ColorString) "We will keep the info private."),
                Question.Confirm("SeniorCitizen", "Do you consider yourself a senior citizen?")
            );
            Console.WriteLine($"Hi {answers.Name}, who is {answers.Age} years old ({answers.SeniorCitizen})");
        }
    }

    public sealed class NpmProgram : ConsoleProgram
    {
        public NpmProgram(ParserStyle parserStyle) : base(parserStyle)
        {
        }

        protected override int Handle(ParseResult result)
        {
            Command command = result.Command;
            if (command == null)
                PrintLine(@"Root Command");
            else
                PrintLine($"Command: {command.Name}");
            foreach (KeyValuePair<string, object> kvp in result.Options)
            {
                Print($"Option {kvp.Key}: ");
                var list = kvp.Value as IList;
                if (list != null)
                {
                    foreach (object item in list)
                        Print($"{item?.ToString() ?? "(null)"}, ");
                    PrintBlank();
                } else
                    PrintLine(kvp.Value.ToString());
            }
            return 0;
        }

        protected override IEnumerable<Option> GetOptions()
        {
            yield return new Option("verbose", "v");
        }

        protected override IEnumerable<Command> GetCommands()
        {
            yield return new InstallCommand();
            yield return new UpdateCommand();
        }
    }

    public sealed class InstallCommand : CommandBuilder
    {
        protected override string Name => "install";

        protected override IEnumerable<Option> Options
        {
            get
            {
                yield return new Option("force", "f");
            }
        }

        protected override IEnumerable<Argument> Arguments
        {
            get
            {
                yield return new Argument();
            }
        }
    }

    public sealed class UpdateCommand : CommandBuilder
    {
        protected override string Name => "update";

        protected override IEnumerable<Option> Options
        {
            get
            {
                yield return new Option("version", "ver")
                    .ParamsOfType<int>()
                    .UsedAs(usage => {
                        usage.ParameterRequirement = OptionParameterRequirement.Required;
                    });
                yield return new Option("force", "f");
                yield return new Option("format")
                    .FormatParamsAs(v => v.ToUpperInvariant())
                    .UsedAs(usage => {
                        usage.Requirement = OptionRequirement.OptionalUnlimited;
                        usage.ParameterRequirement = OptionParameterRequirement.Required;
                    });
            }
        }

        protected override IEnumerable<Argument> Arguments
        {
            get
            {
                yield return new Argument();
            }
        }
    }
}
