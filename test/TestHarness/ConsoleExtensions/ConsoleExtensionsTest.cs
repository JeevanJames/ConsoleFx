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

using ConsoleFx.ConsoleExtensions;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness.ConsoleExtensions
{
    internal sealed class Test : TestBase
    {
        internal override void Run()
        {
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
