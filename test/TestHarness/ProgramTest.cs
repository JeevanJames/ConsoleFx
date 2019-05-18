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

using System.Collections.Generic;
using System.IO;

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineArgs.Validators;
using ConsoleFx.Program;

using static ConsoleFx.ConsoleExtensions.Clr;
using static ConsoleFx.ConsoleExtensions.ConsoleEx;

namespace TestHarness
{
    internal sealed class ProgramTest : TestBase
    {
        internal override void Run()
        {
            var program = new MyProgram();
            program.Run();
        }
    }

    public sealed class MyProgram : ConsoleProgram
    {
        public MyProgram() : base(ArgStyle.Windows, ArgGrouping.DoesNotMatter)
        {
        }

        public MyProgram(ArgStyle argStyle, ArgGrouping grouping = ArgGrouping.DoesNotMatter) : base(argStyle, grouping)
        {
        }

        [Argument("source")]
        public FileInfo SourceFile { get; set; }

        public DirectoryInfo Destination { get; set; }

        public bool Overwrite { get; set; }

        protected override IEnumerable<Arg> GetArgs()
        {
            yield return new Argument("source")
                .ValidateAsFile(shouldExist: false)
                .TypedAs(fileName => new FileInfo(fileName));
            yield return new Argument("destination")
                .ValidateAsDirectory(shouldExist: false)
                .TypedAs(dirName => new DirectoryInfo(dirName));
            yield return new Option("overwrite", "o")
                .UsedAsFlag();
        }

        protected override int HandleCommand()
        {
            PrintLine($"Copy {Cyan}{SourceFile} {Reset}to {Green}{Destination}");
            PrintLine($"Overwrite: {Yellow}{Overwrite}");
            return 0;
        }
    }
}
