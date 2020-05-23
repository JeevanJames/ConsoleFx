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
        protected sealed override string PerformCustomValidation(IReadOnlyList<object> arguments, IReadOnlyDictionary<string, object> options)
        {
            return base.PerformCustomValidation(arguments, options);
        }
    }
}
