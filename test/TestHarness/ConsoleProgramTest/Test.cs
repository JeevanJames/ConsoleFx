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

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Program.ErrorHandlers;
using ConsoleFx.CmdLine.Program.HelpBuilders;
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
            int exitCode = program.Run("bleh", "--version");
            Console.WriteLine($"Exit code: {exitCode}");
        }
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

    [Command("clone")]
    [PushDirectory]
    [ErrorCode(-1, typeof(DateTime))]
    public sealed class CloneCommand : Command
    {
        [Help("REPO_URL", "URL of the repository that contains the manifest.")]
        public Uri RepoUrl { get; set; }

        [Help("MANIFEST_DIR", "Directory in the repository that contains the manifest file.")]
        public string ManifestDirectory { get; set; }

        [Option("branch")]
        [Help("The branch in the repository to use to get the manifest.")]
        public string Branch { get; set; }

        [Option("project-root-dir")]
        [Help("The root directory of the project.")]
        public DirectoryInfo ProjectRootDirectory { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Argument(nameof(RepoUrl))
                .ValidateAsUri(UriKind.Absolute)
                .TypedAs<Uri>();

            yield return new Argument(nameof(ManifestDirectory), isOptional: true);

            yield return new Option("branch", "b")
                .UsedAsSingleParameter();

            yield return new Option("project-root-dir", "r")
                .UsedAsSingleParameter()
                .ValidateAsDirectory()
                .TypedAs(value => new DirectoryInfo(value))
                .DefaultsTo(new DirectoryInfo("."));
        }

        protected override int HandleCommand()
        {
            return 0;
        }
    }
}
