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
using System.Diagnostics;

using ConsoleFx.CmdLineArgs;

namespace ConsoleFx.Program
{
    public class ProgramCommand : Command
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CommandHandler _handler;

        public ProgramCommand()
        {
        }

        public ProgramCommand(params string[] names)
            : base(names)
        {
        }

        public ProgramCommand(bool caseSensitive, params string[] names)
            : base(caseSensitive, names)
        {
        }

        public CommandHandler Handler
        {
            get => _handler ?? HandleCommand;
            set => _handler = value;
        }

        protected virtual int HandleCommand(IReadOnlyList<object> arguments,
            IReadOnlyDictionary<string, object> options)
        {
            return 0;
        }
    }
}
