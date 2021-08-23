// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace ConsoleFx.CmdLine.Parser.Runs
{
    [DebuggerDisplay("Argument: Assigned = {Assigned}")]
    internal sealed class ArgumentRun : ArgumentOrOptionRun<Argument>
    {
        internal ArgumentRun(Argument argument)
            : base(argument)
        {
        }

        internal Argument Argument => Arg;
    }
}
