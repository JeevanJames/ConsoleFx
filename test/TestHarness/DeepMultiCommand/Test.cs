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
using System.Collections.Generic;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Validators;

namespace TestHarness.DeepMultiCommand
{
    internal sealed class Test : TestBase
    {
        internal override void Run()
        {
            var program = new MyProgram();
            program.ScanEntryAssemblyForCommands(type => type.Namespace.Equals(typeof(Test).Namespace));
            program.Run("vcs", "git", "remote", "add", "https:/github.com/JeevanJames/Basics.git");
        }
    }

    [Program(nameof(MyProgram), Style = ArgStyle.Unix)]
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
            yield return new Argument(nameof(RemoteUrl))
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
