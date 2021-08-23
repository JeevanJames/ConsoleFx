// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Encapsulates a method that performs custom validation once a <see cref="Command"/> instance
    ///     has been initialized.
    /// </summary>
    /// <param name="arguments">The arguments passed to the command.</param>
    /// <param name="options">The options passed to the command.</param>
    /// <returns>A validation error message, if the validation fails; otherwise <c>null</c>.</returns>
    public delegate string CommandCustomValidator(IReadOnlyList<object> arguments,
        IReadOnlyDictionary<string, object> options);
}
