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
            var helpBuilder = new DefaultHelpBuilder
            {
                ArgumentDescriptionPlacement = ArgDescriptionPlacement.SameLine,
                OptionDescriptionPlacement = ArgDescriptionPlacement.SameLine,
            };
            helpBuilder.DisplayHelp(new CloneCommand());
            
            var program = new MyProgram();
            program.ErrorHandler = new DefaultErrorHandler { ForeColor = ConsoleColor.Red };
            program.ScanEntryAssemblyForCommands(type => type.Namespace.Equals(typeof(Test).Namespace));
            int exitCode = program.Run("clone", "https://github.com/JeevanJames/_project", "--project-root-dir", "D:\\Temp\\MyProjects");
            Console.WriteLine($"Exit code: {exitCode}");
        }
    }

    [Program("ConsoleProgramTest", Style = ArgStyle.Unix)]
    public sealed class MyProgram : ConsoleProgram
    {
    }

    [Command("clone")]
    [PushDirectory]
    [ErrorCode(-1, typeof(DateTime))]
    public sealed class CloneCommand : Command
    {
        public Uri RepoUrl { get; set; }

        public string ManifestDirectory { get; set; }

        [Option("branch")]
        public string Branch { get; set; }

        [Option("project-root-dir")]
        public DirectoryInfo ProjectRootDirectory { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Argument(nameof(RepoUrl))
                .ValidateAsUri(UriKind.Absolute)
                .TypedAs<Uri>()
                .Description("URL of the repository that contains the manifest.", "REPO_URL");

            yield return new Argument(nameof(ManifestDirectory), true)
                .Description("Directory in the repository that contains the manifest file.", "MANIFEST_DIR");

            yield return new Option("branch", "b")
                .UsedAsSingleParameter()
                .Description("The branch in the repository to use to get the manifest.");

            yield return new Option("project-root-dir", "r")
                .UsedAsSingleParameter()
                .ValidateAsDirectory()
                .TypedAs(value => new DirectoryInfo(value))
                .DefaultsTo(new DirectoryInfo("."))
                .Description("The root directory of the project.");
        }

        protected override int HandleCommand()
        {
            Directory.SetCurrentDirectory(Path.GetTempPath());
            if (DateTime.Now.Ticks > 100L)
                throw new InvalidOperationException("Invalid operation");
            return 0;
        }
    }
}
