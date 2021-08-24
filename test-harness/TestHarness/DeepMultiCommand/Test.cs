// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Validators;

namespace TestHarness.DeepMultiCommand
{
    internal sealed class Test : TestBase
    {
        internal override async Task RunAsync()
        {
            var program = new MyProgram();
            program.ScanEntryAssemblyForCommands(type => type.Namespace.Equals(typeof(Test).Namespace, StringComparison.Ordinal));
            await program.RunAsync("vcs", "git", "remote", "add", "https:/github.com/JeevanJames/Basics.git");
        }
    }

    [Program(Style = ArgStyle.Unix)]
    public sealed class MyProgram : ConsoleProgram
    {
    }

    [Command("vcs")]
    public sealed class VcsCommand : Command
    {
    }

    [Command("git", typeof(VcsCommand))]
    public sealed class GitCommand : Command
    {
    }

    [Command("remote", typeof(GitCommand))]
    public sealed class RemoteCommand : Command
    {
    }

    [Command("add", typeof(RemoteCommand))]
    public sealed class AddCommand : Command
    {
        public Uri RemoteUrl { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Argument()
                .ValidateAsUri()
                .TypeAs<Uri>();
        }

        protected override int HandleCommand()
        {
            Console.WriteLine($"Add remote to {RemoteUrl}");
            return 0;
        }
    }
}
