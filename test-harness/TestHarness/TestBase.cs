// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace TestHarness
{
    internal abstract class TestBase
    {
        internal virtual Task RunAsync()
        {
            Run();
            return Task.CompletedTask;
        }

        internal virtual void Run()
        {
            throw new NotImplementedException();
        }
    }
}
