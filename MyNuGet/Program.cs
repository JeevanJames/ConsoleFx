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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ConsoleFx.CmdLineParser.WindowsStyle;
using ConsoleFx.CmdLineParser;
using ConsoleFx.CmdLineParser.Validators;
using ConsoleFx.CmdLineParser.Programs;

using MyNuGet.Install;
using MyNuGet.Update;

using static System.Console;

using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace MyNuGet
{
    public sealed class Program : ConsoleProgram
    {
        private static int Main()
        {
            WriteLine($"{ForegroundColor} - {BackgroundColor}");
            WriteLineColor("[gray]MyNuGet NuGet Simulator");
            WriteLineColor("[DkGreen.BgYellow]The best damn nuget out there");
            WriteLineColor("[White.BgMagenta]MyNuGet NuGet Simulator");
            WriteLineColor("[Blue.BgYellow]Written by Jeevan James");
            int exitCode = new Program(new WindowsParserStyle()).Run();
            if (Debugger.IsAttached)
                ReadLine();
            return exitCode;
        }

        public Program(ParserStyle parserStyle) : base(parserStyle)
        {
        }

        protected override int Handle(ParseResult result)
        {
            WriteLine($"Command: {result.Command?.Name ?? "[Root]"}");
            foreach (KeyValuePair<string, object> kvp in result.Options)
            {
                Write($"Option {kvp.Key}: ");
                var list = kvp.Value as IList;
                if (list != null)
                {
                    foreach (object item in list)
                        Write($"{item?.ToString() ?? "(null)"}, ");
                    WriteLine();
                }
                else
                    WriteLine(kvp.Value);
            }
            foreach (string argument in result.Arguments)
            {
                WriteLine($"Argument {argument}");
            }
            return 0;
        }

        protected override IEnumerable<Command> GetCommands()
        {
            yield return new InstallCommand();
            yield return new UpdateCommand();
        }

        protected override IEnumerable<Option> GetOptions()
        {
            yield return new Option("ConfigFile")
                .UsedAsSingleParameter()
                .ValidateWith(new FileValidator { ShouldExist = true })
                .ParamsOfType(@default: new FileInfo(@"%AppData%\NuGet\NuGet.config"))
                .Description(Root.ConfigFile);
        }
    }
}
