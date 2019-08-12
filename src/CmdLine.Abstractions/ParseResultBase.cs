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

namespace ConsoleFx.CmdLine
{
    [DebuggerDisplay("Command: {Command.Name}")]
    public abstract class ParseResultBase
    {
        public Command Command { get; protected set; }

        /// <summary>
        ///     Gets or sets the list of specified command line arguments.
        /// </summary>
        public IReadOnlyList<object> Arguments { get; protected set; }

        /// <summary>
        ///     Gets or sets the list of specified command line options.
        /// </summary>
        public IReadOnlyDictionary<string, object> Options { get; protected set; }

        public abstract bool TryGetArgument<T>(int index, out T value, T @default = default);

        public abstract bool TryGetArgument<T>(string name, out T value, T @default = default);

        public abstract bool TryGetOption<T>(string name, out T value, T @default = default);

        public abstract bool TryGetOptions<T>(string name, out IReadOnlyList<T> values);
    }
}
