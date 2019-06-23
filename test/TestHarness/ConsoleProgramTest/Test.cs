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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Program.ErrorHandlers;
using ConsoleFx.CmdLine.Validators;

namespace TestHarness.ConsoleProgramTest
{
    internal sealed class Test : TestBase
    {
        internal override void Run()
        {
            var program = new MyProgram();
            program.ErrorHandler = new DefaultErrorHandler { ForeColor = ConsoleColor.Red };
            program.ScanEntryAssemblyForCommands(type => type.Namespace.Equals(typeof(Test).Namespace));
            int exitCode = program.Run("exec", "--", "one", "two", "three");
            Console.WriteLine($"Exit code: {exitCode}");
        }
    }

    public abstract class ManifestCommand : Command
    {
    }

    public abstract class RepoCommand : ManifestCommand
    {
        [Option("tags")]
        [Help("Operate on only the repositories with these tags.")]
        public IList<string> Tags { get; set; }

        [Option("exclude-tags")]
        [Help("Operate on only repositories without these tags.")]
        public IList<string> ExcludedTags { get; set; }

        [Option("only-me")]
        [Help("Only operate on the repository of the current directory.")]
        public bool OnlyMe { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs().Concat(GetMyArgs());

            IEnumerable<Arg> GetMyArgs()
            {
                yield return new Option("tags")
                    .UsedAsUnlimitedOccurrencesAndParameters(optional: true)
                    .ValidateWithRegex(TagPattern);

                yield return new Option("exclude-tags")
                    .UsedAsUnlimitedOccurrencesAndParameters(optional: true)
                    .ValidateWithRegex(TagPattern);

                yield return new Option("only-me")
                    .UsedAsFlag(optional: true);
            }
        }

        private static readonly Regex TagPattern = new Regex(@"^(\w[\w_-]*)$");
    }

    [Program("ConsoleProgramTest", Style = ArgStyle.Unix)]
    public sealed class MyProgram : ConsoleProgram
    {
        [Option("version")]
        [Help("Displays the version of the application.")]
        public bool ShowVersion { get; set; }

        [Argument("argument")]
        [Help("Sample argument")]
        public string Argument { get; set; }

        protected override int HandleCommand()
        {
            if (ShowVersion)
                Console.WriteLine("Version: 1.0.0.1");
            else
                Console.WriteLine($"Default behavior - {Argument ?? "Default value"}");
            return 0;
        }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Option("version")
                .UsedAsFlag(optional: true);

            yield return new Argument("argument")
                .UnderGroups(1);
        }
    }
    [Command("tags")]
    public sealed class TagsCommand : AbstractCommand
    {
    }

    [Command("list", typeof(TagsCommand))]
    public sealed class TagsListCommand : RepoCommand
    {
        [Option("pretty")]
        [Help("Displays the tags in a table")]
        public bool PrettifyOutput { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs().Concat(GetMyArgs());

            IEnumerable<Arg> GetMyArgs()
            {
                yield return new Option("pretty", "p")
                    .UsedAsFlag(optional: true);
            }
        }

        protected override int HandleCommand()
        {
            Console.WriteLine($"Prettify: {PrettifyOutput}");
            return 0;
        }
    }

    [Command("exec")]
    public sealed class ExecCommand : RepoCommand
    {
        public IList<string> GitArgs { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            return base.GetArgs().Concat(GetMyArgs());

            IEnumerable<Arg> GetMyArgs()
            {
                yield return new Argument(nameof(GitArgs), maxOccurences: byte.MaxValue);
            }
        }

        protected override int HandleCommand()
        {
            foreach (string arg in GitArgs)
            {
                Console.WriteLine(arg);
            }
            return 0;
        }
    }
}
