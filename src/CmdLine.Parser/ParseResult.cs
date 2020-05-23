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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ConsoleFx.CmdLine.Parser.Runs;

namespace ConsoleFx.CmdLine.Parser
{
    /// <summary>
    ///     Represents the results of parsing a set of arguments.
    /// </summary>
    public sealed class ParseResult : IParseResult
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ParseRun _run;

        internal ParseResult(ParseRun run)
        {
            _run = run;
            Command = run.Commands[run.Commands.Count - 1];
            Arguments = run.Arguments
                .Select(ar => ar.Value)
                .ToList();
            Options = run.Options
                .ToDictionary(rootOptionRun => rootOptionRun.Option.Name, rootOptionRun => rootOptionRun.Value);
        }

        public Command Command { get; }

        /// <summary>
        ///     Gets the list of specified command line arguments.
        /// </summary>
        public IReadOnlyList<object> Arguments { get; }

        /// <summary>
        ///     Gets the list of specified command line options.
        /// </summary>
        public IReadOnlyDictionary<string, object> Options { get; }

        public bool TryGetArgument<T>(int index, out T value)
        {
            if (index >= _run.Arguments.Count)
            {
                value = default;
                return false;
            }

            ArgumentRun matchingArgument = _run.Arguments[index];

            return TryGetArgument(matchingArgument, out value);
        }

        private static bool TryGetArgument<T>(ArgumentRun matchingArgument, out T value)
        {
            if (!matchingArgument.Assigned)
            {
                value = default;
                return false;
            }

            object resolvedValue = matchingArgument.Value;
            if (resolvedValue != null)
            {
                Type valueType = resolvedValue.GetType();
                if (!typeof(T).IsAssignableFrom(valueType))
                    throw new InvalidOperationException($"The argument's value is of type '{valueType.FullName}' is not assignable to the specified type of '{typeof(T).FullName}'.");
            }

            value = (T)matchingArgument.Value;
            return true;
        }

        public bool TryGetOption<T>(string name, out T value)
        {
            OptionRun matchingOption = _run.Options.FirstOrDefault(r => r.Option.HasName(name));
            if (matchingOption is null)
            {
                value = default;
                return false;
            }

            if (matchingOption.Occurrences == 0)
            {
                if (matchingOption.Option.DefaultSetter != null)
                {
                    value = (T)matchingOption.Option.DefaultSetter();
                    return true;
                }

                value = default;
                return false;
            }

            object resolvedValue = matchingOption.Value;
            if (resolvedValue != null)
            {
                Type valueType = resolvedValue.GetType();
                if (!typeof(T).IsAssignableFrom(valueType))
                    throw new InvalidOperationException($"The option's value is of type '{valueType.FullName}' is not assignable to the specified type of '{typeof(T).FullName}'.");
            }

            value = (T)resolvedValue;
            return true;
        }
    }
}
