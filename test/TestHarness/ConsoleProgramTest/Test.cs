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
using System.Linq;
using System.Text.RegularExpressions;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Validators;

namespace TestHarness.ConsoleProgramTest
{
    internal sealed class Test : TestBase
    {
        internal override void Run()
        {
            var program = new MyProgram();
            program.ScanEntryAssemblyForCommands(type => type.Namespace.Equals(typeof(Test).Namespace));
            program.Run("install2");
            program.Run("install2", "repo2");
        }
    }

    [Program(Style = ArgStyle.Unix)]
    public sealed class MyProgram : ConsoleProgram
    {
        [Option("tags")]
        public IList<string> Tags { get; set; }

        [Option("exclude-tags")]
        public IList<string> ExcludedTags { get; set; }

        [Option("repos")]
        public IList<string> Repositories { get; set; }

        [Option("exclude-repos")]
        public IList<string> ExcludedRepositories { get; set; }

        [Option("only-me")]
        public bool OnlyMe { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Option("tags")
                .UsedAsUnlimitedOccurrencesAndParameters(optional: true)
                .ValidateWithRegex(TagPattern);

            yield return new Option("exclude-tags")
                .UsedAsUnlimitedOccurrencesAndParameters(optional: true)
                .ValidateWithRegex(TagPattern);

            yield return new Option("only-me")
                .UsedAsFlag(optional: true);

            yield return new Option("repos")
                .UsedAsUnlimitedOccurrencesAndParameters(optional: true);

            yield return new Option("exclude-repos")
                .UsedAsUnlimitedOccurrencesAndParameters(optional: true);
        }

        private static readonly Regex TagPattern = new Regex(@"^(\w[\w_-]*)$");

        protected override int HandleCommand()
        {
            Tags.ToList().ForEach(Console.WriteLine);
            ExcludedTags.ToList().ForEach(Console.WriteLine);

            return 0;
        }
    }

    [Command("install2")]
    public class Install2Command : Command
    {
        protected override int HandleCommand()
        {
            Console.WriteLine("AddCommand");
            return 0;
        }
    }

    [Command("repo2", typeof(Install2Command))]
    public sealed class Repo2Command : Command
    {
        protected override int HandleCommand()
        {
            Console.WriteLine("RepoCommand");
            return 0;
        }
    }
}
