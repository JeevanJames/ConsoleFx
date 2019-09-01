﻿#region --- License & Copyright Notice ---
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
using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Program.ErrorHandlers;

namespace TestHarness.ConsoleProgramTest
{
    internal sealed class Test : TestBase
    {
        internal override void Run()
        {
            var program = new MyProgram();
            program.ErrorHandler = new DefaultErrorHandler { ForeColor = ConsoleColor.Red };
            program.ScanEntryAssemblyForCommands(type => type.Namespace.Equals(typeof(Test).Namespace));
            int exitCode = program.Run("Jeevan", "--last-name", "James", "-v");
            Console.WriteLine($"Exit code: {exitCode}");
        }
    }

    [Program("my-program")]
    public sealed class MyProgram : ConsoleProgram
    {
        [Argument("name")]
        public string FirstName { get; set; }

        [Option("last-name"), SingleParameter(false)]
        public string LastName { get; set; }

        [Option("cool"), Flag]
        public bool IsCool { get; set; }

        public bool Verbose { get; set; }

        protected override int HandleCommand()
        {
            Console.WriteLine($"Name: {FirstName} {LastName}");
            Console.WriteLine($"Is cool: {IsCool}");
            Console.WriteLine($"Verbose: {Verbose}");
            return 0;
        }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Option("verbose", "v")
                .UsedAsFlag()
                .AssignTo(nameof(Verbose));
        }
    }
}
