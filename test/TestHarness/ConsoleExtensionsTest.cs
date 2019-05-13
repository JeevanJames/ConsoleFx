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

namespace TestHarness
{
    internal sealed class ConsoleExtensionsTest : TestBase
    {
        internal override void Run()
        {
            const string longStr = @"This is a very long string that I intend to use to test the ConsoleEx.PrintIndented method to prove that it can actually print a long string correctly by splitting them among at empty character occurrences. After attempting the first time, I realized that the initial string, while still long, was not long enough to properly test the method. So, I increased the length by a great deal to allow proper testing.";

            ConsoleEx.PrintIndented(8, longStr);
        }
    }
}
