// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ConsoleFx.CmdLine;
using ConsoleFx.CmdLine.Program;
using ConsoleFx.CmdLine.Validators;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness.Parser2
{
    internal sealed class Test : TestBase
    {
        internal override async Task RunAsync()
        {
            var program = new SyncDirProgram();
            await program.RunAsync(new[] { @"D:\Temp", @"D:\Steam", @"D:\Tools", "--exclude-dir", "Steam" });
        }
    }

    internal sealed class SyncDirProgram : ConsoleProgram
    {
        public SyncDirProgram()
            : base(nameof(SyncDirProgram), ArgStyle.Unix, ArgGrouping.DoesNotMatter)
        {
        }

        protected override int HandleCommand()
        {
            PrintLine($"Sync {Green}{SourceDir} {Reset}to:");
            foreach (DirectoryInfo directory in DestDirs)
                PrintLine($"    {Blue}{directory.FullName}");
            PrintLine($"Top Level Only: {Black.BgYellow}{TopLevelOnly}");
            if (ExcludeDirs.Count > 0)
            {
                PrintLine("Excluded directories:");
                foreach (var dir in ExcludeDirs)
                    PrintLine($"    {Black.BgYellow}{dir}");
            }

            return 0;
        }

        public DirectoryInfo SourceDir { get; set; }

        public List<DirectoryInfo> DestDirs { get; set; }

        [Option("exclude-dir")]
        public List<string> ExcludeDirs { get; set; } = new List<string>();

        [Option("top-level-only")]
        public bool TopLevelOnly { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Argument()
                .ValidateAsDirectory(true)
                .TypeAs<DirectoryInfo>();
            yield return new Argument(maxOccurences: byte.MaxValue)
                .ValidateAsDirectory(true)
                .TypeAs<DirectoryInfo>();

            yield return new Option("exclude-dir", "e")
                .UsedAsUnlimitedOccurrencesAndSingleParameter(optional: true);
            yield return new Option("top-level-only", "t")
                .UsedAsFlag(optional: true);
        }
    }
}
