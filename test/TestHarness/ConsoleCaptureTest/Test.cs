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

namespace TestHarness.ConsoleCaptureTest
{
    internal sealed class Test : TestBase
    {
        internal override void Run()
        {
            var cc = new ConsoleCapture("dotnet.exe", "build2")
                .OnOutput(line => Console.WriteLine($"==> {line}"))
                .OnError(line =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"==> {line}");
                    Console.ResetColor();
                });
            int exitCode = cc.Start();
            Console.WriteLine(exitCode);
            ConsoleEx.WaitForAnyKey();
        }
    }
}
