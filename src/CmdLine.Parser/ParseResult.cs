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
    public sealed class ParseResult : ParseResultBase
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

        public override bool TryGetArgument<T>(int index, out T value, T defaultValue = default)
        {
            if (index >= _run.Arguments.Count)
            {
                value = default;
                return false;
            }

            ArgumentRun matchingArgument = _run.Arguments[index];

            return TryGetArgument(matchingArgument, out value, defaultValue);
        }

        public override bool TryGetArgument<T>(string name, out T value, T defaultValue = default)
        {
            ArgumentRun matchingArgument = _run.Arguments.FirstOrDefault(r => r.Argument.HasName(name));
            if (matchingArgument is null)
            {
                value = default;
                return false;
            }

            return TryGetArgument(matchingArgument, out value, defaultValue);
        }

        private static bool TryGetArgument<T>(ArgumentRun matchingArgument, out T value, T @default = default)
        {
            if (!matchingArgument.Assigned)
            {
                value = @default;
                return true;
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

        public override bool TryGetOption<T>(string name, out T value, T defaultValue = default)
        {
            OptionRun matchingOption = _run.Options.FirstOrDefault(r => r.Option.HasName(name));
            if (matchingOption is null)
            {
                value = default;
                return false;
            }

            if (matchingOption.Occurrences == 0)
            {
                value = matchingOption.Assigned ? (T)matchingOption.Value : defaultValue;
                return true;
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

        public override bool TryGetOptions<T>(string name, out IReadOnlyList<T> values)
        {
            bool found = TryGetOption(name, out List<T> list, new List<T>(0));
            values = found ? list : default;
            return found;
        }
    }
}
