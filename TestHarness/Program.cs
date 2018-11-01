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
            //var pb = new ProgressBar();
            //WaitForKeysLoop(new[] 
            //{
            //    ConsoleKey.LeftArrow.HandledBy(k => pb.Value -= 1),
            //    ConsoleKey.RightArrow.HandledBy(k => pb.Value += 1)
            //},
            //    escapeKeys: new []{ConsoleKey.Escape, ConsoleKey.Enter});

            //TestSelects();

            TestPrompter();

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

        private static void TestSelects()
        {
            PrintLine($"{Console.ForegroundColor}, {Console.BackgroundColor}");

            var choice = SelectSingle(new[]
            {
                "Jeevan James", "Merina Mathew", "Ryan James", "Emma James"
            });
            var choices = SelectMultiple(new[]
            {
                "Jeevan James", "Merina Mathew", "Ryan James", "Emma James"
            }, checkedIndices: new[] {0, 2});
            Console.WriteLine(string.Join(", ", choices));
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
            string[] travelTimes =
            {
                "Doesn't Matter", "Morning", "Noon", "Afternoon", "Evening", "Night"
            };
            dynamic answers = Prompter.Ask(
                StaticText.Text("Please answer the following questions [red]truthfully"),
                Question.Input("Name", "What's your name? ")
                    .Required()
                    .Validate(str => char.IsUpper(str[0])),
                Question.Password("Password", "Enter your password: ")
                    .HideCursor().Required(),
                Question.Confirm("SavePwd", "Remember password", false),
                StaticText.Separator(),
                Question.Input("Age", "Enter your age: ")
                    .Required()
                    .Validate(str => int.TryParse(str, out _))
                    .Convert(str => int.Parse(str)),
                Question.List("TravelTime", "When do you want to travel?", travelTimes),
                Question.Checkbox("Showtimes", "Which show times do you wish to attend", travelTimes)
            );
            

            PrintLine($"Hi {answers.Name} with password {answers.Password} (Remember: {answers.SavePwd})");
            PrintLine($"Your travel time: {travelTimes[answers.TravelTime]}");
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
