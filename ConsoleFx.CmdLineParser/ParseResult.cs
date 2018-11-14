#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2018 Jeevan James

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
        ///     List of specified command line arguments.
        /// </summary>
        public IReadOnlyList<string> Arguments { get; }

        /// <summary>
        ///     List of specified command line options.
        /// </summary>
        public IReadOnlyDictionary<string, object> Options { get; }

        /// <summary>
        ///     Returns the typed value of the specified option.
        /// </summary>
        /// <typeparam name="T">The type of the value to return.</typeparam>
        /// <param name="name">Name of the specified option.</param>
        /// <param name="default">Default value to return if the option is not found.</param>
        /// <returns>The typed value of the specified option.</returns>
        public T OptionAs<T>(string name, T @default = default(T)) =>
            Options.TryGetValue(name, out object value) ? (T)value : @default;

        public IReadOnlyList<T> OptionsAsListOf<T>(string name) =>
            Options.TryGetValue(name, out object value) ? (List<T>)value : null;

        public string Option(string name) =>
            OptionAs<string>(name);

        public IReadOnlyList<string> OptionsAsList(string name) =>
            OptionsAsListOf<string>(name);
    }

    //public abstract class BaseParseResult
    //{
    //    public ParseCommandResult Command { get; internal set; }

    //    internal Dictionary<string, object> InternalOptions { get; } = new Dictionary<string, object>();

    //    public IReadOnlyDictionary<string, object> Options => InternalOptions;

    //    public T OptionAs<T>(string name, T @default = default(T))
    //    {
    //        object value;
    //        return Options.TryGetValue(name, out value) ? (T)value : @default;
    //    }

    //    public IReadOnlyList<T> OptionsAsListOf<T>(string name)
    //    {
    //        object value;
    //        return Options.TryGetValue(name, out value) ? (List<T>)value : null;
    //    }

    //    public string Option(string name) => OptionAs<string>(name);

    //    public IReadOnlyList<string> OptionsAsList(string name) => OptionsAsListOf<string>(name);
    //}

    ///// <summary>
    /////     Represents results of parsing a specified set of tokens. This class represents the results for the root command.
    /////     Results for all commands (if any) are stored hierarchially under the <see cref="BaseParseResult.Command" />
    /////     property.
    /////     Options for the root command as well as all subsequent commands are available under the
    /////     <see cref="BaseParseResult.Options" /> property of the respective result classes.
    /////     Since only the arguments of the last command can be considered, these are stored in the root result and not the
    /////     result of the final command, to avoid unnecessary traversal.
    ///// </summary>
    //public sealed class ParseResult : BaseParseResult
    //{
    //    internal ParseResult()
    //    {
    //    }

    //    internal List<string> InternalArguments { get; } = new List<string>();

    //    public IReadOnlyList<string> Arguments => InternalArguments;
    //}

    //public sealed class ParseCommandResult : BaseParseResult
    //{
    //    internal ParseCommandResult(string name)
    //    {
    //        Name = name;
    //    }

    //    public string Name { get; }
    //}
}
