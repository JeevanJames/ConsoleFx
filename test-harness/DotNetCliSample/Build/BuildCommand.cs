// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.IO;

using ConsoleFx.CmdLine;

namespace ConsoleFx.TestHarness.DotNetCliSample.Build
{
    [Command("build", HelpText = "Build a .NET project.")]
    public sealed class BuildCommand : Command
    {
        [Option("output", "o",
            HelpText = "The output directory to place built artifacts in.BLAHBLAH")]
        public DirectoryInfo OutputDir { get; set; }

        [Option("framework", "f",
            HelpText = "The target framework to build for. The target framework must also be specified in the project file.")]
        public string Framework { get; set; }

        [Option("configuration", "c",
            HelpText = "The configuration to use for building the project. The default for most projects is 'Debug'.")]
        public string Configuration { get; set; } = "Debug";

        [Option("runtime", "r",
            HelpText = "The target runtime to build for.")]
        public string Runtime { get; set; }

        [Option("version-suffix",
            HelpText = "Set the value of the $(VersionSuffix) property to use when building the project.")]
        public string VersionSuffix { get; set; }

        [Flag("no-incremental",
            HelpText = "Do not use incremental building.")]
        public bool NoIncremental { get; set; }

        [Flag("no-dependencies",
            HelpText = "Do not build project-to-project references and only build the specified project.")]
        public bool NoDependencies { get; set; }

        [Flag("nologo",
            HelpText = "Do not display the startup banner or the copyright message.")]
        public bool NoLogo { get; set; }

        [Flag("no-restore",
            HelpText = "Do not restore the project before building.")]
        public bool NoRestore { get; set; }

        [Flag("interactive",
            HelpText = "Allows the command to stop and wait for user input or action (for example to complete authentication).")]
        public bool Interactive { get; set; }

        [Option("verbosity", "v",
            HelpText = "Set the MSBuild verbosity level. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].")]
        public Verbosity Verbosity { get; set; }

        [Flag("force",
            HelpText = "Force all dependencies to be resolved even if the last restore was successful. This is equivalent to deleting project.assets.json.")]
        public bool Force { get; set; }

        [Argument(HelpName = "<PROJECT | SOLUTION>",
            HelpText = "The project or solution file to operate on. If a file is not specified, the command will search the current directory for one.")]
        public string ProjectPath { get; set; }
    }
}
