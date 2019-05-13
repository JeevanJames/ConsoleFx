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
using System.Collections.Generic;

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineArgs.Validators;
using ConsoleFx.CmdLineParser;
using ConsoleFx.CmdLineParser.Style;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness
{
    internal sealed class ParserTest : TestBase
    {
        internal override void Run()
        {
            var command = new Command();
            command.AddArgument("source")
                .FormatAs(s => s.ToUpperInvariant());
            command.AddArgument("destination")
                .ValidateAsString(5);
            command.AddArgument("count", isOptional: true)
                .ValidateAsInteger(0, 100)
                .TypedAs<int>()
                .DefaultsTo(8);
            command.AddOption("v")
                .UsedAsFlag();
            command.AddOption("y")
                .UsedAsFlag();
            command.AddOption("trace", "t")
                .UsedAsSingleParameter()
                .ValidateAsEnum<TraceLevel>().TypedAs<string>();
            command.AddOption("repeat", "r")
                .UsedAsSingleParameter()
                .ValidateAsInteger(0, 3)
                .TypedAs<int>();
            command.AddOption("log", "l")
                .UsedAsSingleParameter()
                .FormatAs("Log{0}");
            command.AddOption("url", "u", "web")
                .UsedAsSingleParameter()
                .ValidateAsUri();
            command.AddOption("id")
                .UsedAsSingleParameter()
                .ValidateAsGuid();

            var parser = new Parser(command, ArgStyle.Windows);
            try
            {
                ParseResult result = parser.Parse("sourceFile", "destfile",
                    "/v",
                    "/r:2",
                    "-log:blah",
                    "/web:https://example.com",
                    "-id:{DD45218B-CE76-4714-A3B3-7F77F4A287F1}");

                foreach (object arg in result.Arguments)
                    Console.WriteLine(arg?.ToString() ?? "<value not specified>");
                foreach (KeyValuePair<string, object> kvp in result.Options)
                    Console.WriteLine($"{kvp.Key} = {kvp.Value}");
            }
            catch (ValidationException ex)
            {
                PrintLine($"{Yellow}{ex.ValidatorType.Name} - {Red}{ex.Message}");
            }
            catch (ParserException ex)
            {
                PrintLine($"{Red}{ex.Message}");
            }
        }
    }

    public enum TraceLevel
    {
        Debug,
        Info,
        Warning,
        Error,
    }
}
