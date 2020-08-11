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

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Represents the result of parsing a collection of tokens.
    /// </summary>
    public interface IParseResult
    {
        /// <summary>
        ///     Gets the top-level <see cref="Command"/>.
        ///     <para/>
        ///     The <see cref="Command"/> instance has properties such as
        ///     <see cref="Command.ParentCommand"/> and <see cref="Command.RootCommand"/> to traverse up
        ///     the command chain.
        /// </summary>
        Command Command { get; }

        /// <summary>
        ///     Gets the list of specified command line arguments.
        /// </summary>
        IReadOnlyList<object> Arguments { get; }

        /// <summary>
        ///     Gets the list of specified command line options.
        /// </summary>
        IReadOnlyDictionary<string, object> Options { get; }

        bool TryGetArgument<T>(int index, out T value);

        bool TryGetOption<T>(string name, out T value);
    }
}
