// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Specifies how the command-line args are expected to be grouped.
    /// </summary>
    public enum ArgGrouping
    {
        /// <summary>
        ///     Command line parameters grouping does not matter. Options and arguments can be mixed together.
        ///     <para/>
        ///     This is the default grouping.
        /// </summary>
        DoesNotMatter,

        /// <summary>
        ///     Options must be specified before arguments in the command line.
        /// </summary>
        OptionsBeforeArguments,

        /// <summary>
        ///     Options must be specified after arguments in the command line.
        /// </summary>
        OptionsAfterArguments,
    }
}
