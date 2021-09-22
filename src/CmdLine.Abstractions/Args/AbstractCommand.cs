// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;

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

        /// <inheritdoc />
        protected sealed override IEnumerable<Arg> GetArgs()
        {
            yield break;
        }

        /// <inheritdoc />
        public sealed override string ValidateParseResult(IReadOnlyList<object> arguments,
            IReadOnlyDictionary<string, object> options)
        {
            return base.ValidateParseResult(arguments, options);
        }

        /// <inheritdoc />
        internal sealed override Task<int> HandleCommandAsync(IParseResult parseResult)
        {
            return Task.FromResult(0);
        }
    }
}
