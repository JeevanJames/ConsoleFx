// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

using ConsoleFx.CmdLine.Program;

namespace ConsoleFx.TestHarness.SimpleProgram
{
    [Program(Name = "simple-program")]
    public sealed class MyProgram : ConsoleProgram
    {
        public static async Task<int> Main()
        {
            var program = new MyProgram();
#if DEBUG
            return await program.RunDebugAsync().ConfigureAwait(false);
#else
            return await program.RunWithCommandLineArgsAsync().ConfigureAwait(false);
#endif
        }

        public MyProgram()
            : base("simple-program", ArgStyle.Unix)
        {
        }

        protected override int HandleCommand()
        {
            Console.WriteLine("In program");
            return base.HandleCommand();
        }
    }
}
