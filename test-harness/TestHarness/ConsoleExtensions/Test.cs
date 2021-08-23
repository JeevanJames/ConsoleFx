// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

using ConsoleFx.ConsoleExtensions;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness.ConsoleExtensions
{
    internal sealed class Test : TestBase
    {
        internal override void Run()
        {
            var table = new Table()
                .AddColumn("Name")
                .AddColumn("Age")
                .AddRow("Hi, my name is Jeevan James, staying at Purva Riviera apartment in Marathahalli Bangalore", 42)
                .AddRow("Ryan", 10);
            table.Print();

            string longStr = $@"This is a very long string that I intend to use to test the {Cyan}ConsoleEx.PrintIndented{Reset} method to prove that it can actually print a long string correctly {Black.BgYellow}by splitting them among at empty character occurrences{Reset.BgReset}. After attempting the first time, I realized that the initial string, while still long, was not long enough to properly test the method. So, I increased the length by a great deal to allow proper testing.";

            PrintIndented(longStr, 8, true);
            PrintBlank();
            PrintIndented(longStr, 16, false);
            PrintBlank();

            var cstr = new ColorString().Text("Jeevan [Yellow.BgBlack]James", CColor.Magenta, CColor.DkYellow);
            Console.WriteLine(cstr);
            PrintLine(cstr.ToString());
        }
    }
}
