// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Specifies whether an option is required or optional.
    /// </summary>
    public enum OptionRequirement
    {
        /// <summary>
        ///     The option is optional (this is the default). Sets the option's MinOccurences property
        ///     to 0 (zero) and MaxOccurences property to 1 (one). However, the MaxOccurence value can
        ///     be increased, and as long as the MinOccurence value is zero, it will be considered
        ///     optional.
        /// </summary>
        Optional,

        /// <summary>
        ///     The option is optional. Sets the option's MinOccurences property to 0 and MaxOccurences
        ///     property to int.MaxValue to indicate unlimited number of occurences.
        /// </summary>
        OptionalUnlimited,

        /// <summary>
        ///     The option is required. Sets the option's MinOccurences and MaxOccurences properties to
        ///     1 (one).
        /// </summary>
        Required,

        /// <summary>
        ///     The option is required. Sets the option's MinOccurences property to 1 and MaxOccurences
        ///     property to int.MaxValue to indicate unlimited number of occurences.
        /// </summary>
        RequiredUnlimited,
    }
}
