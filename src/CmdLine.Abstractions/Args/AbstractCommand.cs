// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace ConsoleFx.CmdLine
{
    /// <summary>
    ///     Represents a command that doesn't do anything but be a parent for other commands.
    /// </summary>
    public class AbstractCommand : Command
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AbstractCommand"/> class.
        /// </summary>
        public AbstractCommand()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AbstractCommand"/> class with the
        ///     specified names.
        /// </summary>
        /// <param name="names">The names to assign to the command.</param>
        public AbstractCommand(params string[] names)
            : base(names)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AbstractCommand"/> class with the
        ///     specified names.
        /// </summary>
        /// <param name="caseSensitive">Specifies whether the names are case sensitive or not.</param>
        /// <param name="names">The names to assign to the command.</param>
        public AbstractCommand(bool caseSensitive, params string[] names)
            : base(caseSensitive, names)
        {
        }

        /// <inheritdoc />
        protected sealed override IEnumerable<Arg> GetArgs()
        {
            yield break;
        }

        /// <inheritdoc />
        protected sealed override int HandleCommand()
        {
            DisplayHelp(this);
            return 0;
        }

        /// <inheritdoc />
        protected sealed override string PerformCustomValidation(IReadOnlyList<object> arguments,
            IReadOnlyDictionary<string, object> options)
        {
            return base.PerformCustomValidation(arguments, options);
        }
    }
}
