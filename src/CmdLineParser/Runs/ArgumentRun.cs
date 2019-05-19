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

using System.Diagnostics;

using ConsoleFx.CmdLine;

namespace ConsoleFx.CmdLine.Parser.Runs
{
    [DebuggerDisplay("{" + nameof(Argument) + "}")]
    internal sealed class ArgumentRun : ArgumentOrOptionRun<Argument>
    {
        internal ArgumentRun(Argument argument)
            : base(argument)
        {
        }

        internal Argument Argument => Arg;

        internal bool Assigned { get; set; }
    }
}
