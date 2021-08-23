// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

using ConsoleFx.ConsoleExtensions;

namespace TestHarness.ConsoleCaptureTest
{
    internal sealed class Test : TestBase
    {
        internal override void Run()
        {
            ConsoleCapture cc = new ConsoleCapture("dotnet.exe", "build")
                .OnOutput(line => Console.WriteLine($"> {line}"))
                .OnError(line =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"> {line}");
                    Console.ResetColor();
                });

            int exitCode = cc.Start();

            Console.WriteLine();
            Console.WriteLine($"Exit code: {exitCode}");
            ConsoleEx.WaitForAnyKey();
        }
    }
}
