// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Parser;
using ConsoleFx.CmdLine.Parser.Style;
using ConsoleFx.CmdLine.Validators;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness.Parser
{
    internal sealed class Test : TestBase
    {
        internal override void Run()
        {
            var command = new Command();
            command.AddArgument()
                .FormatAs(s => s.ToUpperInvariant());
            command.AddArgument()
                .ValidateAsString(5);
            command.AddArgument()
                .ValidateAsInteger(0, 100)
                .TypeAs<int>()
                .DefaultsTo(8);
            command.AddArgument(maxOccurences: byte.MaxValue);
            command.AddOption("v")
                .UsedAsFlag();
            command.AddOption("y")
                .UsedAsFlag();
            command.AddOption("trace", "t")
                .UsedAsSingleParameter()
                .ValidateAsEnum<TraceLevel>().TypeAs<string>();
            command.AddOption("repeat", "r")
                .UsedAsSingleParameter()
                .ValidateAsInteger(0, 3)
                .TypeAs<int>()
                .DefaultsTo(0);
            command.AddOption("log", "l")
                .UsedAsSingleParameter()
                .FormatAs("Log{0}");
            command.AddOption("url", "u", "web")
                .UsedAsSingleParameter()
                .ValidateAsUri();
            command.AddOption("id")
                .UsedAsSingleParameter()
                .ValidateAsGuid();
            command.AddOption("multiple", "m")
                .UsedAsUnlimitedOccurrencesAndParameters(true);
            command.AddOption("kvp")
                .UsedAsSingleParameter(true)
                .ValidateAsKeyValue();

            var parser = new ConsoleFx.CmdLine.Parser.Parser(command, ArgStyle.Unix);
            try
            {
                IParseResult result = parser.Parse("sourceFile", "destfile", "7", "8",
                    "-vy",
                    "--log", "blah-bleh",
                    "--web", "https://example.com",
                    "--id={DD45218B-CE76-4714-A3B3-7F77F4A287F1}",
                    "-m=abc",
                    "-m=def",
                    "--kvp", "key=value",
                    "--",
                    "node", "--version");

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
