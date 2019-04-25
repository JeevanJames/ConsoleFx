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

using ConsoleFx.CmdLineArgs;

namespace ConsoleFx.CmdLineParser
{
    /// <summary>
    ///     Represents the results of parsing a set of arguments.
    /// </summary>
    public sealed class ParseResult
    {
        internal ParseResult(Command command, IReadOnlyList<string> arguments,
            IReadOnlyDictionary<string, object> options)
        {
            Command = command;
            Arguments = arguments;
            Options = options;
        }

        public Command Command { get; }

        /// <summary>
        ///     Gets the list of specified command line arguments.
        /// </summary>
        public IReadOnlyList<string> Arguments { get; }

        /// <summary>
        ///     Gets the list of specified command line options.
        /// </summary>
        public IReadOnlyDictionary<string, object> Options { get; }

        /// <summary>
        ///     Returns the typed value of the specified option.
        /// </summary>
        /// <typeparam name="T">The type of the value to return.</typeparam>
        /// <param name="name">Name of the specified option.</param>
        /// <param name="default">Default value to return if the option is not found.</param>
        /// <returns>The typed value of the specified option.</returns>
        public T OptionAs<T>(string name, T @default = default) =>
            Options.TryGetValue(name, out object value) ? (T)value : @default;

        public IReadOnlyList<T> OptionsAsListOf<T>(string name) =>
            Options.TryGetValue(name, out object value) ? (List<T>)value : null;

        public string Option(string name) =>
            OptionAs<string>(name);

        public IReadOnlyList<string> OptionsAsList(string name) =>
            OptionsAsListOf<string>(name);
    }
}
