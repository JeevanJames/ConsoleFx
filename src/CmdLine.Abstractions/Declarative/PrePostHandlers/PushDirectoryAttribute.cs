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

using System.Diagnostics;
using System.IO;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Restores the current directory to the directory that was current before the command was
    ///     executed.
    /// </summary>
    public sealed class PushDirectoryAttribute : PrePostHandlerAttribute
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _originalDirectory;

        public override void BeforeHandler(Command command)
        {
            _originalDirectory = Directory.GetCurrentDirectory();
        }

        public override void AfterHandler(Command command)
        {
            Directory.SetCurrentDirectory(_originalDirectory);
        }
    }
}
