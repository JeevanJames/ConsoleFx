// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Specify whether the parameters of an option are repeating or individual.
    /// </summary>
    public enum OptionParameterType
    {
        /// <summary>
        ///     The parameters are repeating and have the same meaning.
        /// </summary>
        Repeating,

        /// <summary>
        ///     Each parameter is independent, has its own meaning and is at a specific position.
        /// </summary>
        Individual,
    }
}
