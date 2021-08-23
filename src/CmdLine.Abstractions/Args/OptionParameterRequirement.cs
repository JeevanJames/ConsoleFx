// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Specifies whether the parameters for an option are required or not allowed.
    /// </summary>
    /// <remarks>There is no such thing as optional parameters.</remarks>
    public enum OptionParameterRequirement
    {
        /// <summary>
        ///     Parameters are not allowed for the option.
        /// </summary>
        NotAllowed,

        /// <summary>
        ///     The option requires one or more parameters.
        /// </summary>
        Required,
    }
}
