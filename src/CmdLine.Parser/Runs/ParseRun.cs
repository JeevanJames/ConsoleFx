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

namespace ConsoleFx.CmdLine.Parser.Runs
{
    /// <summary>
    ///     Represents the internal state of a single parse execution. This includes the commands,
    ///     arguments and options that were specified.
    /// </summary>
    internal sealed class ParseRun
    {
        /// <summary>
        ///     Gets all specified commands.
        ///     <para />
        ///     Note: We use a <see cref="List{T}" /> instead of the <see cref="Commands"/> collection
        ///     here, because we want to avoid the duplicate checks, as commands at different levels
        ///     can have the same name.
        /// </summary>
        internal List<Command> Commands { get; } = new List<Command>();

        /// <summary>
        ///     Gets all allowed arguments and their values.
        /// </summary>
        internal List<ArgumentRun> Arguments { get; } = new List<ArgumentRun>();

        /// <summary>
        ///     Gets all allowed options and details of which are specified.
        /// </summary>
        internal List<OptionRun> Options { get; } = new List<OptionRun>();

        /// <summary>
        ///     Gets or sets all the specified options and argument tokens after accounting for the
        ///     commands.
        /// </summary>
        internal List<string> Tokens { get; set; }
    }
}
