// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Applicable arg types for a specific <see cref="MetadataAttribute"/> attribute.
    /// </summary>
    [Flags]
    public enum ApplicableArgTypesForMetadata
    {
        Option = 1,
        Argument = 2,
        Command = 4,
        ArgumentsAndOptions = Argument | Option,
        All = Argument | Option | Command,
    }
}
