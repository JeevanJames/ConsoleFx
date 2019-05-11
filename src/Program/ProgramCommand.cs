#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2019 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System;
using System.Diagnostics;

using ConsoleFx.CmdLineArgs;
using ConsoleFx.CmdLineParser;

namespace ConsoleFx.Program
{
    /// <summary>
    ///     Represents a <see cref="Command"/> that contains a <see cref="Handler"/> to execute
    ///     code if the command matches the command-line args.
    /// </summary>
    public class ProgramCommand : Command
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Func<ParseResult, int> _handler;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgramCommand"/> class.
        /// </summary>
        public ProgramCommand()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgramCommand"/> class with the specified
        ///     <paramref name="names"/>.
        /// </summary>
        /// <param name="names">The names of the command.</param>
        public ProgramCommand(params string[] names)
            : base(names)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgramCommand"/> class with the specified
        ///     <paramref name="names"/> and case-sensitivity.
        /// </summary>
        /// <param name="caseSensitive">Indicates whether the names are case-sensitive.</param>
        /// <param name="names">The names of the command.</param>
        public ProgramCommand(bool caseSensitive, params string[] names)
            : base(caseSensitive, names)
        {
        }

        /// <summary>
        ///     Gets the result of parsing the args of this command.
        /// </summary>
        /// <remarks>
        ///     Note: This property will only be available to the <see cref="HandleCommand"/> virtual
        ///     method, which is called after the args have been parsed.
        /// </remarks>
        public ParseResult ParseResult { get; internal set; }

        /// <summary>
        ///     Gets or sets the delegate to call if the parsed args match this command.
        ///     <para/>
        ///     If not assigned, the virtual <see cref="HandleCommand"/> method is called.
        /// </summary>
        public Func<ParseResult, int> Handler
        {
            get => _handler ?? (_ => HandleCommand());
            set => _handler = value;
        }

        protected virtual int HandleCommand()
        {
            return 0;
        }
    }
}
