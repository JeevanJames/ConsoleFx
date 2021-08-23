// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Represents the result of parsing a collection of tokens.
    /// </summary>
    public interface IParseResult
    {
        /// <summary>
        ///     Gets the top-level <see cref="Command"/>.
        ///     <para/>
        ///     The <see cref="Command"/> instance has properties such as
        ///     <see cref="Command.ParentCommand"/> and <see cref="Command.RootCommand"/> to traverse up
        ///     the command chain.
        /// </summary>
        Command Command { get; }

        /// <summary>
        ///     Gets the list of specified command line arguments.
        /// </summary>
        IReadOnlyList<object> Arguments { get; }

        /// <summary>
        ///     Gets the list of specified command line options.
        /// </summary>
        IReadOnlyDictionary<string, object> Options { get; }

        bool TryGetArgument<T>(int index, out T value);

        bool TryGetOption<T>(string name, out T value);
    }
}
