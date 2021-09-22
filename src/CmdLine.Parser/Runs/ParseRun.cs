// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

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
        ///     Note: We use a <see cref="List{T}" /> instead of the <see cref="CmdLine.Commands"/>
        ///     collection here, because we want to avoid the duplicate checks, as commands at different
        ///     levels can have the same name.
        /// </summary>
        internal List<Command> Commands { get; } = new();

        /// <summary>
        ///     Gets all allowed arguments and their values.
        /// </summary>
        internal List<ArgumentRun> Arguments { get; } = new();

        /// <summary>
        ///     Gets all allowed options and details of which are specified.
        /// </summary>
        internal List<OptionRun> Options { get; } = new();

        /// <summary>
        ///     Gets or sets all the specified options and argument tokens after accounting for the
        ///     commands.
        /// </summary>
        internal List<string> Tokens { get; set; }

        internal static readonly List<string> EmptyTokens = new(0);
    }
}
