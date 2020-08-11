#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

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
