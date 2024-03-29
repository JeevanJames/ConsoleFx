﻿// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.ErrorHandling;

namespace TestHarness.ConsoleProgramTest
{
    internal sealed class Test : TestBase
    {
        internal override async Task RunAsync()
        {
            ConsoleProgram program = new MyProgram()
                .HandleErrorsWith(() => new DefaultErrorHandler { ForeColor = ConsoleColor.Red });
            program.ScanEntryAssemblyForCommands(type => type.Namespace.Equals(typeof(Test).Namespace, StringComparison.Ordinal));
            int exitCode = await program.RunAsync("Jeevan", "-v", "--trait", "Handsome", "--trait", "Awesome");
            Console.WriteLine($"Exit code: {exitCode}");
        }
    }

    public sealed class MyProgram : ConsoleProgram
    {
        [Argument]
        public string FirstName { get; set; }

        [Option("cool")]
        public bool IsCool { get; set; }

        public bool Verbose { get; set; }

        [Option("trait", MultipleOccurrences = true)]
        public IList<string> Traits { get; }

        protected override int HandleCommand()
        {
            Console.WriteLine($"Name: {FirstName}");
            Console.WriteLine($"Is cool: {IsCool}");
            Console.WriteLine($"Verbose: {Verbose}");
            Console.WriteLine("Traits:");
            foreach (string trait in Traits)
            {
                Console.WriteLine($"    {trait}");
            }

            return 0;
        }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Option("verbose", "v")
                .UsedAsFlag()
                .AssignTo(nameof(Verbose));
        }

        protected override void SetupOptions(Options options)
        {
            options["trait"].AddName("t");
        }
    }
}
